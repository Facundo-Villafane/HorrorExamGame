using UnityEngine;
using System.Collections;

public class InteractableObject : MonoBehaviour
{
    [SerializeField] private GameObject interactionPrompt;
    [TextArea(3, 10)]
    [SerializeField] private string dialogueText;
    [SerializeField] private AudioClip voiceClip;
    [SerializeField] private float dialogueDuration = 5.0f;
    
    private bool canInteract = false;
    private bool isInDialogue = false;
    
    private void OnTriggerEnter(Collider other)
    {
        if ((other.CompareTag("MainCamera") || other.CompareTag("Player")) && !isInDialogue)
        {
            interactionPrompt.SetActive(true);
            canInteract = true;
        }
    }
    
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("MainCamera") || other.CompareTag("Player"))
        {
            interactionPrompt.SetActive(false);
            canInteract = false;
            
            // Asegurarse de que si el jugador se aleja durante el diálogo,
            // el diálogo se cierre correctamente
            if (isInDialogue)
            {
                DialogueManager.Instance.HideDialogue();
                isInDialogue = false;
            }
        }
    }
    
    private void Update()
    {
        if (canInteract && Input.GetKeyDown(KeyCode.E) && !isInDialogue)
        {
            isInDialogue = true;
            DialogueManager.Instance.ShowDialogue(dialogueText, voiceClip, dialogueDuration);
            interactionPrompt.SetActive(false);
            StartCoroutine(ResetAfterDialogue());
        }
    }
    
    private IEnumerator ResetAfterDialogue()
    {
        yield return new WaitForSeconds(dialogueDuration);
        
        // Ocultar el diálogo explícitamente
        DialogueManager.Instance.HideDialogue();
        isInDialogue = false;
        
        // Si el jugador sigue en el área de trigger, volver a mostrar el prompt
        if (gameObject.activeSelf && canInteract)
        {
            interactionPrompt.SetActive(true);
        }
    }
    
    // Método para forzar la limpieza cuando el objeto se desactiva
    private void OnDisable()
    {
        if (isInDialogue)
        {
            DialogueManager.Instance.HideDialogue();
            isInDialogue = false;
        }
    }
}