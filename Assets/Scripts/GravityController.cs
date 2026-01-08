using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Jelly Movement Controller for Kids
/// Touch anywhere on screen and the jelly moves towards that point
/// Simple, intuitive controls perfect for children
/// </summary>
public class GravityController : MonoBehaviour
{
    [Header("Movement Settings")]
    [Tooltip("Force applied towards touch point")]
    [SerializeField] private float moveForce = 10f;
    
    [Tooltip("Maximum speed the jelly can reach")]
    [SerializeField] private float maxVelocity = 8f;
    
    [Tooltip("How smooth the movement feels (higher = smoother but slower response)")]
    [SerializeField] [Range(0f, 1f)] private float movementSmoothing = 0.3f;
    
    [Header("Gravity Settings")]
    [Tooltip("Gravity when not touching (jelly falls down)")]
    [SerializeField] private float normalGravity = 2f;
    
    [Tooltip("Gravity when touching (0 = floats, negative = goes up)")]
    [SerializeField] private float touchGravity = 0f;
    
    [Header("References")]
    [Tooltip("Rigidbody2D component - will auto-assign if not set")]
    [SerializeField] private Rigidbody2D rb2D;
    
    [Header("Camera Reference")]
    [Tooltip("Main camera - will auto-assign if not set")]
    [SerializeField] private Camera mainCamera;
    
    [Header("Touch Effect")]
    [Tooltip("Touch effect prefab to spawn at touch position")]
    [SerializeField] private GameObject touchEffectPrefab;
    
    // Internal variables
    private bool isTouching = false;
    private Vector2 touchWorldPosition;
    
    private void Awake()
    {
        // Auto-assign Rigidbody2D if not set
        if (rb2D == null)
        {
            rb2D = GetComponent<Rigidbody2D>();
            
            if (rb2D == null)
            {
                Debug.LogError("GravityController: No Rigidbody2D found! Please add a Rigidbody2D component.");
            }
        }
        
        // Auto-assign Main Camera if not set
        if (mainCamera == null)
        {
            mainCamera = Camera.main;
            
            if (mainCamera == null)
            {
                Debug.LogError("GravityController: No Main Camera found!");
            }
        }
    }
    
    private void Start()
    {
        // Start with normal gravity (jelly falls)
        if (rb2D != null)
        {
            rb2D.gravityScale = normalGravity;
        }
    }
    
    private void Update()
    {
        HandleInput();
    }
    
    private void FixedUpdate()
    {
        // Apply movement force if touching
        if (isTouching && rb2D != null)
        {
            MoveTowardsTouchPoint();
            LimitVelocity();
        }
    }
    
    /// <summary>
    /// Handles touch and mouse input
    /// </summary>
    private void HandleInput()
    {
        // Check if Pointer device is available
        if (Pointer.current == null || mainCamera == null)
        {
            return;
        }
        
        bool isPressingNow = Pointer.current.press.isPressed;
        
        if (isPressingNow)
        {
            // Get touch/mouse position in screen coordinates
            Vector2 screenPosition = Pointer.current.position.ReadValue();
            
            // Convert screen position to world position
            touchWorldPosition = mainCamera.ScreenToWorldPoint(new Vector3(screenPosition.x, screenPosition.y, mainCamera.nearClipPlane));
            
            if (!isTouching)
            {
                // Just started touching
                isTouching = true;
                ActivateAntigravity();
                
                // Spawn touch effect at touch position
                SpawnTouchEffect(touchWorldPosition);
            }
        }
        else
        {
            if (isTouching)
            {
                // Just stopped touching
                isTouching = false;
                ActivateNormalGravity();
            }
        }
    }
    
    /// <summary>
    /// Spawns a touch effect at the given world position
    /// </summary>
    private void SpawnTouchEffect(Vector2 worldPosition)
    {
        if (touchEffectPrefab != null)
        {
            Vector3 spawnPosition = new Vector3(worldPosition.x, worldPosition.y, 0f);
            Instantiate(touchEffectPrefab, spawnPosition, Quaternion.identity);
        }
    }
    
    /// <summary>
    /// Moves the jelly towards the touch point using AddForce
    /// </summary>
    private void MoveTowardsTouchPoint()
    {
        // Calculate direction from jelly to touch point
        Vector2 currentPosition = rb2D.position;
        Vector2 direction = (touchWorldPosition - currentPosition).normalized;
        
        // Calculate distance to touch point
        float distance = Vector2.Distance(currentPosition, touchWorldPosition);
        
        // Apply force towards touch point
        // Force is stronger when further away (more intuitive for kids)
        float forceMagnitude = Mathf.Clamp(distance * movementSmoothing, 0f, 1f) * moveForce;
        rb2D.AddForce(direction * forceMagnitude);
    }
    
    /// <summary>
    /// Limits the jelly's velocity to prevent it from going too fast
    /// </summary>
    private void LimitVelocity()
    {
        if (rb2D.linearVelocity.magnitude > maxVelocity)
        {
            rb2D.linearVelocity = rb2D.linearVelocity.normalized * maxVelocity;
        }
    }
    
    /// <summary>
    /// Activates antigravity mode (jelly floats while touching)
    /// </summary>
    private void ActivateAntigravity()
    {
        if (rb2D != null)
        {
            rb2D.gravityScale = touchGravity;
        }
    }
    
    /// <summary>
    /// Activates normal gravity (jelly falls when not touching)
    /// </summary>
    private void ActivateNormalGravity()
    {
        if (rb2D != null)
        {
            rb2D.gravityScale = normalGravity;
        }
    }
    
    /// <summary>
    /// Public method to change movement force at runtime
    /// </summary>
    public void SetMoveForce(float newForce)
    {
        moveForce = Mathf.Max(0f, newForce);
    }
    
    /// <summary>
    /// Public method to change max velocity at runtime
    /// </summary>
    public void SetMaxVelocity(float newMaxVelocity)
    {
        maxVelocity = Mathf.Max(0.1f, newMaxVelocity);
    }
    
    /// <summary>
    /// Gets whether the player is currently touching the screen
    /// </summary>
    public bool IsTouching()
    {
        return isTouching;
    }
}
