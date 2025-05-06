using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; // Para Image
using TMPro; // Para TextMeshPro

public class flashlight : MonoBehaviour
{
    public GameObject flashlightLight;
    public bool toggle;
    public AudioSource toggleSound;
    
    [Header("Configuración de Batería")]
    public bool canDamageEnemies = true;
    public float batteryLife = 100f;
    public float batteryDrainRate = 5f;
    public float batteryRechargeRate = 2f;
    public bool hasBatteryLimit = true;
    
    [Header("Interfaz de Batería")]
    public Image batteryFillImage;
    public TMP_Text batteryPercentText;
    public GameObject batteryLowWarning;
    public float lowBatteryThreshold = 20f;
    
    private bool hasTriggeredLowBattery = false;
    
    void Start()
    {
        // Inicializar el estado de la linterna según la configuración del inspector
        if (flashlightLight != null)
        {
            flashlightLight.SetActive(toggle);
        }
        
        // Actualizar UI de batería
        UpdateBatteryUI();
        
        // Desactivar advertencia de batería baja al inicio
        if (batteryLowWarning != null)
            batteryLowWarning.SetActive(false);
    }

    void Update()
    {
        // Manejar la entrada para encender/apagar la linterna
        if (Input.GetKeyDown(KeyCode.F))
        {
            // Invertir el estado
            toggle = !toggle;
            
            // Aplicar el cambio al objeto de luz
            if (flashlightLight != null)
            {
                flashlightLight.SetActive(toggle);
            }
            
            // Reproducir sonido si está disponible
            if (toggleSound != null)
                toggleSound.Play();
        }
        
        // Sistema de batería
        if (hasBatteryLimit)
        {
            if (toggle && batteryLife > 0)
            {
                // Consumir batería cuando está encendida
                batteryLife -= batteryDrainRate * Time.deltaTime;
                batteryLife = Mathf.Clamp(batteryLife, 0, 100);
                
                // Comprobar si la batería está baja
                CheckLowBattery();
                
                // Si la batería se agota, apagar la linterna
                if (batteryLife <= 0)
                {
                    toggle = false;
                    if (flashlightLight != null)
                    {
                        flashlightLight.SetActive(false);
                    }
                }
            }
            else if (!toggle)
            {
                // Recargar batería cuando está apagada
                float previousBattery = batteryLife;
                batteryLife += batteryRechargeRate * Time.deltaTime;
                batteryLife = Mathf.Clamp(batteryLife, 0, 100);
                
                // Resetear la advertencia de batería baja cuando se recarga por encima del umbral
                if (batteryLife > lowBatteryThreshold && hasTriggeredLowBattery)
                {
                    hasTriggeredLowBattery = false;
                    if (batteryLowWarning != null)
                        batteryLowWarning.SetActive(false);
                }
            }
        }
        
        // Asegurarse de que la linterna esté realmente apagada si toggle es falso
        // Esta línea es crucial para solucionar el problema
        if (!toggle && flashlightLight != null && flashlightLight.activeSelf)
        {
            flashlightLight.SetActive(false);
        }
        
        // Actualizar UI de batería
        UpdateBatteryUI();
    }
    
    // Verificar si la batería está baja
    private void CheckLowBattery()
    {
        if (batteryLife <= lowBatteryThreshold && !hasTriggeredLowBattery)
        {
            hasTriggeredLowBattery = true;
            
            // Activar advertencia visual
            if (batteryLowWarning != null)
                batteryLowWarning.SetActive(true);
        }
    }
    
    // Método para actualizar la UI de la batería
    private void UpdateBatteryUI()
    {
        // Actualizar la barra de llenado
        if (batteryFillImage != null)
        {
            batteryFillImage.fillAmount = batteryLife / 100f;
            
            // Opcional: Cambiar el color según el nivel de batería
            if (batteryLife <= lowBatteryThreshold)
                batteryFillImage.color = Color.red;
            else if (batteryLife <= 50f)
                batteryFillImage.color = Color.yellow;
            else
                batteryFillImage.color = Color.green;
        }
        
        // Actualizar el texto del porcentaje
        if (batteryPercentText != null)
        {
            batteryPercentText.text = Mathf.RoundToInt(batteryLife) + "%";
        }
    }
}