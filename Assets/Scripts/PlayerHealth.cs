using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PlayerHealth : MonoBehaviour
{
    // Configuración de salud
    public int maxLives = 3;
    public int currentLives;
    
    // UI para mostrar las vidas
    public GameObject[] livesUI; // Arreglo de iconos de vida
    public Image damageFlash; // Panel rojo que parpadea cuando recibes daño
    
    // Efectos de daño
    public AudioSource damageSound;
    public float damageFlashDuration = 0.2f;
    
    // Tiempo de invulnerabilidad después de recibir daño
    public float invulnerabilityTime = 1.5f;
    private bool isInvulnerable = false;
    
    // Referencias a otros componentes
    private SC_FPSController playerController;
    
    // Nombre de la escena de Game Over
    public string gameOverSceneName = "death";
    
    void Start()
    {
        // Inicializar vidas al máximo
        currentLives = maxLives;
        
        // Obtener referencia al controlador
        playerController = GetComponent<SC_FPSController>();
        
        // Configurar la UI de vidas
        UpdateLivesUI();
        
        // Asegurarse de que el flash de daño está invisible
        if (damageFlash != null)
        {
            damageFlash.enabled = false;
        }
    }
    
    // Actualizar la UI de vidas
    void UpdateLivesUI()
    {
        if (livesUI == null || livesUI.Length == 0)
            return;
            
        // Activar/desactivar los iconos según las vidas actuales
        for (int i = 0; i < livesUI.Length; i++)
        {
            if (livesUI[i] != null)
            {
                livesUI[i].SetActive(i < currentLives);
            }
        }
    }
    
    // Método público para recibir daño desde otros scripts
    public void TakeDamage(int damageAmount)
    {
        // Si es invulnerable, ignorar el daño
        if (isInvulnerable)
            return;
            
        // Reducir las vidas
        currentLives -= damageAmount;
        
        // Iniciar invulnerabilidad temporal
        StartCoroutine(InvulnerabilityFrames());
        
        // Mostrar efectos de daño
        StartCoroutine(DamageFlash());
        
        // Reproducir sonido de daño
        if (damageSound != null)
        {
            damageSound.Play();
        }
        
        // Actualizar la UI
        UpdateLivesUI();
        
        // Comprobar si el jugador se quedó sin vidas
        if (currentLives <= 0)
        {
            Die();
        }
    }
    
    // Método para añadir una vida (power-up, etc.)
    public void AddLife()
    {
        if (currentLives < maxLives)
        {
            currentLives++;
            UpdateLivesUI();
        }
    }
    
    // Efecto visual de recibir daño
    IEnumerator DamageFlash()
    {
        if (damageFlash != null)
        {
            damageFlash.enabled = true;
            yield return new WaitForSeconds(damageFlashDuration);
            damageFlash.enabled = false;
        }
    }
    
    // Tiempo de invulnerabilidad después de recibir daño
    IEnumerator InvulnerabilityFrames()
    {
        isInvulnerable = true;
        
        // Opcional: hacer que el jugador parpadee durante la invulnerabilidad
        Renderer[] renderers = GetComponentsInChildren<Renderer>();
        
        for (float i = 0; i < invulnerabilityTime; i += 0.2f)
        {
            // Alternar visibilidad para crear efecto de parpadeo
            foreach (Renderer renderer in renderers)
            {
                renderer.enabled = !renderer.enabled;
            }
            yield return new WaitForSeconds(0.2f);
        }
        
        // Restaurar visibilidad
        foreach (Renderer renderer in renderers)
        {
            renderer.enabled = true;
        }
        
        isInvulnerable = false;
    }
    
    // Cuando el jugador pierde todas las vidas
    void Die()
    {
        // Desactivar el controlador del jugador
        if (playerController != null)
        {
            playerController.enabled = false;
        }
        
        // Iniciar la secuencia de muerte
        StartCoroutine(DeathSequence());
    }
    
    IEnumerator DeathSequence()
    {
        // Esperar un momento antes de cargar la escena de game over
        yield return new WaitForSeconds(1.5f);
        
        // Cargar la escena de game over
        SceneManager.LoadScene(gameOverSceneName);
    }
}