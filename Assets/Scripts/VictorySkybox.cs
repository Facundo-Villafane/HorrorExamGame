using UnityEngine;

public class VictorySkybox : MonoBehaviour
{
    public Material victorySkyboxMaterial; // Asigna el material desde el Inspector
    
    void Start()
    {
        // Guardar el skybox actual para restaurarlo después si es necesario
        Material defaultSkybox = RenderSettings.skybox;
        
        // Aplicar el nuevo skybox
        RenderSettings.skybox = victorySkyboxMaterial;
        
        // Opcional: Forzar la actualización del reflejo ambiental
        DynamicGI.UpdateEnvironment();
    }
}