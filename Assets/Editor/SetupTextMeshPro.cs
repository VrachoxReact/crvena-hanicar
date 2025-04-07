using UnityEngine;
using UnityEditor;
using System.Reflection;
using System;

public class SetupTextMeshPro : Editor
{
    [MenuItem("Tools/Setup TextMeshPro Essentials")]
    public static void SetupTMPEssentials()
    {
        Debug.Log("Setting up TextMeshPro Essentials...");
        
        try
        {
            // Try to find the TMPro namespace and the TMP_PackageUtilities class
            Assembly tmpAssembly = FindTMProAssembly();
            
            if (tmpAssembly != null)
            {
                Type tmpPackageUtilitiesType = tmpAssembly.GetType("TMPro.TMP_PackageUtilities");
                
                if (tmpPackageUtilitiesType != null)
                {
                    // Get the ImportTMPEssentials method
                    MethodInfo importMethod = tmpPackageUtilitiesType.GetMethod(
                        "ImportTMPEssentials",
                        BindingFlags.Static | BindingFlags.Public);
                    
                    if (importMethod != null)
                    {
                        // Invoke the method to import TextMeshPro essentials
                        importMethod.Invoke(null, null);
                        Debug.Log("TextMeshPro essentials imported successfully.");
                    }
                    else
                    {
                        Debug.LogError("Could not find ImportTMPEssentials method");
                    }
                }
                else
                {
                    Debug.LogError("Could not find TMP_PackageUtilities type");
                }
            }
            else
            {
                Debug.LogError("Could not find TextMeshPro assembly");
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"Error setting up TextMeshPro: {e.Message}\n{e.StackTrace}");
        }
    }
    
    private static Assembly FindTMProAssembly()
    {
        // Try to find the TextMeshPro assembly
        foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
        {
            if (assembly.FullName.Contains("Unity.TextMeshPro"))
            {
                return assembly;
            }
        }
        return null;
    }
} 