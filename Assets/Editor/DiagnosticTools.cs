using UnityEngine;
using UnityEditor;

public class DiagnosticTools
{
    [MenuItem("Tools/Diagnostics/Run Diagnostics")]
    public static void RunDiagnostics()
    {
        DebugConsoleCommands.RunDiagnostics();
    }
    
    [MenuItem("Tools/Diagnostics/Validate Missing References")]
    public static void ValidateSerializedFields()
    {
        DebugConsoleCommands.ValidateSerializedFields();
    }
    
    [MenuItem("Tools/Diagnostics/Check Resources")]
    public static void CheckResources()
    {
        Debug.Log("Checking for required resource directories...");
        
        string cardsPath = Application.dataPath + "/Resources/Cards";
        string uiPath = Application.dataPath + "/Resources/UI";
        
        bool cardsExist = System.IO.Directory.Exists(cardsPath);
        bool uiExist = System.IO.Directory.Exists(uiPath);
        
        if (!cardsExist)
        {
            Debug.LogError($"Missing required resource directory: {cardsPath}");
            if (EditorUtility.DisplayDialog("Create Directory?", 
                "The Cards resource directory is missing. Create it now?", 
                "Create Directory", "Cancel"))
            {
                System.IO.Directory.CreateDirectory(cardsPath);
                Debug.Log("Created directory: " + cardsPath);
                AssetDatabase.Refresh();
            }
        }
        else
        {
            Debug.Log("Cards resource directory exists");
        }
        
        if (!uiExist)
        {
            Debug.LogError($"Missing required resource directory: {uiPath}");
            if (EditorUtility.DisplayDialog("Create Directory?", 
                "The UI resource directory is missing. Create it now?", 
                "Create Directory", "Cancel"))
            {
                System.IO.Directory.CreateDirectory(uiPath);
                Debug.Log("Created directory: " + uiPath);
                AssetDatabase.Refresh();
            }
        }
        else
        {
            Debug.Log("UI resource directory exists");
        }
    }
    
    [MenuItem("Tools/Diagnostics/Create Debug Panel")]
    public static void CreateDebugPanel()
    {
        // Check if a panel already exists
        DiagnosticPanel existingPanel = GameObject.FindFirstObjectByType<DiagnosticPanel>();
        if (existingPanel != null)
        {
            Debug.Log("Diagnostic panel already exists in the scene");
            EditorGUIUtility.PingObject(existingPanel.gameObject);
            Selection.activeGameObject = existingPanel.gameObject;
            return;
        }
        
        // Create panel hierarchy
        GameObject panelRoot = new GameObject("DiagnosticPanel");
        Canvas canvas = null;
        
        // Try to find an existing canvas
        Canvas[] canvases = GameObject.FindObjectsByType<Canvas>(FindObjectsSortMode.None);
        foreach (Canvas c in canvases)
        {
            if (c.renderMode == RenderMode.ScreenSpaceOverlay)
            {
                canvas = c;
                break;
            }
        }
        
        // Create a canvas if none exists
        if (canvas == null)
        {
            GameObject canvasObj = new GameObject("DiagnosticCanvas");
            canvas = canvasObj.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvasObj.AddComponent<UnityEngine.UI.CanvasScaler>();
            canvasObj.AddComponent<UnityEngine.UI.GraphicRaycaster>();
        }
        
        panelRoot.transform.SetParent(canvas.transform, false);
        
        // Add components
        panelRoot.AddComponent<DiagnosticPanel>();
        GameObject diagnosticObj = new GameObject("GameDiagnostics");
        diagnosticObj.AddComponent<GameDiagnostics>();
        diagnosticObj.transform.SetParent(panelRoot.transform, false);
        
        Debug.Log("Created diagnostic panel in the scene");
        EditorGUIUtility.PingObject(panelRoot);
        Selection.activeGameObject = panelRoot;
    }
} 