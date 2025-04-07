# Fixing Errors in Crvena Card Game

This document provides detailed instructions on how to fix the compilation errors in the Crvena Card Game project.

## Step 1: Remove TextMeshPro References

Since TextMeshPro is causing issues, we have removed its references:

1. The `using TMPro;` statement has been removed from UIManager.cs
2. The TextMeshPro reference has been removed from both assembly definition files:
   - Assets/Scripts/CrvenaGame.asmdef
   - Assets/Tests/EditMode/EditModeTests.asmdef

## Step 2: Install Required Packages

Use our custom tools to install packages:

1. Open Unity
2. Go to Tools > Install Required Packages
   - This will install Test Framework and Unity UI packages with the correct versions
3. Go to Tools > Fix Assembly Definitions
   - This will reload the assembly definitions with the correct references

If TextMeshPro is required and you'd like to try installing it:
1. Go to Tools > Setup TextMeshPro Essentials
   - This attempts to set up TextMeshPro properly in your project

## Step 3: Diagnose Remaining Issues

If errors persist:

1. Go to Tools > Diagnose Project Errors
   - This will print detailed diagnostic information to the console window
2. Check the console for specific package issues, references, and other problems

## Step 4: Manual Fixes

If the automated tools don't resolve all issues:

1. Open the Package Manager (Window > Package Manager)
2. Verify that Test Framework and Unity UI are installed correctly
3. Try removing and then re-installing these packages if they appear corrupted

## Testing Framework Issues

For testing framework errors, check:

1. Make sure `nunit.framework.dll` is properly referenced in your test assembly
2. If you get errors about missing `[Test]` attributes, try:
   - Updating the Test Framework package (Window > Package Manager > Test Framework > Update)
   - Deleting the `Library` folder in your project and letting Unity rebuild it

## Resource Directories

Make sure these directories exist:
- `Assets/Resources/Cards`
- `Assets/Resources/UI`

## Next Steps

After installing the packages and fixing the references:

1. Let Unity recompile all scripts by saving all modified files
2. Restart Unity completely if issues persist
3. Try building the project to see if build-time errors differ from editor errors

## Complete Project Rebuild (Last Resort)

If nothing else works:
1. Copy all your script files somewhere safe
2. Create a new Unity project with the same Unity version
3. Import the packages via the Package Manager
4. Copy your scripts back into the new project 