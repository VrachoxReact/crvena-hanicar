using UnityEngine;
using UnityEngine.UI;

public class DiagnosticPanel : MonoBehaviour
{
    [SerializeField] private GameDiagnostics diagnostics;
    [SerializeField] private Button runButton;
    [SerializeField] private Button closeButton;
    [SerializeField] private GameObject panelObject;
    
    private void Start()
    {
        if (runButton != null)
        {
            runButton.onClick.AddListener(OnRunButtonClicked);
        }
        
        if (closeButton != null)
        {
            closeButton.onClick.AddListener(OnCloseButtonClicked);
        }
        
        // Hide panel initially
        if (panelObject != null)
        {
            panelObject.SetActive(false);
        }
    }
    
    public void ShowPanel()
    {
        if (panelObject != null)
        {
            panelObject.SetActive(true);
        }
    }
    
    private void OnRunButtonClicked()
    {
        if (diagnostics != null)
        {
            diagnostics.RunDiagnostics();
        }
        else
        {
            Debug.LogError("Diagnostics reference is missing!");
        }
    }
    
    private void OnCloseButtonClicked()
    {
        if (panelObject != null)
        {
            panelObject.SetActive(false);
        }
    }
} 