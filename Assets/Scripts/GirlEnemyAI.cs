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
    
    // Estados de comportamiento
    public bool isChasing = false;
    public bool isAttacking = false;
    
    // Velocidades configurables
    public float chaseSpeed = 5.0f;   // Velocidad al perseguir
    
    // Distancias
    public float chaseRange = 15f;
    public float attackRange = 2f;
    public float minTimeBetweenSounds = 8f;
    
    private float lastSoundTime = 0f;
    
    void Start()
    {
        // Asegurar que tenemos las referencias necesarias
        if (agent == null)
            agent = GetComponent<NavMeshAgent>();
            
        if (animator == null)
            animator = GetComponent<Animator>();
            
        if (enemyAudioSource == null)
            enemyAudioSource = GetComponent<AudioSource>();
    }
    
    void Update()
    {
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
        
        // Actualizar el animator
        animator.SetBool("isChasing", isChasing);
        animator.SetBool("isAttacking", isAttacking);
        
        // Reproducir sonidos aleatorios
        if (Time.time > lastSoundTime + minTimeBetweenSounds && enemySounds.Length > 0)
        {
            if (Random.Range(0f, 1f) < 0.1f) // 10% de probabilidad cada frame cuando está permitido
            {
                PlayRandomSound();
            }
        }
    }
    
    // Método público para iniciar la persecución desde otro script
    public void StartChasing()
    {
        isChasing = true;
        animator.SetBool("isChasing", true);
        agent.speed = chaseSpeed;
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
}