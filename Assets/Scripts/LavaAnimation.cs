using UnityEngine;

public class LavaAnimation : MonoBehaviour
{
    [Header("Configuración de Lava")]
    public Material lavaMaterial;
    
    [Header("Velocidades de Desplazamiento")]
    public float scrollSpeedX = 0.2f;
    public float scrollSpeedY = 0.1f;
    
    [Header("Múltiples Capas (Opcional)")]
    public Material lavaLayer2Material;
    public float layer2SpeedX = -0.15f;
    public float layer2SpeedY = 0.25f;
    
    [Header("Efecto Pulsante")]
    public bool usePulsingEffect = true;
    public float pulseSpeed = 2f;
    public float pulseIntensity = 0.5f;
    public Color baseEmissionColor = Color.red;
    
    private Vector2 baseOffset;
    private Vector2 layer2BaseOffset;
    private Color originalEmission;
    
    void Start()
    {
        // Guardar valores originales
        if (lavaMaterial != null)
        {
            baseOffset = lavaMaterial.mainTextureOffset;
            originalEmission = lavaMaterial.GetColor("_EmissionColor");
        }
        
        if (lavaLayer2Material != null)
        {
            layer2BaseOffset = lavaLayer2Material.mainTextureOffset;
        }
    }
    
    void Update()
    {
        AnimateMainTexture();
        
        if (lavaLayer2Material != null)
        {
            AnimateSecondLayer();
        }
        
        if (usePulsingEffect)
        {
            AnimatePulsingEffect();
        }
    }
    
    void AnimateMainTexture()
    {
        if (lavaMaterial == null) return;
        
        float offsetX = (baseOffset.x + Time.time * scrollSpeedX) % 1f;
        float offsetY = (baseOffset.y + Time.time * scrollSpeedY) % 1f;
        
        lavaMaterial.mainTextureOffset = new Vector2(offsetX, offsetY);
    }
    
    void AnimateSecondLayer()
    {
        float offsetX = (layer2BaseOffset.x + Time.time * layer2SpeedX) % 1f;
        float offsetY = (layer2BaseOffset.y + Time.time * layer2SpeedY) % 1f;
        
        lavaLayer2Material.mainTextureOffset = new Vector2(offsetX, offsetY);
    }
    
    void AnimatePulsingEffect()
    {
        if (lavaMaterial == null) return;
        
        float pulse = Mathf.Sin(Time.time * pulseSpeed) * pulseIntensity + 1f;
        Color newEmission = baseEmissionColor * pulse;
        
        lavaMaterial.SetColor("_EmissionColor", newEmission);
    }
    
    void OnDestroy()
    {
        // Restaurar valores originales al destruir el objeto
        if (lavaMaterial != null)
        {
            lavaMaterial.mainTextureOffset = baseOffset;
            lavaMaterial.SetColor("_EmissionColor", originalEmission);
        }
        
        if (lavaLayer2Material != null)
        {
            lavaLayer2Material.mainTextureOffset = layer2BaseOffset;
        }
    }
}