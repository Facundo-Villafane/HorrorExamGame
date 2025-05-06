using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class MenuButtonEffect : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [Header("Hover Effect")]
    public Image backgroundImage; // Imagen que aparecerá al hacer hover
    public AudioSource hoverSound; // Sonido opcional
    
    [Header("Debug")]
    public bool showDebugMessages = true;
    
    void Start()
    {
        // Verificar que tenemos una imagen de fondo
        if (backgroundImage == null)
        {
            // Intentar buscar una imagen entre los hijos
            Transform bgTransform = transform.Find("Background");
            if (bgTransform != null)
            {
                backgroundImage = bgTransform.GetComponent<Image>();
            }
            
            if (backgroundImage == null && showDebugMessages)
            {
                Debug.LogError("Error: No se encontró una imagen de fondo para el botón: " + gameObject.name);
            }
        }
        
        // Configurar la imagen de fondo
        if (backgroundImage != null)
        {
            // Asegurarnos de que el GameObject esté activo
            backgroundImage.gameObject.SetActive(true);
            
            // Ocultar la imagen desactivando su renderizado
            backgroundImage.enabled = false;
            
            // Configuraciones adicionales
            backgroundImage.raycastTarget = false; // Para evitar problemas con eventos
            
            if (showDebugMessages)
            {
                Debug.Log("MenuButtonEffect inicializado correctamente para: " + gameObject.name);
            }
        }
    }
    
    public void OnPointerEnter(PointerEventData eventData)
    {
        // Mostrar la imagen de fondo
        if (backgroundImage != null)
        {
            backgroundImage.enabled = true;
            
            if (showDebugMessages)
            {
                Debug.Log("Mouse sobre botón: " + gameObject.name);
            }
        }
        
        // Reproducir sonido si existe
        if (hoverSound != null && !hoverSound.isPlaying)
        {
            hoverSound.Play();
        }
    }
    
    public void OnPointerExit(PointerEventData eventData)
    {
        // Ocultar la imagen de fondo
        if (backgroundImage != null)
        {
            backgroundImage.enabled = false;
        }
    }
}