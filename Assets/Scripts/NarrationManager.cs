using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class NarrationManager : MonoBehaviour
{
    [Header("Referencias UI")]
    public GameObject narrativePanel;    // Panel que contendrá el texto
    public TextMeshProUGUI narrativeText; // Componente de texto
    
    [Header("Referencias de Audio")]
    public AudioSource voiceAudioSource;  // Fuente de audio para reproducir voces
    
    private Coroutine currentNarration;   // Para llevar un seguimiento de narraciones activas
    
    void Start()
    {
        // Asegurarse de que el panel está oculto al iniciar
        if (narrativePanel != null)
            narrativePanel.SetActive(false);
    }
    
    public void ShowNarration(string text, AudioClip voiceClip, float displayTime)
    {
        // Si hay una narración en curso, detenerla
        if (currentNarration != null)
            StopCoroutine(currentNarration);
            
        // Iniciar la nueva narración
        currentNarration = StartCoroutine(ShowNarrationCoroutine(text, voiceClip, displayTime));
    }
    
    private IEnumerator ShowNarrationCoroutine(string text, AudioClip voiceClip, float displayTime)
    {
        // Configurar y mostrar el panel de texto
        if (narrativePanel != null && narrativeText != null)
        {
            narrativeText.text = text;
            narrativePanel.SetActive(true);
            
            // Reproducir el audio de voz si está disponible
            if (voiceAudioSource != null && voiceClip != null)
            {
                voiceAudioSource.clip = voiceClip;
                voiceAudioSource.Play();
            }
            
            // Determinar el tiempo de espera (usar la duración del clip si es más larga)
            float waitTime = displayTime;
            if (voiceClip != null && voiceClip.length > waitTime)
                waitTime = voiceClip.length;
                
            yield return new WaitForSeconds(waitTime);
            
            // Ocultar el panel de texto
            narrativePanel.SetActive(false);
        }
        
        currentNarration = null;
    }
}