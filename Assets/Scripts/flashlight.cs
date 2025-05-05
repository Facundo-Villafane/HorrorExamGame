using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class flashlight : MonoBehaviour
{
    public GameObject flashlightLight;
    public bool toggle;
    public AudioSource toggleSound;

    void Start()
    {
        if (toggle == false)
        {
            flashlightLight.SetActive(false);
        }
        if (toggle == true)
        {
            flashlightLight.SetActive(true);
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            toggle = !toggle;
            //toggleSound.Play();
            if (toggle == false)
            {
                flashlightLight.SetActive(false);
            }
            if (toggle == true)
            {
                flashlightLight.SetActive(true);
            }
        }
    }
}