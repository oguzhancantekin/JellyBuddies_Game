using UnityEngine;

/// <summary>
/// Jelly Buddy Script for Jelly Buddies
/// Adds jelly-like physics: rotation, squash/stretch on impact, and breathing animation
/// </summary>
public class JellyBuddy : MonoBehaviour
{
    [Header("Jelly Effect Settings")]
    [Tooltip("Maximum torque force applied for rotation")]
    [SerializeField] private float maxTorque = 2f;
    
    [Tooltip("Minimum torque force applied for rotation")]
    [SerializeField] private float minTorque = 0.5f;
    
    [Tooltip("How often to change rotation direction (in seconds)")]
    [SerializeField] private float torqueChangeInterval = 1.5f;
    
    [Tooltip("Damping factor for smooth rotation (0-1, higher = more damping)")]
    [SerializeField] [Range(0f, 1f)] private float rotationDamping = 0.95f;
    
    [Header("Floating Effect")]
    [Tooltip("Enable subtle floating oscillation")]
    [SerializeField] private bool enableFloatingOscillation = true;
    
    [Tooltip("Floating oscillation strength")]
    [SerializeField] private float floatStrength = 0.3f;
    
    [Tooltip("Floating oscillation speed")]
    [SerializeField] private float floatSpeed = 2f;
    
    [Header("Squash & Stretch")]
    [Tooltip("Enable squash effect on collision")]
    [SerializeField] private bool enableSquashOnImpact = true;
    
    [Tooltip("How much to squash on impact (0-1, higher = more squash)")]
    [SerializeField] [Range(0f, 0.5f)] private float squashAmount = 0.3f;
    
    [Tooltip("How long the squash effect lasts (in seconds)")]
    [SerializeField] private float squashDuration = 0.2f;
    
    [Tooltip("Minimum impact velocity to trigger squash")]
    [SerializeField] private float minImpactVelocity = 2f;
    
    [Header("Breathing Effect")]
    [Tooltip("Enable breathing (size pulsing) while floating")]
    [SerializeField] private bool enableBreathing = true;
    
    [Tooltip("Breathing intensity (how much size changes)")]
    [SerializeField] [Range(0f, 0.2f)] private float breathingIntensity = 0.05f;
    
    [Tooltip("Breathing speed")]
    [SerializeField] private float breathingSpeed = 1.5f;
    
    [Header("Star Collection")]
    [Tooltip("Enable star collection mechanic")]
    [SerializeField] private bool enableStarCollection = true;
    
    [Tooltip("Scale boost when collecting a star")]
    [SerializeField] private float starCollectionScaleBoost = 1.3f;
    
    [Tooltip("Duration of star collection effect")]
    [SerializeField] private float starCollectionDuration = 0.3f;
    
    [Tooltip("Tag name for collectible stars")]
    [SerializeField] private string starTag = "Star";
    
    [Header("References")]
    [Tooltip("Rigidbody2D component - will auto-assign if not set")]
    [SerializeField] private Rigidbody2D rb2D;
    
    // Rotation variables
    private float currentTorque;
    private float torqueTimer;
    private float floatOffset;
    
    // Squash & Stretch variables
    private Vector3 originalScale;
    private bool isSquashing = false;
    private float squashTimer = 0f;
    private Vector3 targetScale;
    private Vector3 squashVelocity;
    
    // Breathing variables
    private float breathingOffset;
    
    // Star collection variables
    private bool isCollectingStarEffect = false;
    private float starCollectionTimer = 0f;
    private Vector3 starCollectionVelocity;
    
    private void Awake()
    {
        // Auto-assign Rigidbody2D if not set in inspector
        if (rb2D == null)
        {
            rb2D = GetComponent<Rigidbody2D>();
            
            if (rb2D == null)
            {
                Debug.LogError("JellyBuddy: No Rigidbody2D found! Please add a Rigidbody2D component.");
            }
        }
        
        // Store original scale
        originalScale = transform.localScale;
        targetScale = originalScale;
        
        // Initialize with random torque
        currentTorque = Random.Range(minTorque, maxTorque) * (Random.value > 0.5f ? 1f : -1f);
        torqueTimer = 0f;
        floatOffset = Random.Range(0f, Mathf.PI * 2f);
        breathingOffset = Random.Range(0f, Mathf.PI * 2f);
    }
    
