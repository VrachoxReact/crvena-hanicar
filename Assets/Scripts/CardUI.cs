using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(Card))]
public class CardUI : MonoBehaviour, IPointerClickHandler
{
    private Card card;
    private GameManager gameManager;
    
    private void Awake()
    {
        card = GetComponent<Card>();
        if (gameManager == null)
        {
            gameManager = FindFirstObjectByType<GameManager>();
        }
    }
    
    public void OnPointerClick(PointerEventData eventData)
    {
        // When card is clicked, notify the GameManager
        gameManager.OnCardClicked(card);
    }
    
    // Optional: Add hover effects or animations
    public void OnPointerEnter(PointerEventData eventData)
    {
        // Only animate cards in the human player's hand
        if (transform.parent.GetComponent<Player>()?.playerType == PlayerType.Human)
        {
            transform.localPosition += new Vector3(0, 0.3f, 0);
        }
    }
    
    public void OnPointerExit(PointerEventData eventData)
    {
        // Reset position when not hovering
        if (transform.parent.GetComponent<Player>()?.playerType == PlayerType.Human)
        {
            transform.localPosition -= new Vector3(0, 0.3f, 0);
        }
    }
} 