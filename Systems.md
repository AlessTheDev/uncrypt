## Systems Index
1. [Dialogue System](#dialogue-system)  
2. [Singletons Management](#singletons-management)  
   - [SceneSingleton](#scenesingleton)  
   - [PersistentSingleton](#persistentsingleton)  
3. [Game Bootstrapper](#game-bootstrapper)  
4. [Behavior Tree AI](#behavior-tree-ai)  
5. [Terminal System](#terminal-system)  
6. [State Machine System](#state-machine-system)  
7. [Core Scripts](#core-scripts)  
8. [Enemies](#enemies)  
9. [Dialogue & Companion System](#dialogue--companion-system)  
10. [UI Systems](#ui-systems)  
11. [Utility Systems](#utility-systems)  
12. [NPCs](#npcs)

### Dialogue System

**Path:** `/Assets/Scripts/DialogueSystem/`
This system powers interactive character conversations, integrates with companion behavior, and supports action triggering mid-dialogue.
**Key Files:**
- `DialogueManager.cs`, `DialogueExecutor.cs`: Logic and control
- `DialogueUIManager.cs`, `OptionUI.cs`: UI handling
- `Dialogue.cs`: Dialogue data model
**Features:**
* JSON-based format for dialogue creation
* Visual editor: [dialogue-editor](https://github.com/AlessTheDev/dialogue-editor)
* Event action system for triggering in-game events

Example:
```json
[  
  {    
    "dialogueId": "arrived_to_filesystem_1",  
    "characterId": "companion",  
    "sentence": "Okay, we've finally arrived at our destination",  
    "nextDialogue": "arrived_to_filesystem_2",  
    "actions": ["PlayAnimation|A_Talk"],  
    "options": []  
  },  
  {    
    "dialogueId": "arrived_to_filesystem_2",  
    "characterId": "companion",  
    "sentence": "I hope you understand how the firewall works",  
    "nextDialogue": "arrived_to_filesystem_3",  
    "actions": [],  
    "options": []  
  }
]
```

---
### Singletons Management

**Path:** `/Assets/Scripts/`
Uncrypt uses two custom singleton base classes to streamline access to global and scene-specific systems, reducing boilerplate and ensuring correct initialization across gameplay areas.

#### SceneSingleton

`SceneSingleton<T>` ensures **one unique instance per scene**. When the scene changes, the instance is automatically reset. This is used for managers that don't persist across levels but need easy access (e.g., `GameManager`).

**Key Features:**

* Scene-local scope with auto-reset on scene unload
* Subclassing support with `OnAwake()` and `Clean()` lifecycle hooks
* Warning log if overwritten within the same scene

**Example:**

```csharp
public class GameManager : SceneSingleton<GameManager>
{
    protected override void OnAwake()
    {
        // Initialize scene-level systems
    }

    protected override void Clean()
    {
        // Remove listeners and references
    }
}
```

#### PersistentSingleton

`PersistentSingleton<T>` is used for systems that **persist across all scenes**, like input management or audio. It enforces a true singleton pattern: duplicate instances are destroyed at runtime.

**Key Features:**

* Persistent across scenes using `DontDestroyOnLoad`
* Auto-destruction of duplicate instances
* Initialization logic in `OnAwake()`

**Example:**

```csharp
public class InputManager : PersistentSingleton<InputManager>
{
    public InputActions InputActions { get; private set; }

    protected override void OnAwake()
    {
        InputActions = new InputActions();
        InputActions.Enable();
    }
}
```

By using this system, we maintain clean access patterns, minimize coupling, and avoid the risks of Unity's default `FindObjectOfType` or manual singleton setups.

---

### Game Bootstrapper

Path: `/Assets/Scripts/GameBootstrap.cs`
This script ensures critical systems (like input, audio, or persistent managers) are loaded before any scene runs and persist throughout the game session.

#### Purpose
The GameBootstrap class loads a prefab (GameBootstrap.prefab) from the Resources folder at startup and calls DontDestroyOnLoad on it. Any systems required across all scenes (e.g., InputManager, AudioManager) should be placed as children of this prefab to ensure they persist.

#### Initialization Flow
1. Triggered automatically using RuntimeInitializeOnLoadMethod with BeforeSceneLoad.
2. Loads the GameBootstrap prefab from Resources/.
3. Instantiates the bootstrap object and prevents it from being destroyed on scene change.

Use this as a root object to attach persistent systems like:
InputManager (via PersistentSingleton)
AudioManager

Note:
A future improvement is already planned (see TODO in source): Refactoring the bootstrap logic to load dynamically based on save file dependencies, enabling greater flexibility in scene and state management.

---

### Behavior Tree AI

**Path:** `/Assets/Scripts/BehaviourTree/`
Enables modular and extensible AI via behavior trees. Used for defining complex enemy behaviors through reusable strategy components.

**Key Files:**
* Core: `BehaviourTree.cs`, `Node.cs`, `Selector.cs`, `Sequence.cs`
* Strategies: Include `Action`, `Condition`, `PatrolStrategy.cs`, etc.

> [!DANGER] Current implementation has known bugs. A custom implementation is advised for production.

---

### Terminal System

**Path:** `/Assets/Scripts/Terminal/`
Simulates cybersecurity tools integrated with gameplay levels.

**Core Scripts:**

- `TerminalManager.cs`, `Commands.cs`, `FileSystem.cs`: Command execution and virtual filesystem
- `TerminalWriter.cs`, `TerminalUIOpenAnimation.cs`: Interfaces and outputs

**Use Cases:**
* Port scanning (Firewall level)
* File browsing 

---

### State Machine System

**Path:** `/Assets/Scripts/StateMachine.cs` and related folders
Modular state management system used across player and companion behaviors.

**Components:**
- `State.cs`, `StateMachine.cs`: Generic base logic
- `Player/StateMachine/`: States like `DashState`, `Idle`, `Running`
- `Companion/States/`: States like `FollowPlayer`, `LeadPlayer`

---
### Core Scripts

Located under `/Assets/Scripts/`:

- `GameManager.cs`, `GameBootstrap.cs`: Game lifecycle
- `InputManager.cs`: Input handling
- `PersistentSingleton.cs`: Cross-scene data

---

### Enemies
Includes base class `Enemy.cs` and variations like `Carbon.cs`, `Samurai.cs`, `FireWallWizard.cs`, and more. Each enemy uses unique behaviors and sometimes ties into the behavior tree system.

---

### Dialogue & Companion System

* Companion behavior logic: `CompanionController.cs`, `CompanionDialogueExecutor.cs`
* Integrates with Dialogue System and State Machine
---

### UI Systems

Focuses on in-game UI, dialogues, interactions, death screens, and puzzle interfaces. (`PauseMenu.cs`, `PlayerUIManager.cs`, `FileSystemPuzzleUI.cs`)

---

### Utility Systems

Used across systems for common tasks like saving, camera transitions, object pooling and generating objects.

---

### NPCs

Simple NPC logic for arena guards, etc.

---