using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections.Generic;

public class HintManager : MonoBehaviour
{
    [SerializeField] private Text diamondText;
    [SerializeField] private Button hintButton;
    [SerializeField] private Button adButton;
    [SerializeField] private GameObject hintPanel;
    [SerializeField] private Text hintText;
    
    [Header("Monetization")]
    [SerializeField] private int hintCost = 5;
    [SerializeField] private int startingDiamonds = 10;
    [SerializeField] private int diamondsPerAd = 3;
    [SerializeField] private float diamondPackPrice = 0.99f;
    [SerializeField] private int diamondsInPack = 20;
    [SerializeField] private int adRewardAmount = 3;
    
    private int diamonds;
    private int consecutiveHints = 0;
    private GameManager gameManager;
    private UnityAction onHintRequested;
    
    public int Diamonds => diamonds;
    
    private void Awake()
    {
        if (gameManager == null)
        {
            gameManager = FindFirstObjectByType<GameManager>();
        }
        
        // Initialize UI elements
        hintButton.onClick.AddListener(RequestHint);
        adButton.onClick.AddListener(WatchAd);
        adButton.gameObject.SetActive(false);
        hintPanel.SetActive(false);
        
        LoadDiamonds();
        UpdateDiamondUI();
    }
    
    public void RegisterHintCallback(UnityAction callback)
    {
        onHintRequested = callback;
    }
    
    private void RequestHint()
    {
        if (diamonds >= hintCost)
        {
            UseDiamonds(hintCost);
            onHintRequested?.Invoke();
        }
        else
        {
            // Not enough diamonds, offer options to get more
            adButton.gameObject.SetActive(true);
        }
    }
    
    public void ShowHint(string hintMessage, int cost)
    {
        // Deduct diamonds
        UseDiamonds(cost);
        
        // Increment consecutive hints counter
        consecutiveHints++;
        
        // Show the hint message
        ShowHintText(hintMessage);
    }
    
    public void ShowHintText(string hint)
    {
        hintPanel.SetActive(true);
        hintText.text = hint;
        
        // Auto-hide after a few seconds
        Invoke("HideHintPanel", 3.0f);
    }
    
    private void HideHintPanel()
    {
        hintPanel.SetActive(false);
    }
    
    private void WatchAd()
    {
        // This would connect to the ad system API
        Debug.Log("Showing ad...");
        
        // Simulate ad completion
        // In a real implementation, this would be called by the ad SDK's callback
        OnAdCompleted();
    }
    
    private void OnAdCompleted()
    {
        // Reward player with diamonds after watching ad
        AddDiamonds(diamondsPerAd);
        adButton.gameObject.SetActive(false);
        
        // Show message
        ShowHintText($"You received {diamondsPerAd} diamonds!");
    }
    
    public void BuyDiamondPack()
    {
        // This would connect to IAP system
        Debug.Log($"Processing purchase of {diamondsInPack} diamonds for ${diamondPackPrice}...");
        
        // Simulate purchase completion
        // In a real implementation, this would be called by the IAP SDK's callback
        OnPurchaseCompleted();
    }
    
    private void OnPurchaseCompleted()
    {
        AddDiamonds(diamondsInPack);
        ShowHintText($"Purchase successful! You received {diamondsInPack} diamonds!");
    }
    
    private void AddDiamonds(int amount)
    {
        diamonds += amount;
        SaveDiamonds();
        UpdateDiamondUI();
    }
    
    private void UseDiamonds(int amount)
    {
        diamonds = Mathf.Max(0, diamonds - amount);
        SaveDiamonds();
        UpdateDiamondUI();
    }
    
    private void UpdateDiamondUI()
    {
        diamondText.text = diamonds.ToString();
    }
    
    private void SaveDiamonds()
    {
        PlayerPrefs.SetInt("Diamonds", diamonds);
        PlayerPrefs.Save();
    }
    
    private void LoadDiamonds()
    {
        if (PlayerPrefs.HasKey("Diamonds"))
        {
            diamonds = PlayerPrefs.GetInt("Diamonds");
        }
        else
        {
            diamonds = startingDiamonds;
            SaveDiamonds();
        }
    }
    
    public int GetHintCost(int consecutiveHints)
    {
        // Exponential cost: 1, 2, 4, 8, 16, etc.
        return (int)Mathf.Pow(2, consecutiveHints);
    }
    
    // Public method to reward watching ads
    public void RewardAdCompletion()
    {
        // Add diamonds for watching an ad
        diamonds += adRewardAmount;
        SaveDiamonds();
        UpdateDiamondUI();
        
        // Reset the consecutive hint counter to make hints cheaper
        consecutiveHints = 0;
        
        if (gameManager != null)
        {
            // Show a hint since they watched an ad
            gameManager.ShowHint();
        }
    }
} 