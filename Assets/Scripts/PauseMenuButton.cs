using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class PauseMenuButtonSprite : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private Image backgroundSprite;
    
    private void Start()
    {
        // Asegúrate de que el fondo rojo comience invisible
        if (backgroundSprite != null)
            backgroundSprite.enabled = false;
    }
    
    public void OnPointerEnter(PointerEventData eventData)
    {
        // Muestra el fondo rojo cuando el mouse está encima
        if (backgroundSprite != null)
            backgroundSprite.enabled = true;
    }
    
    public void OnPointerExit(PointerEventData eventData)
    {
        // Oculta el fondo rojo cuando el mouse sale
        if (backgroundSprite != null)
            backgroundSprite.enabled = false;
    }
}