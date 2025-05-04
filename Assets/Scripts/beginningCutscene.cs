using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class beginningCutscene : MonoBehaviour
{
    public GameObject cutsceneCam, player;
    public float cutsceneTime;
    
    // Añade estos elementos para la leyenda y la voz
    [Header("Narración Inicial")]
    public GameObject legendPanel;           // Panel que contiene el texto
    public TextMeshProUGUI legendText;       // Componente de texto
    [TextArea(3, 10)]
    public string narrativeText;             // El texto que se mostrará
    public AudioSource voiceAudioSource;     // Componente para reproducir el audio
    public AudioClip voiceClip;              // El clip de voz a reproducir
    public float textDisplayTime = 5f;       // Cuánto tiempo se mostrará el texto
    
    void Start()
    {
        // Asegúrate de que la leyenda comience oculta
        if (legendPanel != null)
            legendPanel.SetActive(false);
            
        StartCoroutine(cutscene());
    }
    
    IEnumerator cutscene()
    {
        // Espera el tiempo de la cutscene inicial
        yield return new WaitForSeconds(cutsceneTime);
        
        // Activa el jugador y desactiva la cámara de cutscene
        player.SetActive(true);
        cutsceneCam.SetActive(false);
        
        // Espera un momento antes de mostrar la leyenda (opcional)
        yield return new WaitForSeconds(0.5f);
        
        // Muestra la leyenda y reproduce la voz
        StartCoroutine(ShowNarration());
    }
    
    IEnumerator ShowNarration()
    {
        // Configura y muestra el panel de texto
        if (legendPanel != null && legendText != null)
        {
            legendText.text = narrativeText;
            legendPanel.SetActive(true);
            
            // Reproduce el audio de voz si está disponible
            if (voiceAudioSource != null && voiceClip != null)
            {
                voiceAudioSource.clip = voiceClip;
                voiceAudioSource.Play();
            }
            
            // Espera el tiempo configurado o la duración del clip de audio (el que sea mayor)
            float waitTime = textDisplayTime;
            if (voiceClip != null && voiceClip.length > waitTime)
                waitTime = voiceClip.length;
                
            yield return new WaitForSeconds(waitTime);
            
            // Oculta el panel de texto
            legendPanel.SetActive(false);
        }
    }
}