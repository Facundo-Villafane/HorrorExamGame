using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyPatrol : MonoBehaviour
{
    // Referencias a componentes
    public NavMeshAgent agent;
    public Animator animator;
    
    // Puntos de patrulla
    public Transform[] patrolPoints;
    
    // Configuración de comportamiento
    public float patrolSpeed = 3f;
    public float chaseSpeed = 5f;
    public float fleeSpeed = 7f; // Nueva velocidad para huir
    public float attackRange = 2f;
    public float detectionRange = 10f;
    public float waitTimeAtPoint = 2f;
    
    // Sistema de daño
    public int damageAmount = 1;
    public float attackCooldown = 1.5f;
    private float lastAttackTime = 0f;
    public AudioSource attackSound;
    public AudioSource hurtSound; // Sonido para cuando recibe daño
    
    // Estado actual
    private enum State { Patrolling, Chasing, Attacking, Waiting, Fleeing } // Añadido estado Fleeing
    private State currentState;
    
    // Variables de control
    private int currentPatrolIndex = 0;
    private Transform player;
    private float waitTimer = 0f;
    private Coroutine fleeCoroutine; // Referencia a la corrutina de huida
    
    // Referencia al script de salud del jugador
    private PlayerHealth playerHealth;
    
    // Para evitar que la animación de caminar se reinicie
    private bool hasReachedDestination = false;
    
    void Start()
    {
        // Verificar que haya puntos de patrulla
        if (patrolPoints.Length == 0)
        {
            Debug.LogError("No patrol points assigned to enemy!");
            return;
        }
        
        // Buscar al jugador si no está asignado
        if (player == null)
            player = GameObject.FindGameObjectWithTag("Player")?.transform;
            
        // Buscar el script de salud del jugador
        if (player != null)
            playerHealth = player.GetComponent<PlayerHealth>();
        
        // Asignar componentes si no están asignados
        if (agent == null)
            agent = GetComponent<NavMeshAgent>();
        if (animator == null)
            animator = GetComponent<Animator>();
        
        // Iniciar en modo patrulla
        currentState = State.Patrolling;
        hasReachedDestination = false;
        
        // Forzar la animación de caminar al inicio
        SetAnimationState(false, true, false);
        
        // Configurar el primer destino
        agent.isStopped = false;
        agent.speed = patrolSpeed;
        agent.SetDestination(patrolPoints[0].position);
    }
    
    void Update()
    {
        // Si estamos huyendo, saltarse toda la lógica normal
        if (currentState == State.Fleeing)
            return;
            
        // Si no hay referencia al jugador, no podemos continuar
        if (player == null)
            return;
        
        // Comprobar si estamos realmente moviéndonos
        bool isActuallyMoving = agent.velocity.magnitude > 0.15f;
        
        // Actualizar animación basada en el movimiento actual
        if (currentState != State.Attacking && currentState != State.Waiting)
        {
            if (isActuallyMoving && !animator.GetBool("isChasing"))
            {
                SetAnimationState(false, true, false);
            }
            else if (!isActuallyMoving && !animator.GetBool("isIdle") && hasReachedDestination)
            {
                SetAnimationState(true, false, false);
            }
        }
        
        // Si el enemigo está esperando
        if (currentState == State.Waiting)
        {
            // Asegurar que está reproduciendo la animación correcta
            if (!animator.GetBool("isIdle"))
            {
                SetAnimationState(true, false, false);
            }
            
            waitTimer += Time.deltaTime;
            if (waitTimer >= waitTimeAtPoint)
            {
                hasReachedDestination = false;
                currentState = State.Patrolling;
                GoToNextPatrolPoint();
            }
        }
        
        // Comprobar distancia al jugador
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);
        
        // Si el jugador está en rango de detección y no estamos ya atacando
        if (distanceToPlayer <= detectionRange && currentState != State.Attacking)
        {
            // Si no estábamos persiguiendo, empezar persecución
            if (currentState != State.Chasing)
            {
                hasReachedDestination = false;
                currentState = State.Chasing;
                agent.speed = chaseSpeed;
                agent.isStopped = false;
                
                // Establecer animación de persecución solo si no está ya reproduciéndola
                if (!animator.GetBool("isChasing"))
                {
                    SetAnimationState(false, true, false);
                }
            }
            
            // Perseguir al jugador
            agent.SetDestination(player.position);
            
            // Si estamos lo suficientemente cerca para atacar
            if (distanceToPlayer <= attackRange)
            {
                // Intentar atacar si ha pasado el tiempo de enfriamiento
                if (Time.time > lastAttackTime + attackCooldown)
                {
                    AttackPlayer();
                }
            }
        }
        else if (currentState == State.Patrolling)
        {
            // Si estamos patrullando y hemos llegado al destino
            if (!agent.pathPending && agent.remainingDistance < 0.5f && !hasReachedDestination)
            {
                hasReachedDestination = true;
                currentState = State.Waiting;
                agent.isStopped = true;
                
                // Establecer animación de idle
                SetAnimationState(true, false, false);
                waitTimer = 0f;
            }
        }
    }
    
    // Método para atacar al jugador
    void AttackPlayer()
    {
        currentState = State.Attacking;
        agent.isStopped = true;
        
        // Actualizar tiempo del último ataque
        lastAttackTime = Time.time;
        
        // Establecer animación de ataque
        SetAnimationState(false, false, true);
        
        // Reproducir sonido de ataque si existe
        if (attackSound != null)
        {
            attackSound.Play();
        }
        
        // Infligir daño al jugador si tiene el componente PlayerHealth
        if (playerHealth != null)
        {
            playerHealth.TakeDamage(damageAmount);
        }
        
        // Comenzar un breve periodo de enfriamiento después del ataque
        StartCoroutine(ResetAfterAttack());
    }
    
    // Nuevo método para huir del jugador cuando recibe daño
    public void FleeFromPlayer(float duration)
    {
        // Cancelar la huida anterior si está en curso
        //if (fleeCoroutine != null)
        //    StopCoroutine(fleeCoroutine);
            
        // Iniciar nueva huida
        //fleeCoroutine = StartCoroutine(FleeCoroutine(duration));
    }
    
    // Corrutina para manejar el comportamiento de huida
    private IEnumerator FleeCoroutine(float duration)
    {
        if (agent == null || player == null)
            yield break;
            
        // Guardar el estado actual para restaurarlo después
        State previousState = currentState;
        
        // Cambiar al estado de huida
        currentState = State.Fleeing;
        
        // Calcular dirección de huida (opuesta al jugador)
        Vector3 fleeDirection = transform.position - player.position;
        fleeDirection.y = 0; // Mantener en el mismo plano
        fleeDirection.Normalize();
        
        // Establecer destino de huida (alejarse del jugador)
        Vector3 fleeDestination = transform.position + fleeDirection * 15f;
        
        // Intentar encontrar un punto válido en el NavMesh
        NavMeshHit hit;
        if (NavMesh.SamplePosition(fleeDestination, out hit, 15f, NavMesh.AllAreas))
        {
            fleeDestination = hit.position;
        }
        
        // Configurar el agente de navegación
        agent.isStopped = false;
        agent.speed = fleeSpeed;
        agent.SetDestination(fleeDestination);
        
        // Actualizar animación a "corriendo"
        SetAnimationState(false, true, false);
        
        // Reproducir sonido de dolor si existe
        if (hurtSound != null)
        {
            hurtSound.Play();
        }
        
        // Esperar la duración especificada
        yield return new WaitForSeconds(duration);
        
        // Restaurar el estado anterior
        if (previousState == State.Patrolling || previousState == State.Waiting)
        {
            currentState = State.Patrolling;
            GoToNextPatrolPoint();
        }
        else if (previousState == State.Chasing)
        {
            float distanceToPlayer = Vector3.Distance(transform.position, player.position);
            
            if (distanceToPlayer <= detectionRange)
            {
                currentState = State.Chasing;
                agent.speed = chaseSpeed;
                agent.SetDestination(player.position);
                SetAnimationState(false, true, false);
            }
            else
            {
                currentState = State.Patrolling;
                GoToNextPatrolPoint();
            }
        }
        
        // Limpiar la referencia a la corrutina
        fleeCoroutine = null;
    }
    
    void GoToNextPatrolPoint()
    {
        // Incrementar el índice de patrulla
        currentPatrolIndex = (currentPatrolIndex + 1) % patrolPoints.Length;
        
        // Moverse al siguiente punto de patrulla
        agent.isStopped = false;
        agent.speed = patrolSpeed;
        
        agent.SetDestination(patrolPoints[currentPatrolIndex].position);
        
        // Establecer animación de caminar
        SetAnimationState(false, true, false);
    }
    
    IEnumerator ResetAfterAttack()
    {
        // Esperar a que termine la animación de ataque
        yield return new WaitForSeconds(1.5f); // Ajusta esto según la duración de tu animación
        
        // Volver a perseguir
        if (player != null)
        {
            float distanceToPlayer = Vector3.Distance(transform.position, player.position);
            
            if (distanceToPlayer <= detectionRange)
            {
                hasReachedDestination = false;
                currentState = State.Chasing;
                agent.isStopped = false;
                agent.speed = chaseSpeed;
                SetAnimationState(false, true, false);
                agent.SetDestination(player.position);
            }
            else
            {
                hasReachedDestination = false;
                currentState = State.Patrolling;
                GoToNextPatrolPoint();
            }
        }
    }
    
    // Método para establecer las animaciones
    void SetAnimationState(bool isIdle, bool isChasing, bool isAttacking)
    {
        // Solo cambiar los estados si realmente son diferentes de los actuales
        bool idleChanged = animator.GetBool("isIdle") != isIdle;
        bool chasingChanged = animator.GetBool("isChasing") != isChasing;
        bool attackingChanged = animator.GetBool("isAttacking") != isAttacking;
        
        if (idleChanged || chasingChanged || attackingChanged)
        {
            // Cambiar gradualmente para evitar saltos en la animación
            animator.SetBool("isIdle", isIdle);
            animator.SetBool("isChasing", isChasing);
            animator.SetBool("isAttacking", isAttacking);
            
            // Si tienes un parámetro para huir, manejarlo aquí
            // animator.SetBool("isFleeing", currentState == State.Fleeing);
        }
    }
    
    // Para visualizar los rangos en el editor
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRange);
        
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
        
        // Dibujar líneas entre puntos de patrulla
        if (patrolPoints != null && patrolPoints.Length > 0)
        {
            Gizmos.color = Color.blue;
            for (int i = 0; i < patrolPoints.Length; i++)
            {
                if (patrolPoints[i] != null)
                {
                    Vector3 pos = patrolPoints[i].position;
                    pos.y += 0.5f; // Elevarlo un poco para que se vea mejor
                    Gizmos.DrawSphere(pos, 0.3f);
                    
                    if (i < patrolPoints.Length - 1 && patrolPoints[i+1] != null)
                    {
                        Gizmos.DrawLine(patrolPoints[i].position, patrolPoints[i+1].position);
                    }
                    else if (patrolPoints[0] != null)
                    {
                        Gizmos.DrawLine(patrolPoints[i].position, patrolPoints[0].position);
                    }
                }
            }
        }
    }
    
    // Limpiar las corrutinas al desactivar el GameObject
    void OnDisable()
    {
        if (fleeCoroutine != null)
        {
            StopCoroutine(fleeCoroutine);
            fleeCoroutine = null;
        }
    }
}