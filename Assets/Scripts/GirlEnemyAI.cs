using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class GirlEnemyAI : MonoBehaviour
{
    public NavMeshAgent agent;
    public Transform player;
    public Animator animator;
    public AudioSource enemyAudioSource;
    public AudioClip[] enemySounds;
    public AudioClip attackSound;
    
    // Estados de comportamiento
    public bool isChasing = false;
    public bool isAttacking = false;
    public bool isFleeing = false; // Nuevo estado para huir
    
    // Velocidades configurables
    public float chaseSpeed = 5.0f;
    public float fleeSpeed = 7.0f;
    
    // Distancias
    public float chaseRange = 15f;
    public float attackRange = 2f;
    public float minTimeBetweenSounds = 8f;
    
    // Variables para el ataque
    public float attackCooldown = 1.5f;
    private float lastAttackTime = 0f;
    public int damageAmount = 1;
    
    private float lastSoundTime = 0f;
    private Coroutine fleeCoroutine;
    
    // Referencia al script de vida del jugador
    private PlayerHealth playerHealth;
    
    void Start()
    {
        // Asegurar que tenemos las referencias necesarias
        if (agent == null)
            agent = GetComponent<NavMeshAgent>();
            
        if (animator == null)
            animator = GetComponent<Animator>();
            
        if (enemyAudioSource == null)
            enemyAudioSource = GetComponent<AudioSource>();
            
        // Obtener referencia al script de salud del jugador
        if (player != null)
        {
            playerHealth = player.GetComponent<PlayerHealth>();
        }
    }
    
    void Update()
    {
        // Si está huyendo, saltarse toda la lógica de persecución y ataque
        if (isFleeing)
            return;
            
        if (player == null)
            return;
            
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);
        
        // Verificar si debe perseguir al jugador
        if (distanceToPlayer < chaseRange)
        {
            isChasing = true;
            agent.SetDestination(player.position);
            agent.speed = chaseSpeed;
            
            // Verificar si está lo suficientemente cerca para atacar
            if (distanceToPlayer < attackRange)
            {
                isAttacking = true;
                
                // Si ha pasado suficiente tiempo desde el último ataque
                if (Time.time > lastAttackTime + attackCooldown)
                {
                    Attack();
                }
            }
            else
            {
                isAttacking = false;
            }
        }
        else
        {
            isChasing = false;
            isAttacking = false;
            agent.speed = 0;
        }
        
        // Actualizar el animator con los estados actuales
        UpdateAnimator();
        
        // Reproducir sonidos aleatorios
        if (Time.time > lastSoundTime + minTimeBetweenSounds && enemySounds.Length > 0)
        {
            if (Random.Range(0f, 1f) < 0.1f) // 10% de probabilidad cada frame cuando está permitido
            {
                PlayRandomSound();
            }
        }
    }
    
    // Actualizar el animator con los estados actuales
    private void UpdateAnimator()
    {
        if (animator != null)
        {
            animator.SetBool("isChasing", isChasing);
            animator.SetBool("isAttacking", isAttacking);
            animator.SetBool("isIdle", !isChasing && !isAttacking);
            
            // Si tienes un parámetro para fleeing, puedes añadirlo aquí
            // animator.SetBool("isFleeing", isFleeing);
        }
    }
    
    // Método para atacar al jugador
    void Attack()
    {
        // Actualizar tiempo del último ataque
        lastAttackTime = Time.time;
        
        // Reproducir sonido de ataque
        if (attackSound != null && enemyAudioSource != null)
        {
            enemyAudioSource.clip = attackSound;
            enemyAudioSource.Play();
        }
        
        // Infligir daño al jugador
        if (playerHealth != null)
        {
            playerHealth.TakeDamage(damageAmount);
        }
    }
    
    // Método público para iniciar la persecución desde otro script
    public void StartChasing()
    {
        if (!isFleeing)
        {
            isChasing = true;
            animator.SetBool("isChasing", true);
            agent.speed = chaseSpeed;
        }
    }
    
    // Método público para huir del jugador cuando recibe daño
    public void FleeFromPlayer(float duration)
    {
        // Si quieres simplemente eliminar la funcionalidad de huida,
        // puedes dejar este método vacío o comentarlo completamente
        
        // Cancelar la huida anterior si está en curso
        if (fleeCoroutine != null)
            StopCoroutine(fleeCoroutine);
            
        // Iniciar nueva huida
        fleeCoroutine = StartCoroutine(FleeCoroutine(duration));
    }
    
    private IEnumerator FleeCoroutine(float duration)
    {
        // Marcar que está huyendo
        isFleeing = true;
        
        // Guardar estados anteriores
        bool wasChasing = isChasing;
        bool wasAttacking = isAttacking;
        
        // Desactivar persecución y ataque
        isChasing = false;
        isAttacking = false;
        
        // Actualizar animator
        UpdateAnimator();
        
        // Calcular dirección opuesta al jugador
        Vector3 fleeDirection = transform.position - player.position;
        fleeDirection.y = 0; // Mantener en el mismo plano
        fleeDirection.Normalize();
        
        // Calcular destino de huida
        Vector3 fleeDestination = transform.position + fleeDirection * 15f;
        
        // Buscar punto válido en NavMesh
        NavMeshHit hit;
        if (NavMesh.SamplePosition(fleeDestination, out hit, 15f, NavMesh.AllAreas))
        {
            fleeDestination = hit.position;
        }
        
        // Configurar el NavMeshAgent
        agent.speed = fleeSpeed;
        agent.SetDestination(fleeDestination);
        
        // Esperar el tiempo especificado
        yield return new WaitForSeconds(duration);
        
        // Restaurar estado normal
        isFleeing = false;
        isChasing = wasChasing;
        isAttacking = wasAttacking;
        
        // Actualizar animator
        UpdateAnimator();
        
        fleeCoroutine = null;
    }
    
    void PlayRandomSound()
    {
        if (enemyAudioSource != null && enemySounds.Length > 0 && !enemyAudioSource.isPlaying)
        {
            int randomIndex = Random.Range(0, enemySounds.Length);
            enemyAudioSource.clip = enemySounds[randomIndex];
            enemyAudioSource.Play();
            lastSoundTime = Time.time;
        }
    }
    
    // Para debug: visualizar el rango de detección
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, chaseRange);
        
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, attackRange);
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