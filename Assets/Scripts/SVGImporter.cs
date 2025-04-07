using UnityEngine;
using System.IO;
using UnityEngine.Networking;
using System.Collections;

public class SVGImporter : MonoBehaviour
{
    public static SVGImporter Instance { get; private set; }
    
    [SerializeField] private bool preloadAllAssets = true;
    private const string SVG_PATH = "file://";
    
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            
            if (preloadAllAssets)
            {
                StartCoroutine(PreloadAssets());
            }
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    private IEnumerator PreloadAssets()
    {
        Debug.Log("Preloading SVG assets...");
        
        // Preload card assets
        Object[] cardAssets = Resources.LoadAll("Cards", typeof(Texture2D));
        Debug.Log($"Preloaded {cardAssets.Length} card assets");
        
        // Preload UI assets
        Object[] uiAssets = Resources.LoadAll("UI", typeof(Texture2D));
        Debug.Log($"Preloaded {uiAssets.Length} UI assets");
        
        yield return null;
    }
    
    public Texture2D LoadSVGAsTexture(string svgPath, int width = 256, int height = 256)
    {
        // In a real implementation, this would use a library like SVG Importer from the Asset Store
        // For our purposes, we'll simulate by loading pre-rendered textures
        
        string assetName = Path.GetFileNameWithoutExtension(svgPath);
        Texture2D texture = Resources.Load<Texture2D>(assetName);
        
        if (texture == null)
        {
            Debug.LogWarning($"Could not load SVG asset: {assetName}");
            // Create a placeholder texture
            texture = new Texture2D(width, height);
            Color[] colors = new Color[width * height];
            for (int i = 0; i < colors.Length; i++)
            {
                colors[i] = Color.magenta; // Placeholder color
            }
            texture.SetPixels(colors);
            texture.Apply();
        }
        
        return texture;
    }
    
    public IEnumerator LoadSVGAsTextureAsync(string svgPath, System.Action<Texture2D> callback, int width = 256, int height = 256)
    {
        // In a real implementation, this would load and parse the SVG file
        // For our simulation, we'll use a coroutine to simulate async loading
        
        yield return new WaitForSeconds(0.1f); // Simulate loading time
        
        Texture2D texture = LoadSVGAsTexture(svgPath, width, height);
        callback?.Invoke(texture);
    }
    
    // These methods are added for unit testing
    
    public Sprite ImportSVG(string svgPath)
    {
        // Create a texture from the SVG file
        Texture2D texture = LoadSVGAsTexture(svgPath);
        
        // Create a sprite from the texture
        return Sprite.Create(
            texture, 
            new Rect(0, 0, texture.width, texture.height), 
            new Vector2(0.5f, 0.5f)
        );
    }
    
    public Sprite GetSVGSprite(string name)
    {
        // Look for the sprite in Resources
        Sprite sprite = Resources.Load<Sprite>($"Cards/{name}");
        
        if (sprite == null)
        {
            // If not found, try UI folder
            sprite = Resources.Load<Sprite>($"UI/{name}");
        }
        
        if (sprite == null)
        {
            // If still not found, create a placeholder
            Texture2D texture = new Texture2D(100, 100);
            Color[] colors = new Color[100 * 100];
            for (int i = 0; i < colors.Length; i++)
            {
                colors[i] = Color.magenta; // Placeholder color
            }
            texture.SetPixels(colors);
            texture.Apply();
            
            sprite = Sprite.Create(
                texture, 
                new Rect(0, 0, texture.width, texture.height), 
                new Vector2(0.5f, 0.5f)
            );
            
            Debug.LogWarning($"Could not load SVG sprite: {name}, using placeholder");
        }
        
        return sprite;
    }
} 