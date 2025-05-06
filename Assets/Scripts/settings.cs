using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class settings : MonoBehaviour
{
    public Dropdown graphicsDrop, resoDrop;
    public Slider volumeSlider;
    public Toggle chromaticToggle, vignetteToggle, grainToggle;
    public bool inGame;
    public GameObject chromaticCam, vignetteCam, grainCam;
    
    [Header("UI Navigation")]
    public GameObject checkMarkImage;     // La imagen de check que aparecerá
    public GameObject previousMenu;       // Referencia al menú anterior (pausa o principal)
    public GameObject settingsMenu;       // Referencia a este menú de configuración
    public float checkDisplayTime = 1.0f; // Duración de la marca de verificación
    private Coroutine saveCoroutine;      // Para controlar la corrutina

    void Start()
    {
        // Asegúrate de que la marca de verificación esté oculta al inicio
        if (checkMarkImage != null)
        {
            checkMarkImage.SetActive(false);
        }
        
        // Inicializa la configuración por defecto si es la primera vez
        if (PlayerPrefs.GetInt("settingsSaved", 0) == 0)
        {
            PlayerPrefs.SetInt("graphics", 0);
            PlayerPrefs.SetInt("resolution", 0);
            PlayerPrefs.SetFloat("mastervolume", 1.0f);
            PlayerPrefs.SetInt("chromatic", 0);
            PlayerPrefs.SetInt("vignette", 0);
            PlayerPrefs.SetInt("grain", 0);
        }
        
        // Carga la configuración guardada
        LoadSettings();
    }
    
    // Método para cargar las configuraciones guardadas
    private void LoadSettings()
    {
        //Graphics
        if (PlayerPrefs.GetInt("graphics", 2) == 2)
        {
            graphicsDrop.value = 0;
            QualitySettings.SetQualityLevel(0);
        }
        if (PlayerPrefs.GetInt("graphics", 1) == 1)
        {
            graphicsDrop.value = 1;
            QualitySettings.SetQualityLevel(1);
        }
        if (PlayerPrefs.GetInt("graphics", 0) == 0)
        {
            graphicsDrop.value = 2;
            QualitySettings.SetQualityLevel(2);
        }
        //Resolution
        if (PlayerPrefs.GetInt("resolution", 2) == 2)
        {
            resoDrop.value = 0;
            Screen.SetResolution(854, 480, true);
        }
        if (PlayerPrefs.GetInt("resolution", 1) == 1)
        {
            resoDrop.value = 1;
            Screen.SetResolution(1280, 720, true);
        }
        if (PlayerPrefs.GetInt("resolution", 0) == 0)
        {
            resoDrop.value = 2;
            Screen.SetResolution(1920, 1080, true);
        }
        //Volume
        volumeSlider.value = PlayerPrefs.GetFloat("mastervolume");
        AudioListener.volume = PlayerPrefs.GetFloat("mastervolume");
        //Chromatic Aberration
        if (PlayerPrefs.GetInt("chromatic", 1) == 1)
        {
            chromaticToggle.isOn = false;
            if (inGame == true && chromaticCam != null)
            {
                chromaticCam.SetActive(false);
            }
        }
        if (PlayerPrefs.GetInt("chromatic", 0) == 0)
        {
            chromaticToggle.isOn = true;
            if (inGame == true && chromaticCam != null)
            {
                chromaticCam.SetActive(true);
            }
        }
        //Vignette
        if (PlayerPrefs.GetInt("vignette", 1) == 1)
        {
            vignetteToggle.isOn = false;
            if (inGame == true && vignetteCam != null)
            {
                vignetteCam.SetActive(false);
            }
        }
        if (PlayerPrefs.GetInt("vignette", 0) == 0)
        {
            vignetteToggle.isOn = true;
            if (inGame == true && vignetteCam != null)
            {
                vignetteCam.SetActive(true);
            }
        }
        //Grain
        if (PlayerPrefs.GetInt("grain", 1) == 1)
        {
            grainToggle.isOn = false;
            if (inGame == true && grainCam != null)
            {
                grainCam.SetActive(false);
            }
        }
        if (PlayerPrefs.GetInt("grain", 0) == 0)
        {
            grainToggle.isOn = true;
            if (inGame == true && grainCam != null)
            {
                grainCam.SetActive(true);
            }
        }
    }
    
    // Este método se llama cuando se desactiva el objeto
    void OnDisable()
    {
        // Asegurarnos de que el check esté oculto cuando se desactive el menú
        if (checkMarkImage != null)
        {
            checkMarkImage.SetActive(false);
        }
        
        // Detener cualquier corrutina en progreso
        if (saveCoroutine != null)
        {
            StopCoroutine(saveCoroutine);
            saveCoroutine = null;
        }
    }
    
    // Método modificado para guardar configuración
    public void saveSettings()
    {
        PlayerPrefs.SetInt("settingsSaved", 1);
        PlayerPrefs.Save();
        
        // Mostrar la marca de verificación
        if (checkMarkImage != null)
        {
            checkMarkImage.SetActive(true);
            
            // Iniciar la corrutina para volver al menú anterior
            if (saveCoroutine != null)
            {
                StopCoroutine(saveCoroutine);
            }
            saveCoroutine = StartCoroutine(ReturnToPreviousMenu());
        }
        else
        {
            // Si no hay marca de verificación, volver inmediatamente al menú anterior
            ReturnToPrevious();
        }
    }
    
    // Método para volver directamente al menú anterior sin guardar
    public void backButton()
    {
        ReturnToPrevious();
    }
    
    // Corrutina para mostrar la marca y volver al menú anterior
    IEnumerator ReturnToPreviousMenu()
    {
        // Esperar el tiempo especificado mientras se muestra la marca
        yield return new WaitForSeconds(checkDisplayTime);
        
        // Ocultar la marca de verificación
        if (checkMarkImage != null)
        {
            checkMarkImage.SetActive(false);
        }
        
        // Volver al menú anterior
        ReturnToPrevious();
        
        // Limpiar la referencia a la corrutina
        saveCoroutine = null;
    }
    
    // Método para volver al menú anterior
    void ReturnToPrevious()
    {
        if (previousMenu != null && settingsMenu != null)
        {
            settingsMenu.SetActive(false);
            previousMenu.SetActive(true);
        }
        else
        {
            Debug.LogWarning("El menú anterior o el menú de configuración no están asignados. Verifica las referencias en el Inspector.");
        }
    }
    
    // Resto de los métodos...
    public void setGraphics()
    {
        if (graphicsDrop.value == 0)
        {
            PlayerPrefs.SetInt("graphics", 2);
            PlayerPrefs.Save();
            QualitySettings.SetQualityLevel(0);
        }
        if (graphicsDrop.value == 1)
        {
            PlayerPrefs.SetInt("graphics", 1);
            PlayerPrefs.Save();
            QualitySettings.SetQualityLevel(1);
        }
        if (graphicsDrop.value == 2)
        {
            PlayerPrefs.SetInt("graphics", 0);
            PlayerPrefs.Save();
            QualitySettings.SetQualityLevel(2);
        }
    }
    
    public void setResolution()
    {
        if (resoDrop.value == 0)
        {
            PlayerPrefs.SetInt("resolution", 2);
            PlayerPrefs.Save();
            Screen.SetResolution(854, 480, true);
            Debug.Log("480p set");
        }
        if (resoDrop.value == 1)
        {
            PlayerPrefs.SetInt("resolution", 1);
            PlayerPrefs.Save();
            Screen.SetResolution(1280, 720, true);
            Debug.Log("720p set");
        }
        if (resoDrop.value == 2)
        {
            PlayerPrefs.SetInt("resolution", 0);
            PlayerPrefs.Save();
            Screen.SetResolution(1920, 1080, true);
            Debug.Log("1080p set");
        }
    }
    
    public void setVolume()
    {
        PlayerPrefs.SetFloat("mastervolume", volumeSlider.value);
        PlayerPrefs.Save();
        AudioListener.volume = volumeSlider.value;
    }
    
    public void toggleChromatic()
    {
        if (chromaticToggle.isOn == false)
        {
            PlayerPrefs.SetInt("chromatic", 1);
            PlayerPrefs.Save();
            if (inGame == true && chromaticCam != null)
            {
                chromaticCam.SetActive(false);
            }
        }
        if (chromaticToggle.isOn == true)
        {
            PlayerPrefs.SetInt("chromatic", 0);
            PlayerPrefs.Save();
            if (inGame == true && chromaticCam != null)
            {
                chromaticCam.SetActive(true);
            }
        }
    }
    
    public void toggleVignette()
    {
        if (vignetteToggle.isOn == false)
        {
            PlayerPrefs.SetInt("vignette", 1);
            PlayerPrefs.Save();
            if (inGame == true && vignetteCam != null)
            {
                vignetteCam.SetActive(false);
            }
        }
        if (vignetteToggle.isOn == true)
        {
            PlayerPrefs.SetInt("vignette", 0);
            PlayerPrefs.Save();
            if (inGame == true && vignetteCam != null)
            {
                vignetteCam.SetActive(true);
            }
        }
    }
    
    public void toggleGrain()
    {
        if (grainToggle.isOn == false)
        {
            PlayerPrefs.SetInt("grain", 1);
            PlayerPrefs.Save();
            if (inGame == true && grainCam != null)
            {
                grainCam.SetActive(false);
            }
        }
        if (grainToggle.isOn == true)
        {
            PlayerPrefs.SetInt("grain", 0);
            PlayerPrefs.Save();
            if (inGame == true && grainCam != null)
            {
                grainCam.SetActive(true);
            }
        }
    }
}