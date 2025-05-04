using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class NarrationTrigger : MonoBehaviour
{
    [Header("Configuración de Narración")]
    [TextArea(3, 10)]
    public string narrativeText;             // El texto que se mostrará
    public AudioClip voiceClip;              // El clip de voz a reproducir
    public float textDisplayTime = 5f;       // Cuánto tiempo se mostrará el texto
    
    [Header("Configuración de Trigger")]
    public bool triggerOnce = true;          // Si solo debe activarse una vez
    public float delayBeforeNarration = 0f;  // Espera antes de mostrar la narración
    public string requiredTag = "Player";    // Qué tag debe tener el objeto que activa el trigger
    
    [Header("Efectos Especiales (Opcional)")]
    public GameObject specialEffectPrefab;   // Efectos especiales opcionales al activar
    public float effectDuration = 3f;        // Duración de los efectos especiales
    
    private bool hasTriggered = false;
    
    // Referencia al administrador de narraciones
    private NarrationManager narrationManager;
    
    void Start()
    {
        // Buscar el administrador de narraciones en la escena
        narrationManager = FindObjectOfType<NarrationManager>();
        
        if (narrationManager == null)
        {
            Debug.LogError("¡No se encontró NarrationManager en la escena! Asegúrate de añadirlo.");
        }
    }
    
    private void OnTriggerEnter(Collider other)
    {
        // Verificar si el objeto que entró tiene el tag requerido
        if (other.CompareTag(requiredTag))
        {
            // Si está configurado para activarse solo una vez y ya se activó, no hacer nada
            if (triggerOnce && hasTriggered)
                return;
                
            hasTriggered = true;
            
            // Activar la narración con un retraso opcional
            StartCoroutine(TriggerNarrationWithDelay());
        }
    }
    
    IEnumerator TriggerNarrationWithDelay()
    {
        // Esperar el retraso configurado
        if (delayBeforeNarration > 0)
            yield return new WaitForSeconds(delayBeforeNarration);
            
        // Activar efectos especiales si están configurados
        if (specialEffectPrefab != null)
        {
            GameObject effect = Instantiate(specialEffectPrefab, transform.position, Quaternion.identity);
            Destroy(effect, effectDuration);
        }
        
        // Activar la narración
        if (narrationManager != null)
        {
            narrationManager.ShowNarration(narrativeText, voiceClip, textDisplayTime);
        }
    }
}