# Crvena - Unity Card Game

Crvena (meaning "Red" in Croatian) is a strategic card game similar to Hearthstone but played with traditional Belot cards. The goal is to minimize your points by avoiding hearts, especially the Ace of Hearts.

## Game Rules

### Basic Rules
- **Goal**: Have the fewest points when someone reaches 51 points
- **Cards**: Standard Belot deck (32 cards: 7-Ace in four suits)
- **Card Strength**: 7 < 8 < 9 < 10 < Jack < Queen < King < Ace
- **Points**: Each round starts with 10 points. Hearts = 1 point, Ace of Hearts = 3 points

### Special Rules
- **Dealing**: The player with the most points in the previous round deals next
- **Following Suit**: Players must follow the lead suit if possible
- **Red Cards**: Cannot be played in the first two tricks
- **Consecutive Zero Rule**: If a player gets 0 points for three consecutive rounds, they get -3 points

## Game Features

### Players
- 1 Human player
- 3 AI opponents with different difficulty levels:
  - **West**: Idiot Bot (plays randomly)
  - **East**: Medium Bot (basic strategy)
  - **North**: Advanced Bot (more advanced strategy)
  - **Hint System**: Omniscient Bot (has perfect information)

### Hint System
- Get strategic advice from an "all-knowing" AI
- Unlock hints with diamonds or by watching ads
- Hints show the optimal card to play based on all available information

### Monetization
- Diamond currency for purchasing hints
- Earn diamonds by watching ads
- Purchase diamond packs through in-app purchases

## Project Structure

### Core Scripts
- **Card.cs**: Defines card properties and behaviors
- **Deck.cs**: Handles deck creation, shuffling, and dealing
- **Player.cs**: Manages player hands, scoring, and AI behavior
- **GameManager.cs**: Controls game flow and rules
- **OmniscientBot.cs**: Provides optimal card selection logic for hints
- **HintManager.cs**: Manages the hint system and monetization
- **CardUI.cs**: Handles card interactions and UI
- **MainMenu.cs**: Controls main menu interface
- **UIManager.cs**: Manages game interface elements
- **VisualEffects.cs**: Provides animations and visual polish
- **TutorialManager.cs**: Guides new players through gameplay
- **SVGImporter.cs**: Handles SVG asset loading and rendering

### Assets
The project uses SVG (Scalable Vector Graphics) for all visual elements, providing:
- **Crisp visuals** at any resolution
- **Small file size** compared to traditional raster graphics
- **Easily modifiable** design elements
- **Responsive UI** that scales perfectly on different screen sizes

#### SVG Assets Structure
- **Cards**: Individual card designs (suits and ranks)
  - Card fronts (ace_of_hearts.svg, 7_of_clubs.svg, etc.)
  - Card back (card_back.svg)
  - Suit symbols (hearts.svg, diamonds.svg, clubs.svg, spades.svg)
- **UI Elements**: Interface components
  - Buttons (button_normal.svg, button_hover.svg, button_pressed.svg)
  - Panels (panel_background.svg)
  - Icons (diamond_icon.svg)
  - Game logo (game_logo.svg)

### UI Elements
- Main menu with play, rules, shop, and settings options
- In-game UI with player scores, turn indicator, and hint button
- Diamond counter and shop interface
- Game over screen showing final scores
- Tutorial system with step-by-step instructions

## Development Setup

### Requirements
- Unity 2020.3 or newer
- Basic understanding of C# and Unity

### SVG Integration
The project uses a custom SVG import system. To properly display SVG assets:
1. SVG files are stored in the Resources folder
2. The SVGImporter class handles loading and conversion
3. For production, you may want to:
   - Install an SVG plugin from the Unity Asset Store for better performance
   - Or pre-render SVGs to textures for platforms with limited SVG support

### Getting Started
1. Clone this repository
2. Open the project in Unity
3. Open the MainMenu scene
4. Press Play to test the game

### Building
1. Go to File > Build Settings
2. Add all scenes to the build
3. Select your target platform
4. Click Build

## Future Improvements
- Add animations and sound effects
- Implement online multiplayer
- Add more card themes and backgrounds
- Create tournaments and challenges
- Expand the monetization options
- Optimize SVG rendering for mobile platforms

## License
This project is open source under the MIT license.

## Credits
Developed by [Your Name] for [Purpose/Class/etc.] 