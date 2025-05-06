using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class menu : MonoBehaviour
{
    [Header("Panel References")]
    public GameObject loadingScreen;
    public GameObject menuPanel;
    public GameObject settingsPanel;
    public GameObject controlsPanel; // Panel de controles

    [Header("Button References")]
    public Button continueButton;
    
    [Header("Scene References")]
    public string levelOneScene;  // Nombre de la escena del primer nivel
    public string levelTwoScene;  // Nombre de la escena del segundo nivel
    
    [Header("Canvas Sorting")]
    public Canvas menuCanvas; // Canvas principal del menú

    void Start()
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        
        // Asegurarse de que solo el panel del menú principal esté activo
        ShowMainMenu();
        
        // Configurar el sorting order del canvas del menú
        if (menuCanvas != null)
            menuCanvas.sortingOrder = 10;
        
        // Verificar si hay datos guardados para continuar y actualizar el botón
        UpdateContinueButton();
        
        // Para propósitos de depuración, muestra el nivel guardado
        Debug.Log("Nivel guardado al iniciar el menú: " + PlayerPrefs.GetInt("level", 0));
    }
    
    // Método para actualizar el estado del botón Continue
    private void UpdateContinueButton()
    {
        if (continueButton != null)
        {
            // Comprobar si hay un nivel guardado
            int savedLevel = PlayerPrefs.GetInt("level", 0);
            
            // Activar el botón solo si hay un nivel guardado
            continueButton.interactable = (savedLevel > 0);
            
            // Cambiar el color si está desactivado
            ColorBlock colors = continueButton.colors;
            if (!continueButton.interactable)
            {
                colors.disabledColor = new Color(0.3f, 0.3f, 0.3f, 0.5f);
            }
            else
            {
                // Asegurarse de que el botón se vea normal cuando está activo
                colors.normalColor = new Color(1f, 1f, 1f, 1f);
            }
            continueButton.colors = colors;
        }
    }
    
    // Método para iniciar un nuevo juego
    public void PlayNewGame()
    {
        // Activar pantalla de carga
        if (loadingScreen != null)
            loadingScreen.SetActive(true);
            
        // Guardar que estamos en el nivel 1
        PlayerPrefs.SetInt("level", 1);
        PlayerPrefs.Save();
        
        Debug.Log("Iniciando nuevo juego - Nivel guardado: " + PlayerPrefs.GetInt("level", 0));
        
        // Cargar la primera escena
        SceneManager.LoadScene(levelOneScene);
    }
    
    // Método para continuar juego guardado
    public void ContinueGame()
    {
        // Obtener el nivel guardado
        int savedLevel = PlayerPrefs.GetInt("level", 0);
        
        Debug.Log("Intentando continuar - Nivel guardado: " + savedLevel);
        
        if (savedLevel > 0)
        {
            // Mostrar pantalla de carga
            if (loadingScreen != null)
                loadingScreen.SetActive(true);
            
            // Cargar la escena correspondiente según el nivel guardado
            if (savedLevel == 3) // Victoria
            {
                // Para la pantalla de victoria, podrías elegir mostrar la pantalla
                // de victoria de nuevo o iniciar un nuevo juego
                Debug.Log("Juego completado - Iniciando nuevo juego");
                PlayerPrefs.SetInt("level", 1);
                PlayerPrefs.Save();
                SceneManager.LoadScene(levelOneScene);
            }
            else if (savedLevel == 2) // Nivel 2
            {
                Debug.Log("Cargando nivel 2: " + levelTwoScene);
                SceneManager.LoadScene(levelTwoScene);
            }
            else // Nivel 1 o cualquier otro caso
            {
                Debug.Log("Cargando nivel 1: " + levelOneScene);
                SceneManager.LoadScene(levelOneScene);
            }
        }
        else
        {
            Debug.LogWarning("No hay nivel guardado para continuar");
        }
    }
    
    // Método para abrir el menú de configuración
    public void OpenSettingsMenu()
    {
        HideAllPanels();
        if (settingsPanel != null)
            settingsPanel.SetActive(true);
    }
    
    // Método para abrir el panel de controles
    public void OpenControlsMenu()
    {
        HideAllPanels();
        if (controlsPanel != null)
            controlsPanel.SetActive(true);
    }
    
    // Método para volver al menú principal
    public void BackToMainMenu()
    {
        ShowMainMenu();
    }
    
    // Método para salir del juego
    public void QuitGame()
    {
        #if UNITY_EDITOR
        // Código para salir del editor de Unity
        EditorApplication.isPlaying = false;
        Debug.Log("Saliendo del modo play en el Editor...");
        #else
        // Código para salir del juego en una build
        Application.Quit();
        Debug.Log("Saliendo del juego...");
        #endif
    }
    
    // Método auxiliar para ocultar todos los paneles
    private void HideAllPanels()
    {
        if (menuPanel != null)
            menuPanel.SetActive(false);
            
        if (settingsPanel != null)
            settingsPanel.SetActive(false);
            
        if (controlsPanel != null)
            controlsPanel.SetActive(false);
    }
    
    // Método auxiliar para mostrar solo el menú principal
    private void ShowMainMenu()
    {
        HideAllPanels();
        if (menuPanel != null)
            menuPanel.SetActive(true);
    }
}