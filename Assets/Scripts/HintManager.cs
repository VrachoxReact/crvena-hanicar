using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

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
    [SerializeField] private int diamondPackPrice = 0.99f;
    [SerializeField] private int diamondsInPack = 20;
    
    private int diamonds;
    private GameManager gameManager;
    private UnityAction onHintRequested;
    
    public int Diamonds => diamonds;
    
    private void Awake()
    {
        gameManager = FindObjectOfType<GameManager>();
        
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
} 