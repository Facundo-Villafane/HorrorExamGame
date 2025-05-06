using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AI;
using TMPro;

public class EnemyHealth : MonoBehaviour
{
    public int maxHealth = 100;
    public int currentHealth;
    
    [Header("UI de Salud")]
    public GameObject healthBarPanel;
    public Image healthFillImage;
    public TMP_Text enemyNameText;
    private bool healthBarActivated = false;
    
    [Header("Configuración de Recuperación")]
    public bool canRecover = true;
    public float recoveryDelay = 7f;
    public float recoveryRate = 10f;
    private Coroutine recoveryCoroutine;
    
    // Efectos cuando recibe daño
    public AudioSource hurtSound;
    public GameObject hurtEffect;
    
    // Referencias a componentes
    private MonoBehaviour enemyAIScript;
    private NavMeshAgent navMeshAgent;
    
    void Start()
    {
        currentHealth = maxHealth;
        
        // Buscar scripts de IA
        enemyAIScript = GetComponent<GirlEnemyAI>();
        if (enemyAIScript == null)
            enemyAIScript = GetComponent<EnemyPatrol>();
            
        // Obtener NavMeshAgent para controlar el movimiento
        navMeshAgent = GetComponent<UnityEngine.AI.NavMeshAgent>();
        
        // Desactivar la barra de salud al inicio
        if (healthBarPanel != null)
            healthBarPanel.SetActive(false);
            
        // Configurar el nombre del enemigo
        if (enemyNameText != null)
        {
            if (GetComponent<GirlEnemyAI>() != null)
                enemyNameText.text = "Falopatar Lv1";
            else if (GetComponent<EnemyPatrol>() != null)
                enemyNameText.text = "Falopatar Lv2";
            else
                enemyNameText.text = "Falopatar Lv99";
        }
    }
    
    public void TakeDamage(int damage)
    {
        // Cancelar cualquier recuperación en curso
        StopRecoveryProcess();
        
        currentHealth -= damage;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
        
        // Mostrar la barra de salud
        if (!healthBarActivated && healthBarPanel != null)
        {
            healthBarPanel.SetActive(true);
            healthBarActivated = true;
        }
        
        // Actualizar la barra de salud
        UpdateHealthBar();
        
        // Efectos de daño
        if (hurtSound != null)
            hurtSound.Play();
            
        if (hurtEffect != null)
            hurtEffect.SetActive(true);
        
        // Verificar si el enemigo está incapacitado
        if (currentHealth <= 0)
        {
            TemporarilyIncapacitate();
        }
        else
        {
            // Hacer que el enemigo huya
            FleeFromPlayer();
            
            // Iniciar recuperación después de un tiempo
            if (canRecover)
            {
                recoveryCoroutine = StartCoroutine(RecoverAfterDelay());
            }
        }
    }
    
    private void UpdateHealthBar()
    {
        if (healthFillImage != null)
        {
            healthFillImage.fillAmount = (float)currentHealth / maxHealth;
            
            // Cambiar el color según el nivel de salud
            if (currentHealth <= maxHealth * 0.25f)
                healthFillImage.color = new Color(1f, 0f, 0f); // Rojo
            else if (currentHealth <= maxHealth * 0.5f)
                healthFillImage.color = new Color(1f, 0.5f, 0f); // Naranja
            else
                healthFillImage.color = new Color(1f, 0.1f, 0.1f); // Rojo intenso
        }
    }
    
    // Método para hacer que el enemigo huya del jugador
    private void FleeFromPlayer()
    {
        // Implementaremos esto en las modificaciones a los scripts GirlEnemyAI y EnemyPatrol
        if (GetComponent<GirlEnemyAI>() != null)
        {
            GirlEnemyAI aiScript = (GirlEnemyAI)enemyAIScript;
            aiScript.FleeFromPlayer(3f); // Huir durante 3 segundos
        }
        else if (GetComponent<EnemyPatrol>() != null)
        {
            EnemyPatrol patrolScript = (EnemyPatrol)enemyAIScript;
            patrolScript.FleeFromPlayer(3f); // Huir durante 3 segundos
        }
    }
    
    // Método para incapacitar temporalmente al enemigo
    private void TemporarilyIncapacitate()
    {
        // Cambiar el texto
        if (enemyNameText != null)
        {
            enemyNameText.text = "ENEMIGO INCAPACITADO";
            enemyNameText.color = Color.yellow;
        }
        
        // Desactivar la IA
        if (enemyAIScript != null)
            enemyAIScript.enabled = false;
            
        // Desactivar el NavMeshAgent
        if (navMeshAgent != null)
            navMeshAgent.isStopped = true;
            
        // Desactivar colisiones
        Collider enemyCollider = GetComponent<Collider>();
        if (enemyCollider != null)
            enemyCollider.enabled = false;
        
        // Iniciar recuperación después de un tiempo más largo
        StartCoroutine(RecoverFromIncapacitation());
    }
    
    private IEnumerator RecoverFromIncapacitation()
    {
        yield return new WaitForSeconds(recoveryDelay * 3f);
        
        // Reactivar componentes
        if (enemyAIScript != null)
            enemyAIScript.enabled = true;
            
        if (navMeshAgent != null)
            navMeshAgent.isStopped = false;
            
        Collider enemyCollider = GetComponent<Collider>();
        if (enemyCollider != null)
            enemyCollider.enabled = true;
            
        // Dar algo de salud
        currentHealth = maxHealth / 4; // 25% de salud
        
        // Restaurar el texto
        if (enemyNameText != null)
        {
            if (GetComponent<GirlEnemyAI>() != null)
                enemyNameText.text = "FANTASMA";
            else if (GetComponent<EnemyPatrol>() != null)
                enemyNameText.text = "MONSTRUO";
            else
                enemyNameText.text = "ENEMIGO";
                
            enemyNameText.color = Color.white;
        }
        
        // Iniciar recuperación gradual
        recoveryCoroutine = StartCoroutine(RecoverHealth());
    }
    
    private IEnumerator RecoverAfterDelay()
    {
        yield return new WaitForSeconds(recoveryDelay);
        
        if (currentHealth > 0 && currentHealth < maxHealth)
        {
            recoveryCoroutine = StartCoroutine(RecoverHealth());
        }
    }
    
    private IEnumerator RecoverHealth()
    {
        while (currentHealth < maxHealth)
        {
            currentHealth += Mathf.CeilToInt(recoveryRate * Time.deltaTime);
            currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
            UpdateHealthBar();
            yield return null;
        }
    }
    
    private void StopRecoveryProcess()
    {
        if (recoveryCoroutine != null)
        {
            StopCoroutine(recoveryCoroutine);
            recoveryCoroutine = null;
        }
    }
    
    private void OnDisable()
    {
        StopRecoveryProcess();
    }
}