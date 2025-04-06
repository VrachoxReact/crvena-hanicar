using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class OmniscientBot : MonoBehaviour
{
    private GameManager gameManager;
    private List<Player> allPlayers = new List<Player>();
    
    private void Awake()
    {
        gameManager = GetComponent<GameManager>();
    }
    
    public void SetPlayers(List<Player> players)
    {
        allPlayers = players;
    }
    
    public Card GetBestCardToPlay(Player humanPlayer, List<Card> playableCards, CardSuit leadSuit, List<Card> currentTrick)
    {
        if (playableCards.Count == 0) return null;
        
        // If first player in trick
        if (currentTrick.Count == 0)
        {
            return GetBestLeadCard(humanPlayer, playableCards);
        }
        // If following suit
        else
        {
            return GetBestFollowCard(humanPlayer, playableCards, leadSuit, currentTrick);
        }
    }
    
    private Card GetBestLeadCard(Player humanPlayer, List<Card> playableCards)
    {
        // When leading, prefer:
        // 1. Non-heart cards to avoid collecting points
        // 2. Cards that are unlikely to win the trick
        
        // First, try to play a non-heart card
        List<Card> nonHeartCards = playableCards.Where(card => card.suit != CardSuit.Hearts).ToList();
        
        if (nonHeartCards.Count > 0)
        {
            // Prefer lowest rank cards from suits where other players likely have higher cards
            Dictionary<CardSuit, float> suitRisks = CalculateSuitRisks(humanPlayer, nonHeartCards.Select(c => c.suit).Distinct().ToList());
            
            // Find the suit with highest risk (others likely have higher cards)
            CardSuit riskiestSuit = suitRisks.OrderByDescending(pair => pair.Value).First().Key;
            
            // Get lowest card from riskiest suit
            List<Card> cardsOfRiskiestSuit = nonHeartCards.Where(c => c.suit == riskiestSuit).ToList();
            if (cardsOfRiskiestSuit.Count > 0)
            {
                return cardsOfRiskiestSuit.OrderBy(c => c.GetStrengthValue()).First();
            }
        }
        
        // If only hearts available, play lowest heart
        return playableCards.OrderBy(c => c.GetStrengthValue()).First();
    }
    
    private Card GetBestFollowCard(Player humanPlayer, List<Card> playableCards, CardSuit leadSuit, List<Card> currentTrick)
    {
        // When following:
        // 1. If can't win trick or winning would be bad (hearts in trick), play lowest card
        // 2. If can win and no/low points in trick, try to win with lowest winning card
        
        // Check if this is the last player in the trick
        bool isLastPlayer = currentTrick.Count == allPlayers.Count - 1;
        
        // Calculate total points in current trick
        int pointsInTrick = currentTrick.Sum(card => card.pointValue);
        
        // Find the currently winning card
        Card winningCard = currentTrick[0];
        for (int i = 1; i < currentTrick.Count; i++)
        {
            if (currentTrick[i].suit == leadSuit && 
                currentTrick[i].GetStrengthValue() > winningCard.GetStrengthValue())
            {
                winningCard = currentTrick[i];
            }
        }
        
        // Check if any of our cards can win the trick
        List<Card> cardsOfLeadSuit = playableCards.Where(c => c.suit == leadSuit).ToList();
        List<Card> winningCards = cardsOfLeadSuit.Where(c => c.GetStrengthValue() > winningCard.GetStrengthValue()).ToList();
        
        // If this is the last player and there are points in the trick, try not to win
        if (isLastPlayer && pointsInTrick > 0)
        {
            // If we have cards of lead suit but none can win, play the highest one
            if (cardsOfLeadSuit.Count > 0 && winningCards.Count == 0)
            {
                return cardsOfLeadSuit.OrderByDescending(c => c.GetStrengthValue()).First();
            }
            
            // If we must win (only have winning cards of lead suit), play lowest winner
            if (cardsOfLeadSuit.Count > 0 && cardsOfLeadSuit.Count == winningCards.Count)
            {
                return winningCards.OrderBy(c => c.GetStrengthValue()).First();
            }
            
            // If we have non-winning cards, play highest non-winner
            List<Card> nonWinningCards = cardsOfLeadSuit.Except(winningCards).ToList();
            if (nonWinningCards.Count > 0)
            {
                return nonWinningCards.OrderByDescending(c => c.GetStrengthValue()).First();
            }
            
            // If we don't have lead suit, discard highest heart or high card
            if (cardsOfLeadSuit.Count == 0)
            {
                List<Card> hearts = playableCards.Where(c => c.suit == CardSuit.Hearts).ToList();
                if (hearts.Count > 0)
                {
                    return hearts.OrderByDescending(c => c.GetStrengthValue()).First();
                }
                
                // Otherwise discard highest card
                return playableCards.OrderByDescending(c => c.GetStrengthValue()).First();
            }
        }
        
        // If last player and no points in trick, try to win
        if (isLastPlayer && pointsInTrick == 0)
        {
            if (winningCards.Count > 0)
            {
                return winningCards.OrderBy(c => c.GetStrengthValue()).First();
            }
        }
        
        // If not last player:
        // If must follow suit but can't win, play lowest
        if (cardsOfLeadSuit.Count > 0 && winningCards.Count == 0)
        {
            return cardsOfLeadSuit.OrderBy(c => c.GetStrengthValue()).First();
        }
        
        // If can win and there are no hearts in the trick, consider winning
        if (winningCards.Count > 0 && !currentTrick.Any(c => c.suit == CardSuit.Hearts))
        {
            // Win with lowest winning card
            return winningCards.OrderBy(c => c.GetStrengthValue()).First();
        }
        
        // Default: Play lowest card
        return playableCards.OrderBy(c => c.GetStrengthValue()).First();
    }
    
    private Dictionary<CardSuit, float> CalculateSuitRisks(Player humanPlayer, List<CardSuit> suits)
    {
        Dictionary<CardSuit, float> risks = new Dictionary<CardSuit, float>();
        
        foreach (CardSuit suit in suits)
        {
            // Calculate how many higher cards of this suit are in other players' hands
            int totalHigherCards = 0;
            
            // Find our highest card of this suit
            Card highestCardOfSuit = humanPlayer.hand.Where(c => c.suit == suit)
                                              .OrderByDescending(c => c.GetStrengthValue())
                                              .FirstOrDefault();
            
            if (highestCardOfSuit != null)
            {
                // Count cards higher than our highest in each player's hand
                foreach (Player player in allPlayers)
                {
                    if (player != humanPlayer)
                    {
                        totalHigherCards += player.hand.Count(c => 
                            c.suit == suit && c.GetStrengthValue() > highestCardOfSuit.GetStrengthValue());
                    }
                }
            }
            
            // Risk score based on how many higher cards are out there
            risks[suit] = totalHigherCards;
        }
        
        return risks;
    }
} 