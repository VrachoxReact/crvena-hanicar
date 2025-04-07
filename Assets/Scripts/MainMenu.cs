using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private Button playButton;
    [SerializeField] private Button rulesButton;
    [SerializeField] private Button shopButton;
    [SerializeField] private Button settingsButton;
    [SerializeField] private Button quitButton;
    
    [SerializeField] private GameObject rulesPanel;
    [SerializeField] private GameObject shopPanel;
    [SerializeField] private GameObject settingsPanel;
    
    [SerializeField] private Button closeRulesButton;
    [SerializeField] private Button closeShopButton;
    [SerializeField] private Button closeSettingsButton;
    
    [SerializeField] private Button buyDiamondsButton;
    [SerializeField] private Button watchAdForDiamondsButton;
    
    [SerializeField] private Text diamondText;
    
    private HintManager hintManager;
    private AudioSource musicPlayer;
    
    private void Start()
    {
        hintManager = FindFirstObjectByType<HintManager>();
        
        if (musicPlayer == null)
        {
            musicPlayer = FindFirstObjectByType<AudioSource>();
        }
        
        // Initialize UI elements
        playButton.onClick.AddListener(StartGame);
        rulesButton.onClick.AddListener(() => ShowPanel(rulesPanel));
        shopButton.onClick.AddListener(() => ShowPanel(shopPanel));
        settingsButton.onClick.AddListener(() => ShowPanel(settingsPanel));
        quitButton.onClick.AddListener(QuitGame);
        
        closeRulesButton.onClick.AddListener(() => HidePanel(rulesPanel));
        closeShopButton.onClick.AddListener(() => HidePanel(shopPanel));
        closeSettingsButton.onClick.AddListener(() => HidePanel(settingsPanel));
        
        buyDiamondsButton.onClick.AddListener(BuyDiamonds);
        watchAdForDiamondsButton.onClick.AddListener(WatchAdForDiamonds);
        
        // Hide all panels initially
        rulesPanel.SetActive(false);
        shopPanel.SetActive(false);
        settingsPanel.SetActive(false);
        
        UpdateDiamondDisplay();
    }
    
    private void StartGame()
    {
        // Load game scene
        SceneManager.LoadScene("GameScene");
    }
    
    private void ShowPanel(GameObject panel)
    {
        // Hide all panels first
        rulesPanel.SetActive(false);
        shopPanel.SetActive(false);
        settingsPanel.SetActive(false);
        
        // Show the requested panel
        panel.SetActive(true);
    }
    
    private void HidePanel(GameObject panel)
    {
        panel.SetActive(false);
    }
    
    private void BuyDiamonds()
    {
        if (hintManager != null)
        {
            hintManager.BuyDiamondPack();
            UpdateDiamondDisplay();
        }
    }
    
    private void WatchAdForDiamonds()
    {
        if (hintManager != null)
        {
            // This would connect to ad system
            Debug.Log("Showing ad for diamonds...");
            
            // Simulate ad watching
            hintManager.ShowHintText("Thanks for watching! You received 3 diamonds.");
            UpdateDiamondDisplay();
        }
    }
    
    private void UpdateDiamondDisplay()
    {
        if (hintManager != null && diamondText != null)
        {
            diamondText.text = hintManager.Diamonds.ToString();
        }
    }
    
    private void QuitGame()
    {
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #else
        Application.Quit();
        #endif
    }
} 