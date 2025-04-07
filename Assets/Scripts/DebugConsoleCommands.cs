using UnityEngine;

public class DebugConsoleCommands : MonoBehaviour
{
    [SerializeField] private GameDiagnostics diagnostics;
    [SerializeField] private DiagnosticPanel diagnosticPanel;
    
    private bool consolePanelVisible = false;
    
    private void Awake()
    {
        if (diagnostics == null)
        {
            diagnostics = FindFirstObjectByType<GameDiagnostics>();
        }
        
        if (diagnosticPanel == null)
        {
            diagnosticPanel = FindFirstObjectByType<DiagnosticPanel>();
        }
    }
    
    private void Update()
    {
        // Toggle console with F8
        if (Input.GetKeyDown(KeyCode.F8))
        {
            consolePanelVisible = !consolePanelVisible;
            
            if (consolePanelVisible && diagnosticPanel != null)
            {
                diagnosticPanel.ShowPanel();
            }
        }
        
        // Run diagnostics with F9
        if (Input.GetKeyDown(KeyCode.F9) && diagnostics != null)
        {
            diagnostics.RunDiagnostics();
        }
    }
    
    // Called via the editor to manually run diagnostics
    public static void RunDiagnostics()
    {
        GameDiagnostics diagnostics = FindFirstObjectByType<GameDiagnostics>();
        if (diagnostics != null)
        {
            diagnostics.RunDiagnostics();
        }
        else
        {
            Debug.LogError("No GameDiagnostics component found in the scene");
        }
    }
    
    // Called via the editor to detect serialized field issues
    public static void ValidateSerializedFields()
    {
        // Find all GameObjects with MonoBehaviours
        MonoBehaviour[] allBehaviours = FindObjectsByType<MonoBehaviour>(FindObjectsSortMode.None);
        int errorCount = 0;
        
        foreach (MonoBehaviour behaviour in allBehaviours)
        {
            var fields = behaviour.GetType().GetFields(System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            foreach (var field in fields)
            {
                if (field.IsDefined(typeof(SerializeField), false))
                {
                    var value = field.GetValue(behaviour);
                    if (value == null)
                    {
                        Debug.LogWarning($"Missing reference: {behaviour.gameObject.name}.{behaviour.GetType().Name}.{field.Name}", behaviour);
                        errorCount++;
                    }
                }
            }
        }
        
        Debug.Log($"Validation complete. Found {errorCount} missing references");
    }
} 