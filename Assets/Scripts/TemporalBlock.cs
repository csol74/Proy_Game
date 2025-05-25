using UnityEngine;
using System.Collections;

public class TemporalBlock : MonoBehaviour
{
    [Header("Temporal Block Settings")]
    [SerializeField] private float disappearTime = 2f; // Tiempo antes de desaparecer
    [SerializeField] private float respawnTime = 3f;   // Tiempo para reaparecer
    [SerializeField] private bool autoRespawn = true;  // Si debe reaparecer automáticamente
    
    [Header("Visual Effects")]
    [SerializeField] private bool useBlinking = true;  // Parpadeo antes de desaparecer
    [SerializeField] private float blinkStartTime = 0.5f; // Cuándo empezar a parpadear
    [SerializeField] private float blinkSpeed = 0.1f;  // Velocidad del parpadeo
    
    [Header("Detection")]
    [SerializeField] private float detectionHeight = 0.1f; // Altura para detectar al jugador encima
    
    private BoxCollider blockCollider;
    private BoxCollider triggerCollider;
    private MeshRenderer blockRenderer;
    private bool isDisappearing = false;
    private bool hasDisappeared = false;
    private Coroutine disappearCoroutine;
    private Coroutine blinkCoroutine;
    private GameObject player;
    
    // Materiales originales para restaurar después del parpadeo
    private Material[] originalMaterials;
    
    void Start()
    {
        // Obtener componentes necesarios
        blockCollider = GetComponent<BoxCollider>();
        blockRenderer = GetComponent<MeshRenderer>();
        
        // Crear un segundo collider para detección (trigger)
        CreateTriggerCollider();
        
        // Guardar materiales originales
        if (blockRenderer != null)
        {
            originalMaterials = blockRenderer.materials;
        }
        
        // Configurar collider principal como sólido
        if (blockCollider != null)
        {
            blockCollider.isTrigger = false; // Sólido para que el jugador pueda pararse
            blockCollider.enabled = true;
        }
        
        // Buscar al jugador
        player = GameObject.FindGameObjectWithTag("Player");
        if (player == null)
        {
            // Si no tiene tag Player, buscar por componente
            PlayerController playerController = FindObjectOfType<PlayerController>();
            if (playerController != null)
            {
                player = playerController.gameObject;
            }
        }
    }
    
    void CreateTriggerCollider()
    {
        // Crear un GameObject hijo para el trigger
        GameObject triggerObject = new GameObject("TriggerDetector");
        triggerObject.transform.SetParent(transform);
        triggerObject.transform.localPosition = Vector3.up * detectionHeight;
        
        // Agregar collider trigger
        triggerCollider = triggerObject.AddComponent<BoxCollider>();
        triggerCollider.isTrigger = true;
        triggerCollider.size = new Vector3(1.1f, 0.1f, 1.1f); // Ligeramente más grande y delgado
        
        // Agregar el componente detector
        TriggerDetector detector = triggerObject.AddComponent<TriggerDetector>();
        detector.parentBlock = this;
    }
    
    public void OnPlayerEnterTrigger()
    {
        if (!isDisappearing && !hasDisappeared)
        {
            StartDisappearing();
        }
    }
    
    void Update()
    {
        // Método alternativo: verificar posición del jugador cada frame
        if (!isDisappearing && !hasDisappeared && player != null)
        {
            if (IsPlayerOnTop())
            {
                StartDisappearing();
            }
        }
    }
    
    bool IsPlayerOnTop()
    {
        if (player == null) return false;
        
        // Obtener bounds
        Bounds playerBounds = player.GetComponent<CharacterController>().bounds;
        Bounds blockBounds = blockCollider.bounds;
        
        // Verificar si el jugador está encima del bloque
        bool isOverlappingHorizontally = 
            playerBounds.min.x < blockBounds.max.x && 
            playerBounds.max.x > blockBounds.min.x &&
            playerBounds.min.z < blockBounds.max.z && 
            playerBounds.max.z > blockBounds.min.z;
            
        bool isOnTop = 
            playerBounds.min.y <= blockBounds.max.y + 0.1f && 
            playerBounds.min.y >= blockBounds.max.y - 0.2f;
        
        return isOverlappingHorizontally && isOnTop;
    }
    
    void StartDisappearing()
    {
        if (isDisappearing) return;
        
        isDisappearing = true;
        
        // Iniciar parpadeo si está habilitado
        if (useBlinking && blockRenderer != null)
        {
            blinkCoroutine = StartCoroutine(BlinkEffect());
        }
        
        // Iniciar cuenta regresiva para desaparecer
        disappearCoroutine = StartCoroutine(DisappearCountdown());
    }
    
    IEnumerator BlinkEffect()
    {
        float timeUntilBlink = disappearTime - blinkStartTime;
        
        // Esperar hasta que sea momento de parpadear
        if (timeUntilBlink > 0)
        {
            yield return new WaitForSeconds(timeUntilBlink);
        }
        
        // Parpadear hasta desaparecer
        while (!hasDisappeared)
        {
            // Ocultar
            if (blockRenderer != null)
            {
                blockRenderer.enabled = false;
            }
            
            yield return new WaitForSeconds(blinkSpeed);
            
            // Mostrar
            if (blockRenderer != null && !hasDisappeared)
            {
                blockRenderer.enabled = true;
            }
            
            yield return new WaitForSeconds(blinkSpeed);
        }
    }
    
    IEnumerator DisappearCountdown()
    {
        // Esperar el tiempo especificado
        yield return new WaitForSeconds(disappearTime);
        
        // Desaparecer el bloque
        DisappearBlock();
    }
    
    void DisappearBlock()
    {
        hasDisappeared = true;
        
        // Detener el parpadeo
        if (blinkCoroutine != null)
        {
            StopCoroutine(blinkCoroutine);
        }
        
        // Ocultar el bloque
        if (blockRenderer != null)
        {
            blockRenderer.enabled = false;
        }
        
        // Desactivar el collider para que el jugador pase a través
        if (blockCollider != null)
        {
            blockCollider.enabled = false;
        }
        
        // Iniciar respawn si está habilitado
        if (autoRespawn)
        {
            StartCoroutine(RespawnCountdown());
        }
    }
    
    IEnumerator RespawnCountdown()
    {
        yield return new WaitForSeconds(respawnTime);
        RespawnBlock();
    }
    
    void RespawnBlock()
    {
        // Restaurar estado original
        hasDisappeared = false;
        isDisappearing = false;
        
        // Reactivar el collider
        if (blockCollider != null)
        {
            blockCollider.enabled = true;
        }
        
        // Mostrar el bloque
        if (blockRenderer != null)
        {
            blockRenderer.enabled = true;
            // Restaurar materiales originales
            if (originalMaterials != null)
            {
                blockRenderer.materials = originalMaterials;
            }
        }
    }
    
    // Método público para resetear manualmente el bloque
    public void ResetBlock()
    {
        // Detener todas las corrutinas
        if (disappearCoroutine != null)
        {
            StopCoroutine(disappearCoroutine);
        }
        if (blinkCoroutine != null)
        {
            StopCoroutine(blinkCoroutine);
        }
        
        // Restaurar estado original
        RespawnBlock();
    }
    
    // Método público para hacer desaparecer inmediatamente
    public void ForceDisappear()
    {
        if (!hasDisappeared)
        {
            DisappearBlock();
        }
    }
}

// Clase auxiliar para detectar el trigger
public class TriggerDetector : MonoBehaviour
{
    public TemporalBlock parentBlock;
    
    void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<PlayerController>() != null)
        {
            parentBlock.OnPlayerEnterTrigger();
        }
    }
}