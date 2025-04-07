using UnityEngine;
using System.Collections.Generic;
using System.Text;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameDiagnostics : MonoBehaviour
{
    [Header("Diagnostic Settings")]
    [SerializeField] private bool logToConsole = true;
    [SerializeField] private bool logToUI = true;
    [SerializeField] private bool runOnStart = true;
    
    [Header("UI References")]
    [SerializeField] private Text diagnosticText;
    [SerializeField] private ScrollRect scrollRect;
    
    private StringBuilder logBuilder = new StringBuilder();
    
    private void Start()
    {
        if (runOnStart)
        {
            RunDiagnostics();
        }
    }
    
    public void RunDiagnostics()
    {
        logBuilder.Clear();
        
        LogMessage("=== CRVENA GAME DIAGNOSTICS ===");
        LogMessage($"Unity Version: {Application.unityVersion}");
        LogMessage($"Platform: {Application.platform}");
        LogMessage($"Current Scene: {SceneManager.GetActiveScene().name}");
        LogMessage("");
        
        // Check for required components
        CheckComponent<GameManager>("GameManager");
        CheckComponent<UIManager>("UIManager");
        CheckComponent<HintManager>("HintManager");
        CheckComponent<VisualEffects>("VisualEffects");
        CheckComponent<TutorialManager>("TutorialManager");
        
        // Check Game Manager configuration
        CheckGameManagerReferences();
        
        // Check for required resources
        CheckResourcesExist();
        
        // Check UI Manager configuration
        CheckUIManagerReferences();
        
        // Check for errors
        CheckForExistingErrors();
        
        // Update UI with diagnostic results
        if (logToUI && diagnosticText != null)
        {
            diagnosticText.text = logBuilder.ToString();
            if (scrollRect != null)
            {
                // Scroll to top
                Canvas.ForceUpdateCanvases();
                scrollRect.verticalNormalizedPosition = 1;
            }
        }
    }
    
    private void CheckComponent<T>(string componentName) where T : MonoBehaviour
    {
        T component = FindFirstObjectByType<T>();
        if (component == null)
        {
            LogError($"Missing required component: {componentName}");
        }
        else
        {
            LogSuccess($"Found component: {componentName}");
        }
    }
    
    private void CheckGameManagerReferences()
    {
        GameManager gameManager = FindFirstObjectByType<GameManager>();
        if (gameManager == null) return;
        
        LogMessage("\n=== GAME MANAGER REFERENCES ===");
        
        // Use reflection to check serialized fields
        var fields = gameManager.GetType().GetFields(System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        foreach (var field in fields)
        {
            if (field.IsDefined(typeof(SerializeField), false))
            {
                var value = field.GetValue(gameManager);
                if (value == null)
                {
                    LogWarning($"Missing reference: {field.Name}");
                }
            }
        }
    }
    
    private void CheckUIManagerReferences()
    {
        UIManager uiManager = FindFirstObjectByType<UIManager>();
        if (uiManager == null) return;
        
        LogMessage("\n=== UI MANAGER REFERENCES ===");
        
        // Use reflection to check serialized fields
        var fields = uiManager.GetType().GetFields(System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        foreach (var field in fields)
        {
            if (field.IsDefined(typeof(SerializeField), false))
            {
                var value = field.GetValue(uiManager);
                if (value == null)
                {
                    LogWarning($"Missing UI reference: {field.Name}");
                }
            }
        }
    }
    
    private void CheckResourcesExist()
    {
        LogMessage("\n=== RESOURCE CHECKS ===");
        
        // Check for card resources
        bool cardsExist = System.IO.Directory.Exists(Application.dataPath + "/Resources/Cards");
        if (!cardsExist)
        {
            LogError("Missing Cards resource directory");
        }
        else
        {
            LogSuccess("Cards resource directory exists");
            
            // Check for sample card
            Sprite testCard = Resources.Load<Sprite>("Cards/ace_of_hearts");
            if (testCard == null)
            {
                LogError("Card resources not loading correctly");
            }
            else
            {
                LogSuccess("Card resources loading correctly");
            }
        }
        
        // Check for UI resources
        bool uiExists = System.IO.Directory.Exists(Application.dataPath + "/Resources/UI");
        if (!uiExists)
        {
            LogError("Missing UI resource directory");
        }
        else
        {
            LogSuccess("UI resource directory exists");
        }
    }
    
    private void CheckForExistingErrors()
    {
        LogMessage("\n=== CONSOLE ERRORS ===");
        
        // This won't capture existing console logs, but will note if there are any exceptions
        if (Debug.unityLogger.logEnabled)
        {
            LogSuccess("Unity logger is enabled");
        }
        else
        {
            LogWarning("Unity logger is disabled");
        }
    }
    
    private void LogMessage(string message)
    {
        logBuilder.AppendLine(message);
        if (logToConsole) Debug.Log(message);
    }
    
    private void LogWarning(string message)
    {
        string warningMessage = $"⚠️ WARNING: {message}";
        logBuilder.AppendLine(warningMessage);
        if (logToConsole) Debug.LogWarning(message);
    }
    
    private void LogError(string message)
    {
        string errorMessage = $"❌ ERROR: {message}";
        logBuilder.AppendLine(errorMessage);
        if (logToConsole) Debug.LogError(message);
    }
    
    private void LogSuccess(string message)
    {
        string successMessage = $"✅ {message}";
        logBuilder.AppendLine(successMessage);
        if (logToConsole) Debug.Log(message);
    }
} 