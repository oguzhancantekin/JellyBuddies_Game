using UnityEngine;

/// <summary>
/// Star Spawner for Jelly Buddies
/// Spawns stars infinitely at random positions from the top of the screen
/// Automatically destroys stars that fall below the screen
/// </summary>
public class StarSpawner : MonoBehaviour
{
    [Header("Spawn Settings")]
    [Tooltip("Prefab of the star to spawn")]
    [SerializeField] private GameObject starPrefab;
    
    [Tooltip("Time between each star spawn (in seconds)")]
    [SerializeField] private float spawnRate = 1.5f;
    
    [Header("Spawn Position")]
    [Tooltip("Minimum X position for star spawn")]
    [SerializeField] private float minX = 15f;
    
    [Tooltip("Maximum X position for star spawn")]
    [SerializeField] private float maxX = 22f;
    
    [Tooltip("Y position where stars spawn (top of screen)")]
    [SerializeField] private float spawnY = 20f;
    
    [Header("Star Physics")]
    [Tooltip("Falling speed of the stars (gravity scale)")]
    [SerializeField] private float starFallSpeed = 1f;
    
    [Header("Cleanup Settings")]
    [Tooltip("Y position where stars are destroyed (bottom of screen)")]
    [SerializeField] private float destroyY = -6f;
    
    [Tooltip("Enable automatic cleanup of fallen stars")]
    [SerializeField] private bool enableAutoCleanup = true;
    
    // Internal variables
    private float spawnTimer = 0f;
    
    private void Start()
    {
        // Validate star prefab
        if (starPrefab == null)
        {
            Debug.LogError("StarSpawner: Star Prefab is not assigned! Please assign a star prefab in the Inspector.");
        }
        else
        {
            Debug.Log($"StarSpawner: Initialized. Spawn rate: {spawnRate}s, Fall speed: {starFallSpeed}");
        }
    }
    
    private void Update()
    {
        // Update spawn timer
        spawnTimer += Time.deltaTime;
        
        // Check if it's time to spawn a new star
        if (spawnTimer >= spawnRate)
        {
            SpawnStar();
            spawnTimer = 0f; // Reset timer
        }
        
        // Cleanup fallen stars
        if (enableAutoCleanup)
        {
            CleanupFallenStars();
        }
    }
    
    /// <summary>
    /// Spawns a star at a random X position at the top of the screen
    /// </summary>
    private void SpawnStar()
    {
        if (starPrefab == null)
        {
            Debug.LogWarning("StarSpawner: Cannot spawn star - prefab is null!");
            return;
        }
        
        // Generate random X position
        float randomX = Random.Range(minX, maxX);
        Vector3 spawnPosition = new Vector3(randomX, spawnY, 0f);
        
        Debug.Log($"[StarSpawner] Attempting to spawn star at: X={randomX:F2}, Y={spawnY}, Z=0");
        
        // Instantiate the star
        GameObject newStar = Instantiate(starPrefab, spawnPosition, Quaternion.identity);
        
        // FORCE Z position to 0 (ensure it's in front of camera)
        newStar.transform.position = new Vector3(newStar.transform.position.x, newStar.transform.position.y, 0f);
        
        Debug.Log($"[StarSpawner] Star created: {newStar.name} at actual position: {newStar.transform.position}");
        
        // Set up Rigidbody2D for falling
        Rigidbody2D rb = newStar.GetComponent<Rigidbody2D>();
        if (rb == null)
        {
            rb = newStar.AddComponent<Rigidbody2D>();
            Debug.Log("[StarSpawner] Added Rigidbody2D to star");
        }
        
        // Configure Rigidbody2D
        rb.gravityScale = starFallSpeed;
        rb.bodyType = RigidbodyType2D.Dynamic;
        
        Debug.Log($"[StarSpawner] ✓ Star fully configured - Position: {newStar.transform.position}, Gravity: {starFallSpeed}");
    }
    
    /// <summary>
    /// Destroys stars that have fallen below the screen
    /// </summary>
    private void CleanupFallenStars()
    {
        // Find all stars in the scene
        GameObject[] stars = GameObject.FindGameObjectsWithTag("Star");
        
        foreach (GameObject star in stars)
        {
            // Check if star has fallen below destroy threshold
            if (star.transform.position.y < destroyY)
            {
                Debug.Log($"Cleaning up fallen star: {star.name} at Y: {star.transform.position.y}");
                Destroy(star);
            }
        }
    }
    
    /// <summary>
    /// Public method to change spawn rate at runtime
    /// </summary>
    public void SetSpawnRate(float newRate)
    {
        spawnRate = Mathf.Max(0.1f, newRate); // Minimum 0.1 seconds
        Debug.Log($"Spawn rate changed to: {spawnRate}s");
    }
    
    /// <summary>
    /// Public method to change fall speed at runtime
    /// </summary>
    public void SetFallSpeed(float newSpeed)
    {
        starFallSpeed = Mathf.Max(0.1f, newSpeed);
        Debug.Log($"Fall speed changed to: {starFallSpeed}");
    }
    
    /// <summary>
    /// Stops spawning stars
    /// </summary>
    public void StopSpawning()
    {
        enabled = false;
        Debug.Log("Star spawning stopped");
    }
    
    /// <summary>
    /// Resumes spawning stars
    /// </summary>
    public void StartSpawning()
    {
        enabled = true;
        spawnTimer = 0f;
        Debug.Log("Star spawning started");
    }
    
    /// <summary>
    /// Clears all stars from the scene
    /// </summary>
    public void ClearAllStars()
    {
        GameObject[] stars = GameObject.FindGameObjectsWithTag("Star");
        foreach (GameObject star in stars)
        {
            Destroy(star);
        }
        Debug.Log($"Cleared {stars.Length} stars from the scene");
    }
}
