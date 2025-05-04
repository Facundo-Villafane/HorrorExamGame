using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyPatrol : MonoBehaviour
{
    // Referencias a componentes
    [SerializeField] private NavMeshAgent agent;
    [SerializeField] private Animator animator;
    
    // Puntos de patrulla
    [SerializeField] private Transform[] patrolPoints;
    
    // Configuración de comportamiento
    [SerializeField] private float patrolSpeed = 3f;
    [SerializeField] private float chaseSpeed = 5f;
    [SerializeField] private float attackRange = 2f;
    [SerializeField] private float detectionRange = 10f;
    [SerializeField] private float waitTimeAtPoint = 2f;
    
    // Estado actual
    private enum State { Patrolling, Chasing, Attacking, Waiting }
    private State currentState;
    
    // Variables de control
    private int currentPatrolIndex = 0;
    private Transform player;
    private float waitTimer = 0f;
    
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
        
        // Asignar componentes si no están asignados
        if (agent == null)
            agent = GetComponent<NavMeshAgent>();
        if (animator == null)
            animator = GetComponent<Animator>();
        
        // Iniciar en modo patrulla
        currentState = State.Patrolling;
        GoToNextPatrolPoint();
    }
    
    void Update()
    {
        // Si no hay referencia al jugador, no podemos continuar
        if (player == null)
            return;
        
        // Si el enemigo está esperando
        if (currentState == State.Waiting)
        {
            waitTimer += Time.deltaTime;
            if (waitTimer >= waitTimeAtPoint)
            {
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
                currentState = State.Chasing;
                agent.speed = chaseSpeed;
                SetAnimation("run");
            }
            
            // Si estamos lo suficientemente cerca para atacar
            if (distanceToPlayer <= attackRange)
            {
                currentState = State.Attacking;
                agent.isStopped = true;
                SetAnimation("attack");
                StartCoroutine(ResetAfterAttack());
            }
            else
            {
                // Perseguir al jugador
                agent.SetDestination(player.position);
            }
        }
        else if (currentState == State.Patrolling)
        {
            // Si estamos patrullando y hemos llegado al destino
            if (!agent.pathPending && agent.remainingDistance < 0.5f)
            {
                currentState = State.Waiting;
                agent.isStopped = true;
                SetAnimation("idle");
                waitTimer = 0f;
            }
        }
    }
    
    void GoToNextPatrolPoint()
    {
        // Moverse al siguiente punto de patrulla
        agent.isStopped = false;
        agent.speed = patrolSpeed;
        
        agent.SetDestination(patrolPoints[currentPatrolIndex].position);
        SetAnimation("walk");
        
        // Pasar al siguiente punto para la próxima vez
        currentPatrolIndex = (currentPatrolIndex + 1) % patrolPoints.Length;
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
                currentState = State.Chasing;
                agent.isStopped = false;
                agent.speed = chaseSpeed;
                SetAnimation("run");
            }
            else
            {
                currentState = State.Patrolling;
                GoToNextPatrolPoint();
            }
        }
    }
    
    void SetAnimation(string animName)
    {
        // Resetear todas las animaciones
        animator.ResetTrigger("idle");
        animator.ResetTrigger("walk");
        animator.ResetTrigger("run");
        animator.ResetTrigger("attack");
        
        // Activar la animación deseada
        animator.SetTrigger(animName);
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
}