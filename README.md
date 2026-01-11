# Șeptică - Online Multiplayer Card Game
[![Ask DeepWiki](https://devin.ai/assets/askdeepwiki.png)](https://deepwiki.com/devero11/septica-online-multiplayer-card-game)

This repository contains the Unity client for an online multiplayer version of Șeptică, a popular Romanian trick-taking card game. The project is built using the Unity engine and utilizes the Nakama server for real-time multiplayer functionality, social features, and data management.

## Features

- **Real-time Multiplayer Gameplay:** Supports matches for 3 players, 4 players, and teams.
- **Social Integration:** Features a comprehensive friends system allowing users to search, add, remove, and view pending/accepted friend requests.
- **Lobby System:** Create and join game lobbies to play with friends. The lobby leader can configure the game mode and start the match.
- **Matchmaking:** Find public games for various modes (3-player, 4-player, teams) through a matchmaking system.
- **User Authentication:** Players can create a persistent account with a unique username or play as a guest with a randomly generated ID.
- **Competitive Leaderboards:** Track your performance with global and "around me" leaderboards to see how you stack up against other players.
- **Interactive UI:** A responsive user interface built with Unity's UI Toolkit, including drag-and-drop card mechanics, in-game pause menus, and winner announcements.
- **Persistent Player Data:** Player rank, level, and coins are stored and managed through Nakama's storage engine.

## Technology Stack

- **Game Engine:** Unity
- **Backend & Networking:** [Nakama](https://heroiclabs.com/nakama/)
- **UI Framework:** Unity UI Toolkit (UXML)
- **Primary Language:** C#

## Project Structure

The project is organized to separate concerns, making it easier to navigate and understand.

-   `Nakama Implementation/`: Contains all scripts responsible for communication with the Nakama server.
    -   **`NakamaConnection.cs`**: A `ScriptableObject` that acts as the central hub for managing the client, session, socket, and handling server events like match state changes and notifications.
    -   **`ClientManager.cs`**: Manages the initial connection and holds the reference to the `NakamaConnection` object.
    -   **`matchFinderManager.cs`**: Handles matchmaking requests and scene transitions upon finding a match.
-   `MenuUiCode/`: Contains C# scripts that drive the logic for the UI Toolkit (`.uxml`) documents.
    -   **`TabScript.cs`**: Manages the main menu navigation (Home, Friends, Leaderboard, Shop).
    -   **`FriendsTabScript.cs`**: Powers the friends list and add friend functionality.
    -   **`Leaderboard.cs`**: Fetches and displays records for the leaderboards.
-   `Misc/`: A collection of utility scripts.
    -   **`lobbyScript.cs`**: Manages the UI and logic for the game lobby, including inviting friends and starting matches.
-   `Scenes/`: Contains all Unity scenes for the game, including login, main menu, and the different game tables (3-player, 4-player, Teams).
-   **`*.uxml` files**: Define the structure and layout of the user interface using Unity's UI Toolkit.

## Setup and Installation

To run this project locally, you will need a Unity Editor and a running Nakama server instance.

1.  **Clone the repository:**
    ```bash
    git clone https://github.com/devero11/septica-online-multiplayer-card-game.git
    ```

2.  **Set up Nakama Server:**
    -   Follow the [Nakama documentation](https://heroiclabs.com/docs/nakama/getting-started/install/) to install and run a Nakama server instance. Docker is the recommended method.
    -   This client relies on server-side RPC functions (e.g., `search_users`, `send_invite`, `threePlayerLobby`). You will need to deploy the corresponding Lua or Go modules to your Nakama server.

3.  **Configure the Unity Project:**
    -   Open the cloned project in the Unity Editor.
    -   Navigate to `Assets/Nakama Implementation/` in the Project window.
    -   Select the `localConnection.asset` Scriptable Object.
    -   In the Inspector, update the `Scheme`, `Host`, `Port`, and `Server Key` fields to match your Nakama server's configuration.

4.  **Run the Game:**
    -   Open the `Assets/Scenes/LogInMenu.unity` scene.
    -   Press the **Play** button in the Unity Editor to start the client. You can create a new user or join as a guest.