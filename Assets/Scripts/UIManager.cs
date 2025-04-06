using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    [Header("Game Panels")]
    [SerializeField] private GameObject gamePanel;
    [SerializeField] private GameObject gameOverPanel;
    [SerializeField] private GameObject hintPanel;
    [SerializeField] private GameObject adPanel;
    [SerializeField] private GameObject shopPanel;
    
    [Header("Game UI Elements")]
    [SerializeField] private Text gameStatusText;
    [SerializeField] private Text scoreText;
    [SerializeField] private Text diamondText;
    [SerializeField] private Text hintText;
    [SerializeField] private Text winnerText;
    
    [Header("Buttons")]
    [SerializeField] private Button hintButton;
    [SerializeField] private Button watchAdButton;
    [SerializeField] private Button restartButton;
    [SerializeField] private Button mainMenuButton;
    [SerializeField] private Button closeShopButton;
    [SerializeField] private Button closeAdButton;
    [SerializeField] private Button closeHintButton;
    
    [Header("References")]
    [SerializeField] private GameManager gameManager;
    [SerializeField] private HintManager hintManager;
    
    private Dictionary<GameObject, Vector3> panelStartPositions = new Dictionary<GameObject, Vector3>();
    
    private void Awake()
    {
        if (gameManager == null)
        {
            gameManager = FindObjectOfType<GameManager>();
        }
        
        if (hintManager == null)
        {
            hintManager = FindObjectOfType<HintManager>();
        }
    }
    
    private void Start()
    {
        // Store initial positions for animations
        if (gamePanel != null) panelStartPositions[gamePanel] = gamePanel.transform.position;
        if (gameOverPanel != null) panelStartPositions[gameOverPanel] = gameOverPanel.transform.position;
        if (hintPanel != null) panelStartPositions[hintPanel] = hintPanel.transform.position;
        if (adPanel != null) panelStartPositions[adPanel] = adPanel.transform.position;
        if (shopPanel != null) panelStartPositions[shopPanel] = shopPanel.transform.position;
        
        // Set up button listeners
        if (hintButton != null) hintButton.onClick.AddListener(OnHintButtonClicked);
        if (watchAdButton != null) watchAdButton.onClick.AddListener(OnWatchAdButtonClicked);
        if (restartButton != null) restartButton.onClick.AddListener(OnRestartButtonClicked);
        if (mainMenuButton != null) mainMenuButton.onClick.AddListener(OnMainMenuButtonClicked);
        if (closeShopButton != null) closeShopButton.onClick.AddListener(() => HidePanel(shopPanel));
        if (closeAdButton != null) closeAdButton.onClick.AddListener(() => HidePanel(adPanel));
        if (closeHintButton != null) closeHintButton.onClick.AddListener(() => HidePanel(hintPanel));
        
        // Hide panels initially
        if (gameOverPanel != null) gameOverPanel.SetActive(false);
        if (hintPanel != null) hintPanel.SetActive(false);
        if (adPanel != null) adPanel.SetActive(false);
        if (shopPanel != null) shopPanel.SetActive(false);
        
        UpdateDiamondDisplay();
    }
    
    public void UpdateGameStatus(string status)
    {
        if (gameStatusText != null)
        {
            gameStatusText.text = status;
        }
    }
    
    public void UpdateScoreText(List<Player> players)
    {
        if (scoreText != null)
        {
            string scoreString = "";
            foreach (Player player in players)
            {
                scoreString += $"{player.Name}: {player.totalScore} (Round: {player.roundScore})\n";
            }
            scoreText.text = scoreString;
        }
    }
    
    public void UpdateDiamondDisplay()
    {
        if (diamondText != null && hintManager != null)
        {
            diamondText.text = hintManager.Diamonds.ToString();
        }
    }
    
    public void ShowGameOver(string winnerName, int winnerScore)
    {
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(true);
            
            // Animate panel
            Animation anim = gameOverPanel.GetComponent<Animation>();
            if (anim != null) anim.Play();
            
            if (winnerText != null)
            {
                winnerText.text = $"{winnerName} wins with {winnerScore} points!";
            }
        }
    }
    
    public void ShowHintMessage(string message)
    {
        if (hintPanel != null && hintText != null)
        {
            hintPanel.SetActive(true);
            hintText.text = message;
            
            // Auto-hide after a few seconds
            Invoke("HideHintPanel", 3f);
        }
    }
    
    private void HideHintPanel()
    {
        if (hintPanel != null)
        {
            hintPanel.SetActive(false);
        }
    }
    
    private void ShowPanel(GameObject panel)
    {
        if (panel != null)
        {
            panel.SetActive(true);
            
            // Animate panel
            Animation anim = panel.GetComponent<Animation>();
            if (anim != null) anim.Play();
        }
    }
    
    private void HidePanel(GameObject panel)
    {
        if (panel != null)
        {
            panel.SetActive(false);
        }
    }
    
    private void OnHintButtonClicked()
    {
        if (gameManager != null)
        {
            gameManager.ShowHint();
        }
    }
    
    private void OnWatchAdButtonClicked()
    {
        if (gameManager != null)
        {
            gameManager.WatchAd();
        }
    }
    
    private void OnRestartButtonClicked()
    {
        if (gameManager != null)
        {
            gameManager.RestartGame();
        }
    }
    
    private void OnMainMenuButtonClicked()
    {
        SceneManager.LoadScene("MainMenu");
    }
} 