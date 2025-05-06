using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class nextLevel : MonoBehaviour
{
    public string sceneName; // Escena a la que queremos ir (nivel 2 o victoria)
    public int nextLevelNumber; // El número del nivel al que estamos yendo (2 para nivel 2, 3 para victoria)
    
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // Guardar el nivel al que vamos
            PlayerPrefs.SetInt("level", nextLevelNumber);
            PlayerPrefs.Save();
            
            Debug.Log("Avanzando al nivel: " + nextLevelNumber + " (Escena: " + sceneName + ")");
            
            // Cargar la siguiente escena
            SceneManager.LoadScene(sceneName);
        }
    }
}