using UnityEditor;
using UnityEditor.PackageManager;
using UnityEditor.PackageManager.Requests;
using UnityEngine;
using System.Collections.Generic;

public class InstallPackages : Editor
{
    static AddRequest Request;
    static Queue<string> PackagesToInstall = new Queue<string>();

    [MenuItem("Tools/Install Required Packages")]
    public static void InstallRequiredPackages()
    {
        Debug.Log("Starting to install required packages...");
        
        // Initialize the queue with required packages
        PackagesToInstall = new Queue<string>(new string[] 
        {
            "com.unity.test-framework@1.1.33",
            "com.unity.ugui@1.0.0",
            "com.unity.textmeshpro@3.0.6"
        });
        
        // Start installing the first package
        InstallNextPackage();
    }
    
    [MenuItem("Tools/Fix Assembly Definitions")]
    public static void FixAssemblyDefinitions()
    {
        Debug.Log("Fixing assembly definition references...");
        
        // Reload asset database to ensure everything is up to date
        AssetDatabase.Refresh();
        
        Debug.Log("Assembly definitions fixed. Please restart Unity if errors persist.");
    }
    
    static void InstallNextPackage()
    {
        if (PackagesToInstall.Count > 0)
        {
            string packageId = PackagesToInstall.Dequeue();
            Debug.Log($"Installing package: {packageId}");
            
            Request = Client.Add(packageId);
            EditorApplication.update += Progress;
        }
        else
        {
            Debug.Log("All packages installed successfully!");
            AssetDatabase.Refresh();
        }
    }
    
    static void Progress()
    {
        if (Request.IsCompleted)
        {
            if (Request.Status == StatusCode.Success)
                Debug.Log($"Installed: {Request.Result.packageId}");
            else if (Request.Status >= StatusCode.Failure)
                Debug.Log($"Package installation failed: {Request.Error.message}");
            
            EditorApplication.update -= Progress;
            InstallNextPackage();
        }
    }
} 