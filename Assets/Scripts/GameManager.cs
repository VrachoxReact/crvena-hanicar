using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using System.Collections;

public class GameManager : MonoBehaviour
{
    [Header("Game References")]
    [SerializeField] private Deck deck;
    [SerializeField] private Player humanPlayer;
    [SerializeField] private Player westBot;
    [SerializeField] private Player eastBot;
    [SerializeField] private Player northBot;
    [SerializeField] private OmniscientBot omniscientBot;
    
    [Header("UI References")]
    [SerializeField] private UIManager uiManager;
    [SerializeField] private HintManager hintManager;
    [SerializeField] private VisualEffects visualEffects;
    [SerializeField] private TutorialManager tutorialManager;
    
    [Header("UI Elements")]
    [SerializeField] private Text gameStatusText;
    [SerializeField] private Text scoreText;
    [SerializeField] private Button hintButton;
    [SerializeField] private GameObject gameOverPanel;
    [SerializeField] private GameObject adButton;
    [SerializeField] private Text winnerText;
    
    [Header("Game Settings")]
    [SerializeField] private int cardsPerPlayer = 8;
    [SerializeField] private int gameEndScore = 51;
    [SerializeField] private int diamondCost = 5; // Used in hint cost calculation
    [SerializeField] private int startingDiamonds = 10;
    [SerializeField] private bool enableTutorial = true; // Controls if tutorial starts automatically
    
    private List<Player> players = new List<Player>();
    private List<Card> currentTrick = new List<Card>();
    private int currentPlayerIndex = 0;
    private int dealerIndex = 0;
    private int turnNumber = 0;
    private bool isGameOver = false;
    private bool isGamePaused = false;
    private CardSuit leadSuit;
    private int diamonds;
    
    // Highlighted card for tutorial
    private CardVisual highlightedCard;
    
    private void Awake()
    {
        // Find required components if not set in inspector
        if (uiManager == null) uiManager = FindFirstObjectByType<UIManager>();
        if (hintManager == null) hintManager = FindFirstObjectByType<HintManager>();
        if (visualEffects == null) visualEffects = FindFirstObjectByType<VisualEffects>();
        if (tutorialManager == null) tutorialManager = FindFirstObjectByType<TutorialManager>();
        if (omniscientBot == null) omniscientBot = FindFirstObjectByType<OmniscientBot>();
    }
    
    private void Start()
    {
        InitializeGame();
        
        // Start tutorial if enabled in settings
        if (enableTutorial && tutorialManager != null)
        {
            tutorialManager.StartTutorial();
        }
    }
    
    private void InitializeGame()
    {
        players.Clear();
        players.Add(humanPlayer);
        players.Add(westBot);
        players.Add(eastBot);
        players.Add(northBot);
        
        // Set players for OmniscientBot
        if (omniscientBot != null)
        {
            omniscientBot.SetPlayers(players);
        }
        
        diamonds = startingDiamonds;
        UpdateDiamondsUI();
        
        if (hintManager != null)
        {
            hintManager.RegisterHintCallback(ShowHint);
        }
        
        if (uiManager != null)
        {
            uiManager.UpdateGameStatus("Game Starting...");
        }
        
        // Hide game over panel
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(false);
        }
        
