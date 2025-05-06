using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class DeathScreenUI : MonoBehaviour
{
    [Header("UI Elements")]
    public Button restartButton;
    public Button mainMenuButton;
    
    [Header("Animation")]
    public float initialDelay = 1.5f;  // Tiempo antes de mostrar los botones
    public CanvasGroup buttonsCanvasGroup;  // Grupo de Canvas para fade-in de botones
    public float fadeInDuration = 1.0f;  // Duración del fade-in
    
    [Header("Audio")]
    public AudioSource buttonSound;  // Sonido al hacer clic
    
    // Efectos visuales opcionales
    public Image backgroundImage;
    public float backgroundFadeSpeed = 0.5f;
    
    void Start()
    {
        // Asegurar que el cursor esté visible
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        
        // Ocultar los botones inicialmente
        if (buttonsCanvasGroup != null)
        {
            buttonsCanvasGroup.alpha = 0f;
            buttonsCanvasGroup.interactable = false;
        }
        
        // Configurar los eventos de los botones
        if (restartButton != null)
        {
            restartButton.onClick.AddListener(RestartGame);
        }
        
        if (mainMenuButton != null)
        {
            mainMenuButton.onClick.AddListener(ReturnToMainMenu);
        }
        
        // Iniciar la animación de los botones
        StartCoroutine(ShowButtonsAfterDelay());
        
        // Iniciar animación de fondo si existe
        if (backgroundImage != null)
        {
            StartCoroutine(AnimateBackground());
        }
    }
    
    // Corrutina para mostrar los botones después de un retraso
    IEnumerator ShowButtonsAfterDelay()
    {
        yield return new WaitForSeconds(initialDelay);
        
        // Animar la aparición de los botones si tenemos el CanvasGroup
        if (buttonsCanvasGroup != null)
        {
            float elapsed = 0f;
            while (elapsed < fadeInDuration)
            {
                buttonsCanvasGroup.alpha = Mathf.Lerp(0f, 1f, elapsed / fadeInDuration);
                elapsed += Time.deltaTime;
                yield return null;
            }
            buttonsCanvasGroup.alpha = 1f;  // Asegurar que llegue a 1
            buttonsCanvasGroup.interactable = true;  // Habilitar interacción
        }
    }
    
    // Animación para el fondo (pulsación sutil)
    IEnumerator AnimateBackground()
    {
        // Guardar color original
        Color originalColor = backgroundImage.color;
        Color targetColor = new Color(originalColor.r * 0.8f, originalColor.g * 0.8f, originalColor.b * 0.8f, originalColor.a);
        
        while (true)
        {
            // Pulsar entre el color original y el color más oscuro
            float t = (Mathf.Sin(Time.time * backgroundFadeSpeed) + 1) * 0.5f;
            backgroundImage.color = Color.Lerp(originalColor, targetColor, t);
            yield return null;
        }
    }
    
    // Método para reiniciar el juego (carga el nivel 1)
    public void RestartGame()
    {
        // Reproducir sonido si está disponible
        if (buttonSound != null)
        {
            buttonSound.Play();
        }
        
        // Guardar que estamos en el nivel 1
        PlayerPrefs.SetInt("level", 1);
        PlayerPrefs.Save();
        
        Debug.Log("Reiniciando juego - Nivel guardado: 1");
        
        // Cargar el nivel 1
        SceneManager.LoadScene("level1");
    }
    
    // Método para volver al menú principal
    public void ReturnToMainMenu()
    {
        // Reproducir sonido si está disponible
        if (buttonSound != null)
        {
            buttonSound.Play();
        }
        
        Debug.Log("Volviendo al menú principal");
        
        // Cargar la escena del menú
        SceneManager.LoadScene("menu");
    }
}