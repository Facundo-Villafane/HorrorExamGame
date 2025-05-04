using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour
{
    public GameObject intText, key, lockedText;
    public bool interactable, toggle;
    public Animator doorAnim;
    
    // Referencias a los objetos que voy a desactivar/activar
    public GameObject wallToDisable;  // La pared que bloquea el camino
    public GameObject portalEffects;  // Efectos para el portal
    
    void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("MainCamera"))
        {
            intText.SetActive(true);
            interactable = true;
        }
    }
    
    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("MainCamera"))
        {
            intText.SetActive(false);
            interactable = false;
        }
    }
    
    void Update()
    {
        if (interactable == true)
        {
            if (Input.GetKeyDown(KeyCode.E))
            {
                
                if(!key.activeSelf)  // Si la llave ha sido recogida
                {
                    toggle = !toggle;
                    if (toggle == true)
                    {
                        doorAnim.ResetTrigger("close");
                        doorAnim.SetTrigger("open");
                        
                        // Desactivar la pared cuando la puerta se abre
                        if (wallToDisable != null)
                        {
                            wallToDisable.SetActive(false);
                        }
                        
                        // Activar efectos del portal si existen
                        if (portalEffects != null)
                        {
                            portalEffects.SetActive(true);
                        }
                    }
                    if (toggle == false)
                    {
                        doorAnim.ResetTrigger("open");
                        doorAnim.SetTrigger("close");
                        
                    }
                    intText.SetActive(false);
                    interactable = false;
                }
                else  // Si la llave no ha sido recogida
                {
                    lockedText.SetActive(true);
                    StopCoroutine("DisableText");
                    StartCoroutine("DisableText");
                }
            }
        }
    }
    
    IEnumerator DisableText()
    {
        yield return new WaitForSeconds(2.0f);
        lockedText.SetActive(false);
    }
}