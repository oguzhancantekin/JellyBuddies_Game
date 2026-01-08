using UnityEngine;

/// <summary>
/// Touch Effect - Simple Circle Animation
/// Creates a circle that grows and fades out at touch position
/// Use this as a prefab
/// </summary>
[RequireComponent(typeof(SpriteRenderer))]
public class TouchEffect : MonoBehaviour
{
    [Header("Animation Settings")]
    [SerializeField] private float duration = 0.5f;
    [SerializeField] private float startScale = 0.2f;
    [SerializeField] private float endScale = 2f;
    
    private SpriteRenderer spriteRenderer;
    private float timer = 0f;
    private Vector3 initialScale;
    
    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        initialScale = Vector3.one;
        
        // Set initial state
        transform.localScale = initialScale * startScale;
        
        // Ensure color is white with full alpha
        Color color = spriteRenderer.color;
        color = Color.white;
        color.a = 1f;
        spriteRenderer.color = color;
    }
    
    private void Update()
    {
        timer += Time.deltaTime;
        float progress = timer / duration;
        
        if (progress >= 1f)
        {
            Destroy(gameObject);
            return;
        }
        
        // Scale animation (grow)
        float currentScale = Mathf.Lerp(startScale, endScale, progress);
        transform.localScale = initialScale * currentScale;
        
        // Alpha animation (fade out)
        Color color = spriteRenderer.color;
        color.a = Mathf.Lerp(1f, 0f, progress);
        spriteRenderer.color = color;
    }
}
