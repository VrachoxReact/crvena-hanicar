using UnityEngine;
using UnityEditor;
using System.IO;
using System.Collections.Generic;

public class SVGPreprocessor : EditorWindow
{
    [MenuItem("Tools/Crvena/SVG Preprocessor")]
    public static void ShowWindow()
    {
        GetWindow<SVGPreprocessor>("SVG Preprocessor");
    }
    
    private bool processingCards = true;
    private bool processingUI = true;
    private int targetWidth = 180;
    private int targetHeight = 250;
    private int uiScale = 1;
    private bool createPNGFallbacks = true;
    
    private void OnGUI()
    {
        GUILayout.Label("SVG Processing Settings", EditorStyles.boldLabel);
        
        EditorGUILayout.Space();
        
        GUILayout.Label("Asset Types to Process:", EditorStyles.boldLabel);
        processingCards = EditorGUILayout.Toggle("Process Card SVGs", processingCards);
        processingUI = EditorGUILayout.Toggle("Process UI SVGs", processingUI);
        
        EditorGUILayout.Space();
        
        GUILayout.Label("Size Settings:", EditorStyles.boldLabel);
        targetWidth = EditorGUILayout.IntField("Card Width", targetWidth);
        targetHeight = EditorGUILayout.IntField("Card Height", targetHeight);
        uiScale = EditorGUILayout.IntSlider("UI Scale", uiScale, 1, 4);
        
        EditorGUILayout.Space();
        
        GUILayout.Label("Output Options:", EditorStyles.boldLabel);
        createPNGFallbacks = EditorGUILayout.Toggle("Create PNG Fallbacks", createPNGFallbacks);
        
        EditorGUILayout.Space();
        
        if (GUILayout.Button("Process SVG Files"))
        {
            ProcessSVGFiles();
        }
        
        if (GUILayout.Button("Create Missing Cards"))
        {
            CreateMissingCards();
        }
    }
    
    private void ProcessSVGFiles()
    {
        // In a real implementation, this would use a proper SVG rendering library
        // For now, we'll just display information about what would happen
        
        int cardCount = 0;
        int uiCount = 0;
        
        if (processingCards)
        {
            string cardPath = "Assets/Resources/Cards";
            if (Directory.Exists(cardPath))
            {
                string[] svgFiles = Directory.GetFiles(cardPath, "*.svg", SearchOption.TopDirectoryOnly);
                cardCount = svgFiles.Length;
                
                Debug.Log($"Found {cardCount} card SVG files to process");
                
                if (createPNGFallbacks)
                {
                    Debug.Log($"Would create {cardCount} PNG fallbacks at {targetWidth}x{targetHeight} resolution");
                }
            }
            else
            {
                Debug.LogWarning($"Card directory not found: {cardPath}");
            }
        }
        
        if (processingUI)
        {
            string uiPath = "Assets/Resources/UI";
            if (Directory.Exists(uiPath))
            {
                string[] svgFiles = Directory.GetFiles(uiPath, "*.svg", SearchOption.TopDirectoryOnly);
                uiCount = svgFiles.Length;
                
                Debug.Log($"Found {uiCount} UI SVG files to process");
                
                if (createPNGFallbacks)
                {
                    Debug.Log($"Would create {uiCount} PNG fallbacks at scale {uiScale}x");
                }
            }
            else
            {
                Debug.LogWarning($"UI directory not found: {uiPath}");
            }
        }
        
        EditorUtility.DisplayDialog("SVG Processing", 
            $"Found {cardCount + uiCount} SVG files to process.\n\n" +
            "In a real implementation, this would render the SVGs to textures and save them as PNGs.\n\n" +
            "You would need to integrate an SVG rendering library like 'SVG Importer' or 'Unity Vector Graphics' to fully implement this functionality.",
            "OK");
    }
    
    private void CreateMissingCards()
    {
        // Check for existing card assets
        string cardPath = "Assets/Resources/Cards";
        
        if (!Directory.Exists(cardPath))
        {
            Directory.CreateDirectory(cardPath);
        }
        
        List<string> missingCards = new List<string>();
        
        // Check which cards we need to create
        foreach (CardSuit suit in System.Enum.GetValues(typeof(CardSuit)))
        {
            foreach (CardRank rank in System.Enum.GetValues(typeof(CardRank)))
            {
                string rankName = GetRankName(rank);
                string suitName = suit.ToString().ToLower();
                string filename = $"{rankName}_of_{suitName}.svg";
                string filepath = Path.Combine(cardPath, filename);
                
                if (!File.Exists(filepath))
                {
                    missingCards.Add(filename);
                }
            }
        }
        
        if (missingCards.Count > 0)
        {
            Debug.Log($"Found {missingCards.Count} missing card assets to create:");
            foreach (string card in missingCards)
            {
                Debug.Log($"- {card}");
            }
            
            EditorUtility.DisplayDialog("Create Missing Cards", 
                $"Found {missingCards.Count} missing card SVGs.\n\n" +
                "In a real implementation, this would create template SVG files for these cards.\n\n" +
                "You would need to modify and design these template files to complete the card set.",
                "OK");
        }
        else
        {
            EditorUtility.DisplayDialog("Create Missing Cards", 
                "All card SVG files exist! The deck is complete.",
                "OK");
        }
    }
    
    private string GetRankName(CardRank rank)
    {
        switch (rank)
        {
            case CardRank.Seven: return "7";
            case CardRank.Eight: return "8";
            case CardRank.Nine: return "9";
            case CardRank.Ten: return "10";
            case CardRank.Jack: return "jack";
            case CardRank.Queen: return "queen";
            case CardRank.King: return "king";
            case CardRank.Ace: return "ace";
            default: return rank.ToString().ToLower();
        }
    }
} 