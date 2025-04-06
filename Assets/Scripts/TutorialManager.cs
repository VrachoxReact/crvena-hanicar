using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class TutorialManager : MonoBehaviour
{
    [System.Serializable]
    public class TutorialStep
    {
        public string message;
        public GameObject highlightObject;
        public bool waitForInput;
        public bool highlightCard;
        public CardSuit highlightSuit;
        public CardRank highlightRank;
    }
    
    [Header("UI Elements")]
    [SerializeField] private GameObject tutorialPanel;
    [SerializeField] private Text tutorialText;
    [SerializeField] private Button nextButton;
    [SerializeField] private Image tutorialArrow;
    
    [Header("Tutorial Settings")]
    [SerializeField] private List<TutorialStep> tutorialSteps = new List<TutorialStep>();
    [SerializeField] private float textTypingSpeed = 0.05f;
    
    [Header("References")]
    [SerializeField] private GameManager gameManager;
    
    private int currentStepIndex = 0;
    private bool isTutorialActive = false;
    private Coroutine typingCoroutine;
    
    private void Awake()
    {
        if (gameManager == null)
        {
            gameManager = FindObjectOfType<GameManager>();
        }
    }
    
    private void Start()
    {
        if (tutorialPanel != null)
        {
            tutorialPanel.SetActive(false);
        }
        
        if (nextButton != null)
        {
            nextButton.onClick.AddListener(OnNextButtonClicked);
        }
        
        // Check if this is first time playing
        if (!PlayerPrefs.HasKey("TutorialCompleted"))
        {
            StartTutorial();
        }
    }
    
    public void StartTutorial()
    {
        if (tutorialSteps.Count == 0 || tutorialPanel == null) return;
        
        isTutorialActive = true;
        tutorialPanel.SetActive(true);
        currentStepIndex = 0;
        
        // Pause the game if needed
        if (gameManager != null)
        {
            gameManager.SetGamePaused(true);
        }
        
        ShowCurrentStep();
    }
    
    private void ShowCurrentStep()
    {
        if (currentStepIndex >= tutorialSteps.Count)
        {
            EndTutorial();
            return;
        }
        
        TutorialStep step = tutorialSteps[currentStepIndex];
        
        // Set text with typing effect
        if (typingCoroutine != null)
        {
            StopCoroutine(typingCoroutine);
        }
        typingCoroutine = StartCoroutine(TypeText(step.message));
        
        // Position arrow if needed
        if (step.highlightObject != null && tutorialArrow != null)
        {
            tutorialArrow.gameObject.SetActive(true);
            PositionArrowAtObject(step.highlightObject);
        }
        else if (tutorialArrow != null)
        {
            tutorialArrow.gameObject.SetActive(false);
        }
        
        // Highlight specific card if needed
        if (step.highlightCard && gameManager != null)
        {
            gameManager.HighlightCard(step.highlightSuit, step.highlightRank);
        }
        else if (gameManager != null)
        {
            gameManager.ClearCardHighlights();
        }
        
        // Show/hide next button based on if we wait for input
        if (nextButton != null)
        {
            nextButton.gameObject.SetActive(step.waitForInput);
        }
        
        // If not waiting for input, auto-advance after delay
        if (!step.waitForInput)
        {
            StartCoroutine(AutoAdvanceAfterDelay(step.message.Length * textTypingSpeed + 2f));
        }
    }
    
    private IEnumerator TypeText(string message)
    {
        tutorialText.text = "";
        
        foreach (char c in message)
        {
            tutorialText.text += c;
            yield return new WaitForSeconds(textTypingSpeed);
        }
    }
    
    private void PositionArrowAtObject(GameObject target)
    {
        if (target == null || tutorialArrow == null) return;
        
        // Convert target position to screen space
        Vector3 targetScreenPos = Camera.main.WorldToScreenPoint(target.transform.position);
        
        // Position arrow slightly offset from target
        tutorialArrow.rectTransform.position = targetScreenPos + new Vector3(0, 50, 0);
        
        // Point arrow downward
        tutorialArrow.rectTransform.rotation = Quaternion.Euler(0, 0, 180);
    }
    
    private IEnumerator AutoAdvanceAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        NextStep();
    }
    
    private void OnNextButtonClicked()
    {
        NextStep();
    }
    
    private void NextStep()
    {
        currentStepIndex++;
        ShowCurrentStep();
    }
    
    private void EndTutorial()
    {
        isTutorialActive = false;
        
        if (tutorialPanel != null)
        {
            tutorialPanel.SetActive(false);
        }
        
        // Unpause the game
        if (gameManager != null)
        {
            gameManager.SetGamePaused(false);
            gameManager.ClearCardHighlights();
        }
        
        // Mark tutorial as completed
        PlayerPrefs.SetInt("TutorialCompleted", 1);
        PlayerPrefs.Save();
    }
    
    public bool IsTutorialActive()
    {
        return isTutorialActive;
    }
    
    // Public method to skip tutorial
    public void SkipTutorial()
    {
        EndTutorial();
    }
} 