using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Card))]
public class CardVisual : MonoBehaviour
{
    [SerializeField] private Image frontImage;
    [SerializeField] private Image backImage;
    [SerializeField] private RectTransform rectTransform;
    
    private Card card;
    private SVGImporter svgImporter;
    
    private void Awake()
    {
        card = GetComponent<Card>();
        svgImporter = SVGImporter.Instance;
        
        if (svgImporter == null)
        {
            Debug.LogError("SVGImporter not found in scene!");
        }
        
        if (frontImage == null || backImage == null)
        {
            Debug.LogError("Card images not assigned!");
        }
    }
    
    public void InitializeVisuals()
    {
        // Load card back texture
        string backPath = "Cards/card_back";
        Texture2D backTexture = Resources.Load<Texture2D>(backPath);
        if (backTexture != null)
        {
            Sprite backSprite = Sprite.Create(backTexture, new Rect(0, 0, backTexture.width, backTexture.height), new Vector2(0.5f, 0.5f));
            backImage.sprite = backSprite;
        }
        
        // Set front based on card suit and rank
        UpdateCardFront();
    }
    
    public void UpdateCardFront()
    {
        string cardName = GetCardSpriteName();
        string frontPath = $"Cards/{cardName}";
        
        Texture2D frontTexture = Resources.Load<Texture2D>(frontPath);
        if (frontTexture != null)
        {
            Sprite frontSprite = Sprite.Create(frontTexture, new Rect(0, 0, frontTexture.width, frontTexture.height), new Vector2(0.5f, 0.5f));
            frontImage.sprite = frontSprite;
        }
        else
        {
            Debug.LogWarning($"Could not load card texture: {frontPath}");
        }
    }
    
    private string GetCardSpriteName()
    {
        string rankName = "";
        switch (card.rank)
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
        
        string suitName = card.suit.ToString().ToLower();
        return $"{rankName}_of_{suitName}";
    }
    
    public void FlipCard(bool showFront)
    {
        frontImage.gameObject.SetActive(showFront);
        backImage.gameObject.SetActive(!showFront);
    }
    
    public void Highlight(bool isHighlighted)
    {
        if (isHighlighted)
        {
            // Raise the card and apply a glow effect
            rectTransform.anchoredPosition += new Vector2(0, 20);
            frontImage.color = new Color(1f, 1f, 1f, 1f);
        }
        else
        {
            // Reset position and remove glow
            rectTransform.anchoredPosition -= new Vector2(0, 20);
            frontImage.color = new Color(0.9f, 0.9f, 0.9f, 1f);
        }
    }
} 