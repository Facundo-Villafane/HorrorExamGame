using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager Instance { get; private set; }
    
    [SerializeField] private GameObject dialoguePanel;
    [SerializeField] private TextMeshProUGUI dialogueText;
    [SerializeField] private AudioSource audioSource;
    
    private Coroutine hideCoroutine;
    
    private void Awake()
    {
        // Implementación del patrón Singleton
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
        
        // Asegurarse de que el panel está oculto al inicio
        if (dialoguePanel != null)
            dialoguePanel.SetActive(false);
    }
    
    public void ShowDialogue(string text, AudioClip voiceClip, float duration)
    {
        // Cancelar cualquier ocultamiento pendiente
        if (hideCoroutine != null)
            StopCoroutine(hideCoroutine);
            
        // Mostrar el panel y configurar el texto
        dialoguePanel.SetActive(true);
        dialogueText.text = text;
        
        // Reproducir audio si está disponible
        if (voiceClip != null && audioSource != null)
        {
            audioSource.clip = voiceClip;
            audioSource.Play();
        }
        
        // Configurar el ocultamiento automático
        hideCoroutine = StartCoroutine(HideAfterDelay(duration));
    }
    
    public void HideDialogue()
    {
        // Cancelar cualquier ocultamiento pendiente
        if (hideCoroutine != null)
            StopCoroutine(hideCoroutine);
            
        // Ocultar el panel y detener el audio
        dialoguePanel.SetActive(false);
        if (audioSource != null && audioSource.isPlaying)
            audioSource.Stop();
            
        hideCoroutine = null;
    }
    
    private IEnumerator HideAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        HideDialogue();
        hideCoroutine = null;
    }
}