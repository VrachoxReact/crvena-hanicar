using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class VisualEffects : MonoBehaviour
{
    public static VisualEffects Instance { get; private set; }
    
    [Header("Card Animation Settings")]
    [SerializeField] private float cardMoveSpeed = 5f;
    [SerializeField] private float cardRotateSpeed = 540f;
    [SerializeField] private AnimationCurve moveCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
    
    [Header("UI Animation Settings")]
    [SerializeField] private float panelFadeInDuration = 0.3f;
    [SerializeField] private float panelFadeOutDuration = 0.2f;
    
    [Header("Particle Effects")]
    [SerializeField] private GameObject winEffect;
    [SerializeField] private GameObject pointsGainEffect;
    
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    // Card Movement Animation
    public IEnumerator MoveCard(Transform card, Vector3 targetPosition, float duration = 0.5f)
    {
        Vector3 startPosition = card.position;
        float elapsedTime = 0;
        
        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float t = moveCurve.Evaluate(elapsedTime / duration);
            card.position = Vector3.Lerp(startPosition, targetPosition, t);
            yield return null;
        }
        
        card.position = targetPosition;
    }
    
    // Card Flip Animation
    public IEnumerator FlipCard(Card card, bool faceUp, float duration = 0.3f)
    {
        Transform cardTransform = card.transform;
        float startRotation = cardTransform.eulerAngles.y;
        float targetRotation = faceUp ? 180f : 0f;
        float elapsedTime = 0;
        
        while (elapsedTime < duration / 2)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / (duration / 2);
            float currentRotation = Mathf.Lerp(startRotation, 90f, t);
            cardTransform.eulerAngles = new Vector3(0, currentRotation, 0);
            yield return null;
        }
        
        // Toggle card face at the midpoint
        card.Flip();
        
        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float t = (elapsedTime - duration / 2) / (duration / 2);
            float currentRotation = Mathf.Lerp(90f, targetRotation, t);
            cardTransform.eulerAngles = new Vector3(0, currentRotation, 0);
            yield return null;
        }
        
        cardTransform.eulerAngles = new Vector3(0, targetRotation, 0);
    }
    
    // UI Panel Fade Animation
    public IEnumerator FadePanel(CanvasGroup panel, bool fadeIn, float duration = 0.3f)
    {
        float startAlpha = panel.alpha;
        float targetAlpha = fadeIn ? 1f : 0f;
        float elapsedTime = 0;
        
        panel.gameObject.SetActive(true);
        
        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / duration;
            panel.alpha = Mathf.Lerp(startAlpha, targetAlpha, t);
            yield return null;
        }
        
        panel.alpha = targetAlpha;
        
        if (!fadeIn)
        {
            panel.gameObject.SetActive(false);
        }
    }
    
    // Create Points Gain Effect
    public void ShowPointsGain(Vector3 position, int points, Color color)
    {
        if (pointsGainEffect != null)
        {
            GameObject effect = Instantiate(pointsGainEffect, position, Quaternion.identity);
            
            // Set points text
            Text pointsText = effect.GetComponentInChildren<Text>();
            if (pointsText != null)
            {
                pointsText.text = (points > 0 ? "+" : "") + points.ToString();
                pointsText.color = color;
            }
            
            // Auto-destroy after animation
            Destroy(effect, 2f);
        }
    }
    
    // Create Win Effect
    public void ShowWinEffect(Vector3 position)
    {
        if (winEffect != null)
        {
            GameObject effect = Instantiate(winEffect, position, Quaternion.identity);
            
            // Auto-destroy after animation
            Destroy(effect, 3f);
        }
    }
    
    // Button Press Animation
    public IEnumerator AnimateButtonPress(Button button)
    {
        Transform buttonTransform = button.transform;
        Vector3 originalScale = buttonTransform.localScale;
        Vector3 pressedScale = originalScale * 0.9f;
        
        float duration = 0.1f;
        float elapsedTime = 0;
        
        // Scale down
        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / duration;
            buttonTransform.localScale = Vector3.Lerp(originalScale, pressedScale, t);
            yield return null;
        }
        
        // Scale up
        elapsedTime = 0;
        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / duration;
            buttonTransform.localScale = Vector3.Lerp(pressedScale, originalScale, t);
            yield return null;
        }
        
        buttonTransform.localScale = originalScale;
    }
    
    // Shake Card Animation for invalid moves
    public IEnumerator ShakeCard(Transform card, float duration = 0.5f, float magnitude = 5f)
    {
        Vector3 originalPosition = card.position;
        float elapsed = 0f;
        
        while (elapsed < duration)
        {
            float x = Random.Range(-1f, 1f) * magnitude;
            float y = Random.Range(-1f, 1f) * magnitude;
            
            card.position = originalPosition + new Vector3(x, y, 0);
            
            elapsed += Time.deltaTime;
            yield return null;
        }
        
        card.position = originalPosition;
    }
} 