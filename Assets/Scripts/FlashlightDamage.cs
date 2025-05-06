using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlashlightDamage : MonoBehaviour
{
    // Referencia al script existente de la linterna
    public flashlight flashlightScript;
    
    // Configuración del daño
    public int damagePerSecond = 10;
    public float damageRange = 10f;
    public float damageAngle = 30f; // Ángulo del cono de luz
    
    // Efectos visuales cuando se daña a un enemigo
    public GameObject hitEffectPrefab;
    
    // Depuración
    public bool showDebugVisuals = true;
    public bool showDebugMessages = true;
    
    // Componentes internos
    private Light flashlightLight;
    private Transform playerCamera;
    
    // Para controlar la velocidad del daño
    private float damageTimer = 0f;
    private float damageInterval = 0.1f; // Aplicar daño cada 0.1 segundos
    
    void Start()
    {
        // Obtener la referencia a la luz de la linterna
        flashlightLight = GetComponent<Light>();
        
        // Si no se asignó el script, buscarlo
        if (flashlightScript == null)
            flashlightScript = GetComponentInParent<flashlight>();
            
        // Obtener la cámara del jugador
        playerCamera = Camera.main.transform;
        
        if (showDebugMessages)
            Debug.Log("FlashlightDamage inicializado correctamente. Rango: " + damageRange + ", Ángulo: " + damageAngle);
    }
    
    void Update()
    {
        // Solo dañar si la linterna está encendida
        if (flashlightScript != null && flashlightScript.toggle)
        {
            damageTimer += Time.deltaTime;
            
            // Aplicar daño en intervalos
            if (damageTimer >= damageInterval)
            {
                DamageEnemiesInLight();
                damageTimer = 0f;
            }
        }
    }
    
    void DamageEnemiesInLight()
    {
        if (showDebugMessages)
            Debug.Log("Buscando enemigos en el haz de luz...");
        
        // Usar OverlapSphere para una mejor detección
        Collider[] hitColliders = Physics.OverlapSphere(playerCamera.position, damageRange);
        
        if (showDebugMessages && hitColliders.Length > 0)
            Debug.Log("Detectados " + hitColliders.Length + " objetos en el rango");
        
        foreach (var hitCollider in hitColliders)
        {
            // Buscar el componente EnemyHealth en el objeto o sus padres
            EnemyHealth enemyHealth = hitCollider.GetComponent<EnemyHealth>();
            if (enemyHealth == null)
                enemyHealth = hitCollider.GetComponentInParent<EnemyHealth>();
            
            // Si encontramos un enemigo con el componente de salud
            if (enemyHealth != null)
            {
                // Calcular dirección y distancia al enemigo
                Vector3 directionToEnemy = enemyHealth.transform.position - playerCamera.position;
                float distance = directionToEnemy.magnitude;
                
                // Verificar si está en la dirección del rayo de luz
                float angle = Vector3.Angle(playerCamera.forward, directionToEnemy);
                
                // Si está dentro del cono de luz
                if (angle <= damageAngle * 0.5f)
                {
                    // Raycast para verificar que no hay obstáculos entre la linterna y el enemigo
                    RaycastHit hit;
                    if (Physics.Raycast(playerCamera.position, directionToEnemy.normalized, out hit, distance + 0.5f))
                    {
                        // Comprobar si golpeamos al enemigo o a uno de sus hijos
                        bool hitEnemy = hit.collider.gameObject == enemyHealth.gameObject;
                        if (!hitEnemy)
                        {
                            // Comprobar si el objeto golpeado es hijo del enemigo
                            Transform hitTransform = hit.collider.transform;
                            while (hitTransform.parent != null)
                            {
                                if (hitTransform.parent.gameObject == enemyHealth.gameObject)
                                {
                                    hitEnemy = true;
                                    break;
                                }
                                hitTransform = hitTransform.parent;
                            }
                        }
                        
                        if (hitEnemy)
                        {
                            // Calcular daño basado en la distancia (más daño cuanto más cerca)
                            float distanceFactor = 1f - Mathf.Clamp01(distance / damageRange);
                            int damage = Mathf.Max(1, Mathf.RoundToInt(damagePerSecond * damageInterval * distanceFactor));
                            
                            // Aplicar daño al enemigo
                            enemyHealth.TakeDamage(damage);
                            
                            if (showDebugMessages)
                                Debug.Log("¡Dañando al enemigo! Nombre: " + enemyHealth.name + ", Daño: " + damage);
                            
                            // Mostrar efecto de impacto si existe
                            if (hitEffectPrefab != null)
                            {
                                GameObject effect = Instantiate(hitEffectPrefab, hit.point, Quaternion.identity);
                                Destroy(effect, 1f);
                            }
                        }
                        else if (showDebugMessages)
                        {
                            Debug.Log("Hay un obstáculo entre la linterna y el enemigo: " + hit.collider.name);
                        }
                    }
                }
                else if (showDebugMessages)
                {
                    Debug.Log("Enemigo fuera del ángulo de la luz: " + angle + " grados (máx: " + (damageAngle * 0.5f) + ")");
                }
            }
        }
    }
    
    // Para debug: visualizar el cono de luz y los enemigos
    void OnDrawGizmos()
    {
        if (!showDebugVisuals || playerCamera == null) 
            return;
            
        Gizmos.color = Color.yellow;
        
        // Dibujar el cono de luz
        Gizmos.DrawRay(playerCamera.position, playerCamera.forward * damageRange);
        
        // Calcular los vectores del cono
        Vector3 rightDir = Quaternion.AngleAxis(damageAngle * 0.5f, playerCamera.up) * playerCamera.forward;
        Vector3 leftDir = Quaternion.AngleAxis(-damageAngle * 0.5f, playerCamera.up) * playerCamera.forward;
        Vector3 upDir = Quaternion.AngleAxis(damageAngle * 0.5f, playerCamera.right) * playerCamera.forward;
        Vector3 downDir = Quaternion.AngleAxis(-damageAngle * 0.5f, playerCamera.right) * playerCamera.forward;
        
        // Dibujar líneas para representar el cono
        Gizmos.DrawRay(playerCamera.position, rightDir * damageRange);
        Gizmos.DrawRay(playerCamera.position, leftDir * damageRange);
        Gizmos.DrawRay(playerCamera.position, upDir * damageRange);
        Gizmos.DrawRay(playerCamera.position, downDir * damageRange);
        
        // Dibujar una esfera en la posición de cada enemigo con EnemyHealth
        Gizmos.color = Color.red;
        EnemyHealth[] enemies = FindObjectsOfType<EnemyHealth>();
        foreach (var enemy in enemies)
        {
            if (enemy != null)
                Gizmos.DrawSphere(enemy.transform.position, 0.5f);
        }
    }
}