# Crvena - Installation Guide

This document provides detailed instructions for setting up the Crvena card game project in Unity.

## Prerequisites

- **Unity Version**: 2020.3 LTS or newer (Project tested with Unity 2021.3)
- **Development Environment**: Visual Studio 2019/2022 or Visual Studio Code
- **Platform**: Windows, macOS, or Linux for development (game can be built for multiple platforms)

## Installation Steps

### 1. Getting the Project

**Option A: Clone from Repository**
```
git clone https://github.com/yourusername/crvena-hanicar.git
cd crvena-hanicar
```

**Option B: Download ZIP**
- Download the ZIP archive from the repository
- Extract to your preferred location

### 2. Opening the Project in Unity

1. Open Unity Hub
2. Click "Add" and browse to the extracted project folder
3. Select the project folder and click "Open"
4. Unity will import and process the project files (this may take a few minutes)

### 3. Project Structure Setup

After opening, verify the following folders exist:
- `Assets/Scripts`: Contains all C# scripts
- `Assets/Prefabs`: Contains prefab objects
- `Assets/Resources`: Contains SVG graphics and other resources
- `Assets/Scenes`: Contains MainMenu and GameScene

If any are missing, create them manually.

### 4. SVG Integration

This project uses SVG files for visuals. For proper SVG support:

**Option A: Use the built-in basic SVG importer**
- The included SVGImporter.cs provides basic SVG support
- This is suitable for testing and development

**Option B: Enhanced SVG Support (recommended for production)**
1. Install an SVG plugin from the Asset Store:
   - "SVG Importer" by Not Doppler (paid)
   - "Vector Graphics" package (Unity's experimental package)
2. Update the SVGImporter.cs script to use your chosen plugin

### 5. Build Settings Configuration

1. Open Build Settings (File > Build Settings)
2. Add scenes in the following order:
   - MainMenu (should be first)
   - GameScene
3. Select your target platform
4. Click "Switch Platform" if needed

### 6. Running the Game

**Option A: In the Unity Editor**
1. Open the MainMenu scene
2. Click the Play button

**Option B: Building a Standalone Version**
1. Open Build Settings
2. Click "Build" or "Build And Run"
3. Choose a destination folder
4. Launch the built executable

## Troubleshooting

### Common Issues

**Missing References**
- If you see null reference errors, ensure all prefabs have their dependencies assigned
- In GameManager, check that all required references are properly set in the Inspector

**SVG Rendering Issues**
- If SVG files don't render correctly, try using PNG fallbacks in Resources/Cards
- Or convert SVGs to sprites using the editor's import settings

**Script Compilation Errors**
- Ensure you're using a compatible C# version
- Check that all script dependencies are properly referenced

### Getting Help

If you encounter issues not covered here, please:
- Check the Unity Console for specific error messages
- Reference the README.md for project structure information
- Create an issue in the project repository

## Additional Resources

- [Unity Documentation](https://docs.unity3d.com/)
- [C# Programming Guide](https://docs.microsoft.com/en-us/dotnet/csharp/)
- [SVG Specification](https://www.w3.org/TR/SVG2/)

## License

This project is licensed under the MIT License - see the LICENSE.md file for details. 