        dealerIndex = Random.Range(0, players.Count);
        StartNewRound();
    }
    
    private void StartNewRound()
    {
        currentTrick.Clear();
        turnNumber = 0;
        isGameOver = false;
        
        // Clear players' hands
        foreach (Player player in players)
        {
            player.ClearHand();
        }
        
        // Initialize and shuffle deck
        deck.Initialize();
        
        // Deal cards
        for (int i = 0; i < cardsPerPlayer; i++)
        {
            foreach (Player player in players)
            {
                Card card = deck.DrawCard();
                if (card != null)
                {
                    player.AddCardToHand(card);
                }
            }
        }
        
        // Determine first player (after dealer)
        currentPlayerIndex = (dealerIndex + 1) % players.Count;
        
        UpdateUI();
        
        // If the current player is AI, make them play automatically after a delay
        if (players[currentPlayerIndex].playerType != PlayerType.Human)
        {
            StartCoroutine(PlayBotCardWithDelay(1.0f));
        }
    }
    
    private IEnumerator PlayBotCardWithDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        
        if (!isGamePaused)
        {
            PlayBotCard();
        }
    }
    
    public void OnCardClicked(Card card)
    {
        // Only process card clicks if it's the human player's turn
        if (currentPlayerIndex != 0 || isGameOver || isGamePaused) return;
        
        // Check if the card is playable according to rules
        if (IsCardPlayable(card))
        {
            PlayCard(card);
        }
        else
        {
            Debug.Log("Card is not playable!");
            // Give feedback to player
            if (uiManager != null)
            {
                uiManager.ShowHintMessage("This card cannot be played now.");
            }
            
            // Shake the card to indicate it's not playable
            if (visualEffects != null)
            {
                StartCoroutine(visualEffects.ShakeCard(card.transform));
            }
        }
    }
    
    private bool IsCardPlayable(Card card)
    {
        // Rule: Can't play red cards in first two turns
        if (turnNumber <= 2 && card.isRed)
        {
            return false;
        }
        
        // First player can play any valid card
        if (currentTrick.Count == 0)
        {
            return true;
        }
        
        // Must follow suit if possible
        CardSuit leadSuit = currentTrick[0].suit;
        bool hasSameSuit = humanPlayer.hand.Any(c => c.suit == leadSuit);
        
        if (hasSameSuit)
        {
            return card.suit == leadSuit;
        }
        
        // Can play any card if no cards of lead suit
        return true;
    }
    
    private void PlayCard(Card card)
    {
        Player currentPlayer = players[currentPlayerIndex];
        
        // If first card in trick, set lead suit
        if (currentTrick.Count == 0)
        {
            leadSuit = card.suit;
        }
        
        // Remove card from player's hand and add to trick
        currentPlayer.hand.Remove(card);
        currentTrick.Add(card);
        
        // Animate card to center of table
        if (visualEffects != null)
        {
            Vector3 targetPosition = new Vector3(0, 0, 0);
            StartCoroutine(visualEffects.MoveCard(card.transform, targetPosition));
        }
        else
        {
            // Fallback if no visual effects
            card.transform.position = new Vector3(0, 0, 0);
        }
        
        // Flip card face up if it was a bot's card
        if (currentPlayer.playerType != PlayerType.Human)
        {
            // Animate card flip
            if (visualEffects != null)
            {
                StartCoroutine(visualEffects.FlipCard(card, true));
            }
            else
            {
                card.Flip();
            }
        }
        
        // Check if the trick is complete
        if (currentTrick.Count == players.Count)
        {
            // Process completed trick after a delay
            StartCoroutine(ProcessTrickWithDelay(1.5f));
        }
        else
        {
            // Move to next player
            currentPlayerIndex = (currentPlayerIndex + 1) % players.Count;
            UpdateUI();
            
            // If next player is AI, make them play automatically after a delay
            if (players[currentPlayerIndex].playerType != PlayerType.Human)
            {
                StartCoroutine(PlayBotCardWithDelay(1.0f));
            }
        }
    }
    
    private void PlayBotCard()
    {
        Player currentPlayer = players[currentPlayerIndex];
        Card cardToPlay = currentPlayer.PlayCard(leadSuit, turnNumber, turnNumber > 2);
        
        if (cardToPlay != null)
        {
            PlayCard(cardToPlay);
        }
    }
    
    private IEnumerator ProcessTrickWithDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        ProcessTrick();
    }
    
    private void ProcessTrick()
    {
        if (isGamePaused) return;
        
        turnNumber++;
        
        // Calculate points from the trick
        int trickPoints = CalculateTrickPoints();
        
        // Determine the winner of the trick
        int winnerIndex = DetermineTrickWinner();
        Player winner = players[winnerIndex];
        
        // Add points to the winner
        winner.roundScore += trickPoints;
        
        // Show points effect
        if (visualEffects != null && trickPoints > 0)
        {
            Color pointColor = trickPoints >= 3 ? Color.red : Color.yellow;
            visualEffects.ShowPointsGain(winner.transform.position, trickPoints, pointColor);
        }
        
        // Clear trick and update UI
        foreach (Card card in currentTrick)
        {
            deck.DiscardCard(card);
        }
        currentTrick.Clear();
        
        // Check if round is over (all cards played)
        bool roundOver = players.All(p => p.hand.Count == 0);
        
        if (roundOver)
        {
            ProcessRoundEnd();
        }
        else
        {
            // Winner leads next trick
            currentPlayerIndex = winnerIndex;
            UpdateUI();
            
            // If next player is AI, make them play automatically
            if (players[currentPlayerIndex].playerType != PlayerType.Human)
            {
                StartCoroutine(PlayBotCardWithDelay(1.0f));
            }
        }
    }
    
    private int CalculateTrickPoints()
    {
        // Each round starts with 10 points
        // Hearts are worth 1 point, Ace of Hearts is 3 points
        int points = 0;
        foreach (Card card in currentTrick)
        {
            if (card.suit == CardSuit.Hearts)
            {
                points += (card.rank == CardRank.Ace) ? 3 : 1;
            }
        }
        return points;
    }
    
    private int DetermineTrickWinner()
    {
        Card leadCard = currentTrick[0];
        CardSuit leadSuit = leadCard.suit;
        Card highestCard = leadCard;
        int winnerIndex = 0;
        
        for (int i = 1; i < currentTrick.Count; i++)
        {
            Card currentCard = currentTrick[i];
            
            // Only cards of the lead suit can win
            if (currentCard.suit == leadSuit)
            {
                // Higher rank wins (7, 8, 9, 10, Jack, Queen, King, Ace)
                if (currentCard.GetStrengthValue() > highestCard.GetStrengthValue())
                {
                    highestCard = currentCard;
                    winnerIndex = i;
                }
            }
        }
        
        int actualWinnerIndex = (currentPlayerIndex + winnerIndex) % players.Count;
        
        if (uiManager != null)
        {
            uiManager.ShowHintMessage($"{players[actualWinnerIndex].Name} wins the trick!");
        }
        
        return actualWinnerIndex;
    }
    
    private void ProcessRoundEnd()
    {
        // Update all player scores
        foreach (Player player in players)
        {
            player.UpdateScore(player.roundScore);
        }
        
        // Check for game over
        isGameOver = players.Any(p => p.totalScore >= gameEndScore);
        
        UpdateUI();
        
        if (isGameOver)
        {
            ShowGameOver();
        }
        else
        {
            // Determine who shuffles next (worst player from previous round)
            dealerIndex = players.OrderByDescending(p => p.roundScore).First().playerType == PlayerType.Human ? 0 : 
                         players.IndexOf(players.OrderByDescending(p => p.roundScore).First());
            
            // Start new round after delay
            StartCoroutine(StartNewRoundWithDelay(2.0f));
        }
    }
    
    private IEnumerator StartNewRoundWithDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        StartNewRound();
    }
    
    private void ShowGameOver()
    {
        // Find player with lowest score (winner)
        Player winner = players.OrderBy(p => p.totalScore).First();
        
        if (uiManager != null)
        {
            uiManager.ShowGameOver(winner.Name, winner.totalScore);
        }
        
        // Show win effect
        if (visualEffects != null)
        {
            visualEffects.ShowWinEffect(winner.transform.position);
        }
    }
    
    private void UpdateUI()
    {
        if (uiManager != null)
        {
            uiManager.UpdateGameStatus($"Turn: {players[currentPlayerIndex].Name}");
            uiManager.UpdateScoreText(players);
        }
    }
    
    private void UpdateDiamondsUI()
    {
        if (uiManager != null)
        {
            uiManager.UpdateDiamondDisplay();
        }
    }
    
    public void ShowHint()
    {
        if (hintManager != null)
        {
            // Check if they can afford the hint using diamond cost setting
            if (hintManager.Diamonds >= diamondCost)
            {
                string hintMessage = GetHintForCurrentState();
                hintManager.ShowHint(hintMessage, diamondCost);
            }
            else
            {
                if (uiManager != null)
                {
                    uiManager.ShowHintMessage("Not enough diamonds for a hint!");
                }
            }
        }
    }
    
    public void WatchAd()
    {
        Debug.Log("Ad watched - rewarding player");
        
        // Call public method to reward player for watching ad
        if (hintManager != null)
        {
            hintManager.RewardAdCompletion();
        }
    }
    
    private Card GetBestCardHint()
    {
        List<Card> playableCards = new List<Card>();
        
        foreach (Card card in humanPlayer.hand)
        {
            if (IsCardPlayable(card))
            {
                playableCards.Add(card);
            }
        }
        
        if (playableCards.Count == 0)
        {
            return null;
        }
        
        // If we have an omniscient bot, use its logic
        if (omniscientBot != null)
        {
            return omniscientBot.GetBestCardToPlay(humanPlayer, playableCards, leadSuit, currentTrick);
        }
        
        // Fallback to simple logic
        return playableCards.OrderBy(card => card.pointValue).FirstOrDefault();
    }
    
    public void RestartGame()
    {
        // Reset scores
        foreach (Player player in players)
        {
            player.totalScore = 0;
            player.roundScore = 0;
            player.consecutiveZeroRounds = 0;
        }
        
        isGameOver = false;
        
        // Hide game over panel if it's visible
        if (uiManager != null && uiManager.gameOverPanel != null)
        {
            uiManager.gameOverPanel.SetActive(false);
        }
        
        StartNewRound();
    }
    
    public void HighlightCard(CardSuit suit, CardRank rank)
    {
        // Clear previous highlight
        ClearCardHighlights();
        
        // Find the card to highlight
        foreach (Card card in humanPlayer.hand)
        {
            if (card.suit == suit && card.rank == rank)
            {
                CardVisual visual = card.GetComponent<CardVisual>();
                if (visual != null)
                {
                    visual.Highlight(true);
                    highlightedCard = visual;
                }
                break;
            }
        }
    }
    
    public void ClearCardHighlights()
    {
        if (highlightedCard != null)
        {
            highlightedCard.Highlight(false);
            highlightedCard = null;
        }
    }
    
    public void SetGamePaused(bool paused)
    {
        isGamePaused = paused;
    }
    
    // Get a hint message based on the current game state
    private string GetHintForCurrentState()
    {
        // Only show hints if it's the human player's turn
        if (currentPlayerIndex != 0 || isGameOver) 
            return "It's not your turn to play.";
        
        Card bestCard = GetBestCardHint();
        
        if (bestCard != null)
        {
            // Highlight best card
            HighlightCard(bestCard.suit, bestCard.rank);
            
            // Return hint message
            return $"Best card to play: {bestCard.rank} of {bestCard.suit}";
        }
        else
        {
            return "No valid card to play.";
        }
    }
} 