using System;
using UnityEngine;

public enum CardSuit
{
    Hearts,
    Diamonds,
    Clubs,
    Spades
}

public enum CardRank
{
    Seven,
    Eight,
    Nine,
    Ten,
    Jack,
    Queen,
    King,
    Ace
}

public class Card : MonoBehaviour
{
    public CardSuit suit;
    public CardRank rank;
    public bool isRed => suit == CardSuit.Hearts || suit == CardSuit.Diamonds;
    public int pointValue => CalculatePointValue();
    
    [SerializeField] private SpriteRenderer frontRenderer;
    [SerializeField] private SpriteRenderer backRenderer;
    private bool isFaceUp = false;
    
    public void Initialize(CardSuit suit, CardRank rank)
    {
        this.suit = suit;
        this.rank = rank;
        UpdateSprite();
    }
    
    private void UpdateSprite()
    {
        string spriteName = GetSpriteName();
        frontRenderer.sprite = Resources.Load<Sprite>($"Cards/{spriteName}");
    }
    
    private string GetSpriteName()
    {
        string suitName = suit.ToString().ToLower();
        string rankName = "";
        
        switch (rank)
        {
            case CardRank.Seven: rankName = "7"; break;
            case CardRank.Eight: rankName = "8"; break;
            case CardRank.Nine: rankName = "9"; break;
            case CardRank.Ten: rankName = "10"; break;
            case CardRank.Jack: rankName = "jack"; break;
            case CardRank.Queen: rankName = "queen"; break;
            case CardRank.King: rankName = "king"; break;
            case CardRank.Ace: rankName = "ace"; break;
        }
        
        return $"{rankName}_of_{suitName}";
    }
    
    public void Flip()
    {
        isFaceUp = !isFaceUp;
        frontRenderer.gameObject.SetActive(isFaceUp);
        backRenderer.gameObject.SetActive(!isFaceUp);
    }
    
    private int CalculatePointValue()
    {
        if (suit == CardSuit.Hearts)
        {
            return rank == CardRank.Ace ? 3 : 1;
        }
        return 0;
    }
    
    public int GetStrengthValue()
    {
        // Order: 7, 8, 9, 10, Jack, Queen, King, Ace
        return (int)rank;
    }
} 