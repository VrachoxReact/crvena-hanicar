using UnityEngine;
using UnityEditor;
using System.IO;
using System.Reflection;
using System;
using System.Collections.Generic;

public class DiagnoseErrors : Editor
{
    [MenuItem("Tools/Diagnose Project Errors")]
    public static void DiagnoseProjectErrors()
    {
        Debug.Log("=== PROJECT DIAGNOSIS REPORT ===");
        
        // Check for Unity version
        Debug.Log($"Unity Version: {Application.unityVersion}");
        
        // Check installed packages
        CheckInstalledPackages();
        
        // Check for TextMeshPro
        CheckTextMeshPro();
        
        // Check assembly definitions
        CheckAssemblyDefinitions();
        
        // Check for UI references
        CheckUIReferences();
        
        Debug.Log("=== END OF DIAGNOSIS ===");
    }
    
    private static void CheckInstalledPackages()
    {
        Debug.Log("\n--- INSTALLED PACKAGES ---");
        
        string manifestPath = Path.Combine(Application.dataPath, "..", "Packages", "manifest.json");
        if (File.Exists(manifestPath))
        {
            string manifest = File.ReadAllText(manifestPath);
            Debug.Log(manifest);
        }
        else
        {
            Debug.LogError("Could not find manifest.json");
        }
    }
    
    private static void CheckTextMeshPro()
    {
        Debug.Log("\n--- TEXTMESHPRO STATUS ---");
        
        // Check if TextMeshPro types can be loaded
        try
        {
            Type textMeshProType = Type.GetType("TMPro.TextMeshProUGUI, Unity.TextMeshPro");
            if (textMeshProType != null)
            {
                Debug.Log("TextMeshPro types are accessible");
                
                // Check if TMP assemblies are loaded
                bool tmpLoaded = false;
                foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
                {
                    if (assembly.FullName.Contains("Unity.TextMeshPro"))
                    {
                        Debug.Log($"TextMeshPro assembly is loaded: {assembly.FullName}");
                        tmpLoaded = true;
                        break;
                    }
                }
                
                if (!tmpLoaded)
                {
                    Debug.LogError("TextMeshPro assembly is not loaded");
                }
            }
            else
            {
                Debug.LogError("TextMeshPro types cannot be accessed");
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"Error checking TextMeshPro: {e.Message}");
        }
    }
    
    private static void CheckAssemblyDefinitions()
    {
        Debug.Log("\n--- ASSEMBLY DEFINITIONS ---");
        
        // Check CrvenaGame.asmdef
        string gameAsmdefPath = Path.Combine(Application.dataPath, "Scripts", "CrvenaGame.asmdef");
        if (File.Exists(gameAsmdefPath))
        {
            Debug.Log("CrvenaGame.asmdef content:");
            Debug.Log(File.ReadAllText(gameAsmdefPath));
        }
        else
        {
            Debug.LogError("CrvenaGame.asmdef not found");
        }
        
        // Check EditModeTests.asmdef
        string testsAsmdefPath = Path.Combine(Application.dataPath, "Tests", "EditMode", "EditModeTests.asmdef");
        if (File.Exists(testsAsmdefPath))
        {
            Debug.Log("EditModeTests.asmdef content:");
            Debug.Log(File.ReadAllText(testsAsmdefPath));
        }
        else
        {
            Debug.LogError("EditModeTests.asmdef not found");
        }
    }
    
    private static void CheckUIReferences()
    {
        Debug.Log("\n--- UI REFERENCES ---");
        
        // Check UnityEngine.UI availability
        try
        {
            Type buttonType = Type.GetType("UnityEngine.UI.Button, Unity.ugui");
            if (buttonType != null)
            {
                Debug.Log("UnityEngine.UI types are accessible");
            }
            else
            {
                Debug.LogError("UnityEngine.UI types cannot be accessed");
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"Error checking UI references: {e.Message}");
        }
        
        // Check for TestRunner availability
        try
        {
            Type testRunnerType = Type.GetType("NUnit.Framework.TestAttribute, UnityEngine.TestRunner");
            if (testRunnerType != null)
            {
                Debug.Log("Test Framework types are accessible");
            }
            else
            {
                Debug.LogError("Test Framework types cannot be accessed");
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"Error checking Test Framework: {e.Message}");
        }
    }
} 