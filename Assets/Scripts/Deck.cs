using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Deck : MonoBehaviour
{
    [SerializeField] private GameObject cardPrefab;
    [SerializeField] private Transform deckTransform;
    
    private List<Card> cards = new List<Card>();
    private List<Card> discardPile = new List<Card>();
    
    public void Initialize()
    {
        CreateDeck();
        ShuffleDeck();
    }
    
    private void CreateDeck()
    {
        cards.Clear();
        discardPile.Clear();
        
        // Create a standard Belot deck (32 cards: 7-Ace in all four suits)
        foreach (CardSuit suit in System.Enum.GetValues(typeof(CardSuit)))
        {
            foreach (CardRank rank in System.Enum.GetValues(typeof(CardRank)))
            {
                GameObject cardObj = Instantiate(cardPrefab, deckTransform);
                cardObj.SetActive(false);
                Card card = cardObj.GetComponent<Card>();
                card.Initialize(suit, rank);
                cards.Add(card);
            }
        }
    }
    
    public void ShuffleDeck()
    {
        System.Random rng = new System.Random();
        cards = cards.OrderBy(card => rng.Next()).ToList();
        
        // Reposition cards in deck
        for (int i = 0; i < cards.Count; i++)
        {
            cards[i].transform.SetParent(deckTransform);
            cards[i].transform.localPosition = new Vector3(0, i * 0.05f, 0);
            cards[i].gameObject.SetActive(true);
        }
    }
    
    public Card DrawCard()
    {
        if (cards.Count == 0 && discardPile.Count > 0)
        {
            // Recycle discard pile if deck is empty
            cards.AddRange(discardPile);
            discardPile.Clear();
            ShuffleDeck();
        }
        
        if (cards.Count == 0)
        {
            Debug.LogWarning("No cards left to draw!");
            return null;
        }
        
        Card drawnCard = cards[0];
        cards.RemoveAt(0);
        drawnCard.gameObject.SetActive(true);
        return drawnCard;
    }
    
    public void DiscardCard(Card card)
    {
        if (!discardPile.Contains(card))
        {
            discardPile.Add(card);
            card.transform.SetParent(deckTransform);
            card.transform.localPosition = new Vector3(2, 0, 0); // Position of discard pile
        }
    }
    
    public void ReturnAllCardsToDeck()
    {
        cards.AddRange(discardPile);
        discardPile.Clear();
        ShuffleDeck();
    }
} 