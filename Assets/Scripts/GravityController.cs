using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Gravity Controller for Jelly Buddies
/// Controls gravity based on touch input - active gravity when touching, antigravity when released
/// Uses Unity's New Input System (Input System Package)
/// </summary>
public class GravityController : MonoBehaviour
{
    [Header("Gravity Settings")]
    [Tooltip("Gravity value when touch is active (default: 9.81)")]
    [SerializeField] private float activeGravity = 9.81f;
    
    [Tooltip("Gravity value when touch is released (antigravity mode)")]
    [SerializeField] private float antigravityValue = 0f;
    
    [Header("References")]
    [Tooltip("Rigidbody2D component - will auto-assign if not set")]
    [SerializeField] private Rigidbody2D rb2D;
    
    private bool isTouching = false;
    private bool wasPressingLastFrame = false;
    
    private void Awake()
    {
        // Auto-assign Rigidbody2D if not set in inspector
        if (rb2D == null)
        {
            rb2D = GetComponent<Rigidbody2D>();
            
            if (rb2D == null)
            {
                Debug.LogError("GravityController: No Rigidbody2D found! Please add a Rigidbody2D component.");
            }
        }
    }
    
    private void Start()
    {
        // Start with antigravity mode (no gravity)
        SetGravity(antigravityValue);
    }
    
    private void Update()
    {
        HandleInput();
    }
    
    /// <summary>
    /// Handles touch and mouse input for gravity control using New Input System
    /// </summary>
    private void HandleInput()
    {
        // Check if Pointer device is available (works for both touch and mouse)
        if (Pointer.current == null)
        {
            return;
        }
        
        // Get current press state
        bool isPressingNow = Pointer.current.press.isPressed;
        
        // Detect press started (touch began or mouse down)
        if (isPressingNow && !wasPressingLastFrame)
        {
            isTouching = true;
            ActivateGravity();
        }
        // Detect press ended (touch ended or mouse up)
        else if (!isPressingNow && wasPressingLastFrame)
        {
            isTouching = false;
            ActivateAntigravity();
        }
        
        // Update state for next frame
        wasPressingLastFrame = isPressingNow;
    }
    
    /// <summary>
    /// Activates normal gravity (downward pull)
    /// </summary>
    private void ActivateGravity()
    {
        SetGravity(activeGravity);
        Debug.Log("Gravity Activated: " + activeGravity);
    }
    
    /// <summary>
    /// Activates antigravity mode (no gravity)
    /// </summary>
    private void ActivateAntigravity()
    {
        SetGravity(antigravityValue);
        Debug.Log("Antigravity Mode Activated");
    }
    
    /// <summary>
    /// Sets the gravity scale on the Rigidbody2D
    /// </summary>
    /// <param name="gravityValue">Gravity scale value</param>
    private void SetGravity(float gravityValue)
    {
        if (rb2D != null)
        {
            rb2D.gravityScale = gravityValue;
        }
    }
    
    /// <summary>
    /// Public method to change active gravity value at runtime
    /// </summary>
    public void SetActiveGravity(float newGravity)
    {
        activeGravity = newGravity;
        
        // If currently touching, update gravity immediately
        if (isTouching)
        {
            SetGravity(activeGravity);
        }
    }
    
    /// <summary>
    /// Public method to get current gravity state
    /// </summary>
    public bool IsGravityActive()
    {
        return isTouching;
    }
}
