using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class VictoryScreenUI : MonoBehaviour
{
    [Header("UI Elements")]
    public Button mainMenuButton;
    public TMP_Text victoryText;
    public TMP_Text congratulationsText;
    
    [Header("Animations")]
    public float initialDelay = 1.5f;
    public CanvasGroup mainCanvasGroup;
    public float fadeInDuration = 1.0f;
    public float textAnimationDelay = 0.5f;
    
    [Header("Effects")]
    public AudioSource victoryMusic;
    public AudioSource buttonSound;
    public GameObject victoryParticleSystem;
    
    // Variables para animación de texto
    private float textAnimTimer = 0f;
    private Coroutine textAnimationCoroutine;
    
    void Start()
    {
        // Cancelar cualquier carga de escena programada anteriormente
        CancelInvoke();
        StopAllCoroutines();

        // Asegurar que el cursor esté visible
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        
        // Guardar nivel completado (3 representa juego completado/victoria)
        PlayerPrefs.SetInt("level", 3);
        PlayerPrefs.Save();
        
        // Ocultar elementos inicialmente
        if (mainCanvasGroup != null)
        {
            mainCanvasGroup.alpha = 0f;
            mainCanvasGroup.interactable = false;
        }
        
        // Configurar el evento del botón
        if (mainMenuButton != null)
        {
            mainMenuButton.onClick.AddListener(ReturnToMainMenu);
        }
        
        // Iniciar efectos de victoria
        StartCoroutine(PlayVictorySequence());
    }
    
    IEnumerator PlayVictorySequence()
    {
        // Reproducir música si existe
        if (victoryMusic != null && !victoryMusic.isPlaying)
        {
            victoryMusic.Play();
        }
        
        // Iniciar sistema de partículas si existe
        if (victoryParticleSystem != null)
        {
            victoryParticleSystem.SetActive(true);
        }
        
        // Esperar el retraso inicial
        yield return new WaitForSeconds(initialDelay);
        
        // Animar la aparición de los elementos UI
        if (mainCanvasGroup != null)
        {
            float elapsed = 0f;
            while (elapsed < fadeInDuration)
            {
                mainCanvasGroup.alpha = Mathf.Lerp(0f, 1f, elapsed / fadeInDuration);
                elapsed += Time.deltaTime;
                yield return null;
            }
            mainCanvasGroup.alpha = 1f;
            mainCanvasGroup.interactable = true;
        }
        
        // Animar texto si tenemos textos configurados
        if (victoryText != null || congratulationsText != null)
        {
            StartTextAnimation();
        }
    }
    
    // Método para animar los textos
    void StartTextAnimation()
    {
        if (textAnimationCoroutine != null)
        {
            StopCoroutine(textAnimationCoroutine);
        }
        
        textAnimationCoroutine = StartCoroutine(AnimateText());
    }
    
    IEnumerator AnimateText()
    {
        // Esperar un breve momento
        yield return new WaitForSeconds(textAnimationDelay);
        
        // Efectos de escala para el texto de victoria
        if (victoryText != null)
        {
            victoryText.transform.localScale = Vector3.zero;
            float elapsed = 0f;
            float duration = 0.5f;
            
            while (elapsed < duration)
            {
                float scale = Mathf.Lerp(0f, 1.2f, elapsed / duration);
                victoryText.transform.localScale = new Vector3(scale, scale, scale);
                elapsed += Time.deltaTime;
                yield return null;
            }
            
            // Rebote hacia atrás
            elapsed = 0f;
            duration = 0.2f;
            
            while (elapsed < duration)
            {
                float scale = Mathf.Lerp(1.2f, 1f, elapsed / duration);
                victoryText.transform.localScale = new Vector3(scale, scale, scale);
                elapsed += Time.deltaTime;
                yield return null;
            }
            
            // Establecer escala final
            victoryText.transform.localScale = Vector3.one;
        }
        
        // Después animar el texto de felicitaciones
        if (congratulationsText != null)
        {
            congratulationsText.alpha = 0f;
            float elapsed = 0f;
            float duration = 1f;
            
            while (elapsed < duration)
            {
                congratulationsText.alpha = Mathf.Lerp(0f, 1f, elapsed / duration);
                elapsed += Time.deltaTime;
                yield return null;
            }
            
            // Iniciar animación continua de color
            StartCoroutine(PulseTextColor());
        }
    }
    
    // Hacer que el texto de felicitaciones cambie de color con una pulsación suave
    IEnumerator PulseTextColor()
    {
        if (congratulationsText == null)
            yield break;
            
        Color originalColor = congratulationsText.color;
        Color targetColor = new Color(1f, 0.8f, 0f); // Dorado
        
        while (true)
        {
            float t = (Mathf.Sin(Time.time * 2f) + 1) * 0.5f;
            congratulationsText.color = Color.Lerp(originalColor, targetColor, t);
            yield return null;
        }
    }
    
    // Método para volver al menú principal
    public void ReturnToMainMenu()
    {
        // Reproducir sonido si está disponible
        if (buttonSound != null)
        {
            buttonSound.Play();
        }
        
        Debug.Log("Volviendo al menú principal desde la pantalla de victoria");
        
        // Cargar la escena del menú
        SceneManager.LoadScene("menu");
    }
    
    void OnDestroy()
    {
        // Detener corrutinas cuando se destruya el objeto
        if (textAnimationCoroutine != null)
        {
            StopCoroutine(textAnimationCoroutine);
        }
    }
}