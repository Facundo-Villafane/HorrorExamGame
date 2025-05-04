using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GirlEnemyJumpscare : MonoBehaviour
{
    // Referencias a objetos
    public Animator enemyAnimator;
    public GameObject player;
    public Camera jumpscareCamera;
    
    // Configuración de jumpscare
    public float jumpscareTime = 3f;
    public string gameOverSceneName = "death";
    public AudioClip jumpscareSound;
    public AudioSource audioSource;
    
    // Efectos visuales opcionales
    public Material jumpscareMaterial;
    public GameObject jumpscareFX;
    
    private bool hasTriggered = false;
    
    void OnTriggerEnter(Collider other)
    {
        // Verificar si el jugador entró en contacto con el enemigo y que no se haya activado antes
        if (other.CompareTag("Player") && !hasTriggered)
        {
            hasTriggered = true;
            
            // Desactivar el control del jugador
            if (player != null)
            {
                // Desactivar scripts del jugador
                var playerController = player.GetComponent<SC_FPSController>();
                if (playerController != null)
                    playerController.enabled = false;
                    
                // Opcionalmente, desactivar colliders del jugador
                Collider playerCollider = player.GetComponent<Collider>();
                if (playerCollider != null)
                    playerCollider.enabled = false;
            }
            
            // Activar la cámara de jumpscare si existe
            if (jumpscareCamera != null)
            {
                jumpscareCamera.gameObject.SetActive(true);
                
                // Desactivar la cámara del jugador
                Camera playerCamera = other.GetComponentInChildren<Camera>();
                if (playerCamera != null)
                    playerCamera.enabled = false;
            }
            
            // Activar animación de jumpscare
            if (enemyAnimator != null)
            {
                enemyAnimator.ResetTrigger("idle");
                enemyAnimator.ResetTrigger("walk");
                enemyAnimator.ResetTrigger("run");
                enemyAnimator.SetTrigger("jumpscare");
            }
            
            // Reproducir sonido de jumpscare
            if (audioSource != null && jumpscareSound != null)
            {
                audioSource.clip = jumpscareSound;
                audioSource.Play();
            }
            
            // Activar efectos visuales
            if (jumpscareFX != null)
                jumpscareFX.SetActive(true);
                
            // Iniciar secuencia de jumpscare
            StartCoroutine(JumpscareSequence());
        }
    }
    
    IEnumerator JumpscareSequence()
    {
        // Esperar el tiempo configurado
        yield return new WaitForSeconds(jumpscareTime);
        
        // Cargar la escena de game over
        SceneManager.LoadScene(gameOverSceneName);
    }
}