using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public enum PlayerType
{
    Human,
    IdiotBot,
    MediumBot,
    AdvancedBot,
    OmniscientBot
}

public class Player : MonoBehaviour
{
    [SerializeField] private Transform handTransform;
    [SerializeField] private string playerName;
    
    public PlayerType playerType;
    public List<Card> hand = new List<Card>();
    public int totalScore = 0;
    public int roundScore = 0;
    public int consecutiveZeroRounds = 0;
    
    public string Name => playerName;
    
    public void AddCardToHand(Card card)
    {
        hand.Add(card);
        card.transform.SetParent(handTransform);
        RepositionCardsInHand();
        
        if (playerType == PlayerType.Human)
        {
            card.Flip(); // Only show cards to human player
        }
    }
    
    private void RepositionCardsInHand()
    {
        float cardWidth = 1.5f;
        float totalWidth = hand.Count * cardWidth;
        float startX = -totalWidth / 2;
        
        for (int i = 0; i < hand.Count; i++)
        {
            float x = startX + i * cardWidth;
            hand[i].transform.localPosition = new Vector3(x, 0, 0);
        }
    }
    
    public Card PlayCard(CardSuit leadSuit, int turnNumber, bool canPlayRed)
    {
        if (playerType == PlayerType.Human)
        {
            // Human players select cards through UI interaction
            // This will be handled by GameManager
            return null;
        }
        else
        {
            Card selectedCard = SelectCardForBot(leadSuit, turnNumber, canPlayRed);
            if (selectedCard != null)
            {
                hand.Remove(selectedCard);
                RepositionCardsInHand();
            }
            return selectedCard;
        }
    }
    
    private Card SelectCardForBot(CardSuit leadSuit, int turnNumber, bool canPlayRed)
    {
        List<Card> playableCards = GetPlayableCards(leadSuit, turnNumber, canPlayRed);
        
        if (playableCards.Count == 0)
        {
            return null;
        }
        
        // Different bot logic based on type
        switch (playerType)
        {
            case PlayerType.IdiotBot:
                return SelectRandomCard(playableCards);
                
            case PlayerType.MediumBot:
                return SelectMediumBotCard(playableCards, leadSuit);
                
            case PlayerType.AdvancedBot:
                return SelectAdvancedBotCard(playableCards, leadSuit);
                
            case PlayerType.OmniscientBot:
                return SelectOmniscientBotCard(playableCards, leadSuit);
                
            default:
                return SelectRandomCard(playableCards);
        }
    }
    
    private List<Card> GetPlayableCards(CardSuit leadSuit, int turnNumber, bool canPlayRed)
    {
        // Filter cards based on game rules
        List<Card> playableCards = new List<Card>();
        
        // Rule: Can't play red cards in first two turns
        if (turnNumber <= 2 && !canPlayRed)
        {
            playableCards = hand.Where(card => !card.isRed).ToList();
        }
        // Rule: Must follow suit if possible
        else if (leadSuit != CardSuit.Hearts && leadSuit != CardSuit.Diamonds && 
                 leadSuit != CardSuit.Clubs && leadSuit != CardSuit.Spades)
        {
            // First player in trick can play any card
            playableCards = hand.ToList();
        }
        else
        {
            // Must follow suit if possible
            List<Card> sameSuitCards = hand.Where(card => card.suit == leadSuit).ToList();
            
            if (sameSuitCards.Count > 0)
            {
                playableCards = sameSuitCards;
            }
            else
            {
                // Can play any card if no matching suit
                playableCards = hand.ToList();
            }
        }
        
        return playableCards;
    }
    
    private Card SelectRandomCard(List<Card> playableCards)
    {
        if (playableCards.Count == 0) return null;
        
        int randomIndex = Random.Range(0, playableCards.Count);
        return playableCards[randomIndex];
    }
    
    private Card SelectMediumBotCard(List<Card> playableCards, CardSuit leadSuit)
    {
        if (playableCards.Count == 0) return null;
        
        // Medium bot tries to avoid hearts when possible
        List<Card> nonHeartCards = playableCards.Where(card => card.suit != CardSuit.Hearts).ToList();
        
        if (nonHeartCards.Count > 0)
        {
            return SelectRandomCard(nonHeartCards);
        }
        
        // If must play hearts, play lowest point card
        return playableCards.OrderBy(card => card.pointValue).First();
    }
    
    private Card SelectAdvancedBotCard(List<Card> playableCards, CardSuit leadSuit)
    {
        if (playableCards.Count == 0) return null;
        
        // Advanced bot tries to avoid hearts completely if possible
        List<Card> nonHeartCards = playableCards.Where(card => card.suit != CardSuit.Hearts).ToList();
        
        if (nonHeartCards.Count > 0)
        {
            return SelectRandomCard(nonHeartCards);
        }
        
        // If must play hearts, play lowest value heart
        return playableCards.OrderBy(card => card.pointValue).First();
    }
    
    private Card SelectOmniscientBotCard(List<Card> playableCards, CardSuit leadSuit)
    {
        // The omniscient bot knows all cards and makes the optimal play
        // This logic would need info about other players' hands
        // For now, similar to advanced bot but could be improved later
        return SelectAdvancedBotCard(playableCards, leadSuit);
    }
    
    public void UpdateScore(int roundPoints)
    {
        roundScore = roundPoints;
        
        if (roundScore == 0)
        {
            consecutiveZeroRounds++;
            if (consecutiveZeroRounds == 3)
            {
                // Special rule: three consecutive rounds with 0 points
                totalScore -= 3;
                consecutiveZeroRounds = 0;
            }
        }
        else
        {
            consecutiveZeroRounds = 0;
            totalScore += roundScore;
        }
    }
    
    public void ClearHand()
    {
        hand.Clear();
        roundScore = 0;
    }
} 