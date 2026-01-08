using UnityEngine;
using TMPro;

/// <summary>
/// Simple Score Manager for Jelly Buddies
/// Manages score tracking and UI display
/// </summary>
public class ScoreManager : MonoBehaviour
{
    [Header("UI Reference")]
    public TextMeshProUGUI scoreText;
    
    [Header("Score Settings")]
    [SerializeField] private int currentScore = 0;
    
    [Header("Audio Settings")]
    public AudioClip collectSound;
    private AudioSource audioSource;
    
    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
        UpdateScoreDisplay();
    }
    
    /// <summary>
    /// Increases the score by a specified amount
    /// </summary>
    /// <param name="amount">Amount to increase score by</param>
    public void IncreaseScore(int amount)
    {
        currentScore += amount;
        UpdateScoreDisplay();
        
        // Play collect sound
        if (audioSource != null && collectSound != null)
        {
            audioSource.PlayOneShot(collectSound);
        }
        
        Debug.Log($"Score increased by {amount}. Total score: {currentScore}");
    }
    
    /// <summary>
    /// Updates the score display on UI
    /// </summary>
    private void UpdateScoreDisplay()
    {
        if (scoreText != null)
        {
            scoreText.text = "Score: " + currentScore;
        }
        else
        {
            Debug.LogWarning("ScoreManager: scoreText is not assigned!");
        }
    }
    
    /// <summary>
    /// Gets the current score value
    /// </summary>
    public int GetScore()
    {
        return currentScore;
    }
    
    /// <summary>
    /// Resets the score to zero
    /// </summary>
    public void ResetScore()
    {
        currentScore = 0;
        UpdateScoreDisplay();
    }
}
