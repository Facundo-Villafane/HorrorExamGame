using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class pickupFlashlight : MonoBehaviour
{
    public GameObject inttext, flashlight_table, flashlight_hand;
    public AudioSource pickup;
    public bool interactable;
    
    // Añadir referencia al panel de batería
    public GameObject batteryUIPanel;

    void Start()
    {
        // Asegurarse que el panel de batería esté desactivado al inicio
        if (batteryUIPanel != null)
        {
            batteryUIPanel.SetActive(false);
        }
    }

    void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("MainCamera"))
        {
            inttext.SetActive(true);
            interactable = true;
        }
    }
    
    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("MainCamera"))
        {
            inttext.SetActive(false);
            interactable = false;
        }
    }
    
    void Update()
    {
        if (interactable == true)
        {
            if (Input.GetKeyDown(KeyCode.E))
            {
                inttext.SetActive(false);
                interactable = false;
                
                //pickup.Play();
                flashlight_hand.SetActive(true);
                flashlight_table.SetActive(false);
                
                // Activar el panel de UI de la batería cuando recoges la linterna
                if (batteryUIPanel != null)
                {
                    batteryUIPanel.SetActive(true);
                }
                
                // También podríamos activar el uso de la batería en la linterna aquí si quieres
                flashlight flashlightScript = flashlight_hand.GetComponentInChildren<flashlight>();
                if (flashlightScript != null)
                {
                    flashlightScript.hasBatteryLimit = true;
                }
            }
        }
    }
}