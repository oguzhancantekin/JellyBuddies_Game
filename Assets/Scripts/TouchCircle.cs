using UnityEngine;

/// <summary>
/// Touch Circle Effect
/// Creates a visual feedback circle that grows and fades out
/// Perfect for showing where the player touched
/// </summary>
public class TouchCircle : MonoBehaviour
{
    [Header("Animation Settings")]
    [Tooltip("How long the circle animation lasts")]
    [SerializeField] private float lifetime = 0.5f;
    
    [Tooltip("Starting scale of the circle")]
    [SerializeField] private float startScale = 0.1f;
    
    [Tooltip("Ending scale of the circle")]
    [SerializeField] private float endScale = 1.5f;
    
    [Tooltip("Starting alpha (transparency)")]
    [SerializeField] private float startAlpha = 0.8f;
    
    [Tooltip("Ending alpha (transparency)")]
    [SerializeField] private float endAlpha = 0f;
    
    // Internal variables
    private SpriteRenderer spriteRenderer;
    private float timer = 0f;
    private Vector3 initialScale;
    
    private void Awake()
    {
        // Get or add SpriteRenderer
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer == null)
        {
            spriteRenderer = gameObject.AddComponent<SpriteRenderer>();
        }
        
        // Set initial scale
        initialScale = transform.localScale;
        transform.localScale = initialScale * startScale;
        
        // Set initial alpha
        Color color = spriteRenderer.color;
        color.a = startAlpha;
        spriteRenderer.color = color;
    }
    
    private void Update()
    {
        // Update timer
        timer += Time.deltaTime;
        
        // Calculate progress (0 to 1)
        float progress = timer / lifetime;
        
        if (progress >= 1f)
        {
            // Animation complete, destroy this object
            Destroy(gameObject);
            return;
        }
        
        // Animate scale (grow)
        float currentScale = Mathf.Lerp(startScale, endScale, progress);
        transform.localScale = initialScale * currentScale;
        
        // Animate alpha (fade out)
        float currentAlpha = Mathf.Lerp(startAlpha, endAlpha, progress);
        Color color = spriteRenderer.color;
        color.a = currentAlpha;
        spriteRenderer.color = color;
    }
    
    /// <summary>
    /// Sets up the circle with custom settings
    /// </summary>
    public void Setup(Sprite circleSprite, Color circleColor, float duration = 0.5f)
    {
        if (spriteRenderer != null && circleSprite != null)
        {
            spriteRenderer.sprite = circleSprite;
            spriteRenderer.color = circleColor;
        }
        
        lifetime = duration;
    }
}
