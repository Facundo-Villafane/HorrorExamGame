using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GirlEnemyTrigger : MonoBehaviour
{
    // El GameObject del enemigo que se activará
    public GameObject girlEnemy;
    
    // Opcionales: efectos adicionales cuando se activa el enemigo
    public GameObject bloodEffect;
    public AudioSource triggerSound;
    public Light flickeringLight;
    
    // Para controlar que el trigger solo se active una vez
    private bool hasTriggered = false;
    
    // Opcionales: bloques/obstáculos que se activan al aparecer el enemigo
    public GameObject[] blocksToEnable;
    public GameObject[] blocksToDisable;
    
    void OnTriggerEnter(Collider other)
    {
        // Verificar si el jugador entró en el trigger y que no se haya activado antes
        if (other.CompareTag("Player") && !hasTriggered)
        {
            hasTriggered = true;
            
            // Activar el enemigo
            if (girlEnemy != null)
            {
                girlEnemy.SetActive(true);
                
                // Si el enemigo tiene el componente GirlEnemyAI, activar modo persecución
                GirlEnemyAI enemyAI = girlEnemy.GetComponent<GirlEnemyAI>();
                if (enemyAI != null)
                {
                    enemyAI.StartChasing();
                }
            }
            
            // Activar efectos adicionales
            if (bloodEffect != null)
                bloodEffect.SetActive(true);
                
            if (triggerSound != null)
                triggerSound.Play();
                
            if (flickeringLight != null)
                StartCoroutine(FlickerLight());
                
            // Activar/desactivar bloques
            if (blocksToEnable != null)
            {
                foreach (GameObject block in blocksToEnable)
                {
                    if (block != null)
                        block.SetActive(true);
                }
            }
            
            if (blocksToDisable != null)
            {
                foreach (GameObject block in blocksToDisable)
                {
                    if (block != null)
                        block.SetActive(false);
                }
            }
            
            // Desactivar este collider para que no se vuelva a activar
            Collider triggerCollider = GetComponent<Collider>();
            if (triggerCollider != null)
                triggerCollider.enabled = false;
        }
    }
    
    // Efecto de luz parpadeante
    IEnumerator FlickerLight()
    {
        float originalIntensity = flickeringLight.intensity;
        float flickerDuration = 3f; // Duración del efecto
        float endTime = Time.time + flickerDuration;
        
        while (Time.time < endTime)
        {
            // Parpadeo aleatorio
            flickeringLight.intensity = originalIntensity * Random.Range(0.2f, 1.0f);
            yield return new WaitForSeconds(Random.Range(0.01f, 0.1f));
        }
        
        // Volver a la intensidad normal
        flickeringLight.intensity = originalIntensity;
    }
}