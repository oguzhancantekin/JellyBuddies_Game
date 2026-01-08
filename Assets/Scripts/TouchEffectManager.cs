using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Touch Effect Manager
/// Spawns visual feedback circles when the player touches the screen
/// </summary>
public class TouchEffectManager : MonoBehaviour
{
    [Header("Circle Prefab")]
    [Tooltip("Prefab of the touch circle effect")]
    [SerializeField] private GameObject touchCirclePrefab;
    
    [Header("Circle Settings")]
    [Tooltip("Color of the touch circle")]
    [SerializeField] private Color circleColor = Color.white;
    
    [Tooltip("Duration of the circle animation")]
    [SerializeField] private float circleDuration = 0.5f;
    
    [Tooltip("Sorting layer order for the circle")]
    [SerializeField] private int sortingOrder = 100;
    
    [Header("Spawn Settings")]
    [Tooltip("Minimum time between circle spawns (prevents spam)")]
    [SerializeField] private float minSpawnInterval = 0.1f;
    
    [Header("Camera Reference")]
    [Tooltip("Main camera - will auto-assign if not set")]
    [SerializeField] private Camera mainCamera;
    
    // Internal variables
    private float lastSpawnTime = 0f;
    
    private void Awake()
    {
        // Auto-assign Main Camera if not set
        if (mainCamera == null)
        {
            mainCamera = Camera.main;
            
            if (mainCamera == null)
            {
                Debug.LogError("TouchEffectManager: No Main Camera found!");
            }
        }
    }
    
    private void Update()
    {
        HandleTouchInput();
    }
    
    /// <summary>
    /// Handles touch and mouse input to spawn circles
    /// </summary>
    private void HandleTouchInput()
    {
        // Check if Pointer device is available
        if (Pointer.current == null || mainCamera == null)
        {
            return;
        }
        
        // Check if pressing and enough time has passed since last spawn
        if (Pointer.current.press.wasPressedThisFrame)
        {
            if (Time.time - lastSpawnTime >= minSpawnInterval)
            {
                // Get touch/mouse position
                Vector2 screenPosition = Pointer.current.position.ReadValue();
                
                // Spawn circle at touch position
                SpawnTouchCircle(screenPosition);
                
                lastSpawnTime = Time.time;
            }
        }
    }
    
    /// <summary>
    /// Spawns a touch circle at the given screen position
    /// </summary>
    private void SpawnTouchCircle(Vector2 screenPosition)
    {
        if (touchCirclePrefab == null)
        {
            Debug.LogWarning("TouchEffectManager: Touch circle prefab is not assigned!");
            return;
        }
        
        // Convert screen position to world position
        Vector3 worldPosition = mainCamera.ScreenToWorldPoint(new Vector3(screenPosition.x, screenPosition.y, mainCamera.nearClipPlane));
        worldPosition.z = 0f; // Ensure it's at Z = 0
        
        // Instantiate the circle
        GameObject circle = Instantiate(touchCirclePrefab, worldPosition, Quaternion.identity);
        
        // Get the TouchCircle component
        TouchCircle touchCircle = circle.GetComponent<TouchCircle>();
        
        // Get SpriteRenderer and set color and sorting order
        SpriteRenderer spriteRenderer = circle.GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
        {
            spriteRenderer.color = circleColor;
            spriteRenderer.sortingOrder = sortingOrder;
        }
        
        // Setup the circle if component exists
        if (touchCircle != null && spriteRenderer != null && spriteRenderer.sprite != null)
        {
            touchCircle.Setup(spriteRenderer.sprite, circleColor, circleDuration);
        }
    }
    
    /// <summary>
    /// Changes the circle color at runtime
    /// </summary>
    public void SetCircleColor(Color newColor)
    {
        circleColor = newColor;
    }
    
    /// <summary>
    /// Changes the circle duration at runtime
    /// </summary>
    public void SetCircleDuration(float newDuration)
    {
        circleDuration = Mathf.Max(0.1f, newDuration);
    }
}