    private void FixedUpdate()
    {
        if (rb2D == null) return;
        
        // Apply jelly-like rotation torque
        ApplyJellyRotation();
        
        // Apply floating oscillation if enabled
        if (enableFloatingOscillation)
        {
            ApplyFloatingEffect();
        }
        
        // Apply damping to prevent excessive spinning
        ApplyRotationDamping();
    }
    
    private void Update()
    {
        // Handle star collection effect (highest priority)
        if (isCollectingStarEffect)
        {
            UpdateStarCollectionEffect();
        }
        // Handle squash animation
        else if (isSquashing)
        {
            UpdateSquashAnimation();
        }
        // Apply breathing effect when not squashing
        else if (enableBreathing)
        {
            ApplyBreathingEffect();
        }
    }
    
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!enableSquashOnImpact) return;
        
        // Get impact velocity
        float impactVelocity = collision.relativeVelocity.magnitude;
        
        // Only squash if impact is strong enough
        if (impactVelocity >= minImpactVelocity)
        {
            TriggerSquash(collision);
        }
    }
    
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        // Check if the object is a star
        if (other.CompareTag("Star"))
        {
            // Find ScoreManager and increase score
            ScoreManager scoreManager = FindAnyObjectByType<ScoreManager>();
            if (scoreManager != null)
            {
                scoreManager.IncreaseScore(1);
            }
            else
            {
                Debug.LogWarning("JellyBuddy: ScoreManager not found in scene!");
            }
            
            // Destroy the star
            Destroy(other.gameObject);
        }
    }
    
    /// <summary>
    /// Triggers the squash effect based on collision
    /// </summary>
    private void TriggerSquash(Collision2D collision)
    {
        // Calculate squash direction based on collision normal
        Vector2 impactNormal = collision.contacts[0].normal;
        
        // Determine if impact is more vertical or horizontal
        bool isVerticalImpact = Mathf.Abs(impactNormal.y) > Mathf.Abs(impactNormal.x);
        
        if (isVerticalImpact)
        {
            // Squash vertically (flatten on Y, expand on X)
            targetScale = new Vector3(
                originalScale.x * (1f + squashAmount),
                originalScale.y * (1f - squashAmount),
                originalScale.z
            );
        }
        else
        {
            // Squash horizontally (flatten on X, expand on Y)
            targetScale = new Vector3(
                originalScale.x * (1f - squashAmount),
                originalScale.y * (1f + squashAmount),
                originalScale.z
            );
        }
        
        isSquashing = true;
        squashTimer = 0f;
    }
    
    /// <summary>
    /// Updates the squash animation using smooth tweening
    /// </summary>
    private void UpdateSquashAnimation()
    {
        squashTimer += Time.deltaTime;
        float progress = squashTimer / squashDuration;
        
        if (progress < 0.5f)
        {
            // First half: squash to target
            float t = progress * 2f; // 0 to 1
            transform.localScale = Vector3.SmoothDamp(transform.localScale, targetScale, ref squashVelocity, squashDuration * 0.25f);
        }
        else
        {
            // Second half: return to original
            float t = (progress - 0.5f) * 2f; // 0 to 1
            transform.localScale = Vector3.SmoothDamp(transform.localScale, originalScale, ref squashVelocity, squashDuration * 0.25f);
        }
        
        // End squash animation
        if (progress >= 1f)
        {
            isSquashing = false;
            transform.localScale = originalScale;
        }
    }
    
    /// <summary>
    /// Applies breathing effect (subtle size pulsing)
    /// </summary>
    private void ApplyBreathingEffect()
    {
        // Create sine wave for breathing
        float breathingScale = 1f + (Mathf.Sin((Time.time * breathingSpeed) + breathingOffset) * breathingIntensity);
        
        // Apply uniform scaling for breathing
        transform.localScale = originalScale * breathingScale;
    }
    
    /// <summary>
    /// Handles star collection logic
    /// </summary>
    private void CollectStar(GameObject star)
    {
        Debug.Log($"[JellyBuddy] CollectStar() called for: {star.name}");
        
        // Find ScoreManager in the scene automatically
        ScoreManager scoreManager = FindAnyObjectByType<ScoreManager>();
        
        if (scoreManager != null)
        {
            Debug.Log("[JellyBuddy] ✓ ScoreManager found! Adding score...");
            scoreManager.IncreaseScore(1);
            Debug.Log($"[JellyBuddy] Score added! Current score: {scoreManager.GetScore()}");
        }
        else
        {
            Debug.LogError("[JellyBuddy] ✗ ScoreManager NOT FOUND in scene! Make sure you have a GameObject with ScoreManager script in the scene.");
        }
        
        // Trigger visual effect
        Debug.Log("[JellyBuddy] Triggering star collection visual effect...");
        TriggerStarCollectionEffect();
        
        // Destroy the star
        Debug.Log($"[JellyBuddy] Destroying star: {star.name}");
        Destroy(star);
        
        Debug.Log("[JellyBuddy] ✓ Star collection complete!");
    }
    
    /// <summary>
    /// Triggers the star collection visual effect (glow/scale)
    /// </summary>
    private void TriggerStarCollectionEffect()
    {
        isCollectingStarEffect = true;
        starCollectionTimer = 0f;
    }
    
    /// <summary>
    /// Updates the star collection effect animation
    /// </summary>
    private void UpdateStarCollectionEffect()
    {
        starCollectionTimer += Time.deltaTime;
        float progress = starCollectionTimer / starCollectionDuration;
        
        if (progress < 0.5f)
        {
            // First half: scale up (glow effect)
            float t = progress * 2f;
            Vector3 targetScaleEffect = originalScale * starCollectionScaleBoost;
            transform.localScale = Vector3.SmoothDamp(transform.localScale, targetScaleEffect, ref starCollectionVelocity, starCollectionDuration * 0.25f);
        }
        else
        {
            // Second half: return to normal
            float t = (progress - 0.5f) * 2f;
            transform.localScale = Vector3.SmoothDamp(transform.localScale, originalScale, ref starCollectionVelocity, starCollectionDuration * 0.25f);
        }
        
        // End effect
        if (progress >= 1f)
        {
            isCollectingStarEffect = false;
            transform.localScale = originalScale;
        }
    }
    
    /// <summary>
    /// Applies a subtle torque to create jelly-like rotation
    /// </summary>
    private void ApplyJellyRotation()
    {
        // Update timer
        torqueTimer += Time.fixedDeltaTime;
        
        // Change torque direction periodically for more organic movement
        if (torqueTimer >= torqueChangeInterval)
        {
            torqueTimer = 0f;
            
            // Random torque with random direction
            float torqueMagnitude = Random.Range(minTorque, maxTorque);
            currentTorque = torqueMagnitude * (Random.value > 0.5f ? 1f : -1f);
        }
        
        // Apply the torque
        rb2D.AddTorque(currentTorque * Time.fixedDeltaTime);
    }
    
    /// <summary>
    /// Applies a subtle floating oscillation effect
    /// </summary>
    private void ApplyFloatingEffect()
    {
        // Create a sine wave for smooth up/down floating motion
        float floatForce = Mathf.Sin((Time.time * floatSpeed) + floatOffset) * floatStrength;
        
        // Apply vertical force
        rb2D.AddForce(Vector2.up * floatForce, ForceMode2D.Force);
    }
    
    /// <summary>
    /// Applies damping to angular velocity to prevent excessive spinning
    /// </summary>
    private void ApplyRotationDamping()
    {
        // Gradually reduce angular velocity for smoother, more controlled rotation
        rb2D.angularVelocity *= rotationDamping;
    }
    
    /// <summary>
    /// Public method to adjust jelly effect intensity at runtime
    /// </summary>
    public void SetJellyIntensity(float intensity)
    {
        intensity = Mathf.Clamp01(intensity);
        maxTorque = 2f * intensity;
        minTorque = 0.5f * intensity;
        floatStrength = 0.3f * intensity;
        breathingIntensity = 0.05f * intensity;
    }
    
    /// <summary>
    /// Public method to enable/disable floating effect
    /// </summary>
    public void SetFloatingEnabled(bool enabled)
    {
        enableFloatingOscillation = enabled;
    }
    
    /// <summary>
    /// Public method to manually trigger squash effect
    /// </summary>
    public void ManualSquash(bool vertical = true)
    {
        if (vertical)
        {
            targetScale = new Vector3(
                originalScale.x * (1f + squashAmount),
                originalScale.y * (1f - squashAmount),
                originalScale.z
            );
        }
        else
        {
            targetScale = new Vector3(
                originalScale.x * (1f - squashAmount),
                originalScale.y * (1f + squashAmount),
                originalScale.z
            );
        }
        
        isSquashing = true;
        squashTimer = 0f;
    }
}
