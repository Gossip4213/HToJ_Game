# Heads, Tails, or Justice? (HToJ)

![Unity](https://img.shields.io/badge/Unity-2022_LTS-black?style=flat&logo=unity)
![Language](https://img.shields.io/badge/Language-C%23-blue?style=flat&logo=csharp)
![Narrative](https://img.shields.io/badge/Engine-Ink-ff69b4?style=flat)
![Status](https://img.shields.io/badge/Status-In_Development-orange?style=flat)

> A scalable, hybrid-architecture Visual Novel framework developed in Unity.
> Designed to explore the intersection of **interactive narrative**, **moral decision-making**, and **behavioral data collection**.

<div align="center">
<img width="100%" alt="Main-UI showcase" src="https://github.com/user-attachments/assets/ca116a02-48c3-4a98-a17b-b1a0096656c6" />
</div>

## 📖 Introduction

**HToJ** is a research-oriented development project. While presented as a fantasy visual novel, its underlying architecture is engineered as a **psychological experimental platform**.

The project adopts a **Hybrid Narrative Architecture**, combining the **Ink** language for fluid branching storytelling with Unity's native **ScriptableObjects** for handling complex, high-stakes moral decision events ("Cases"). This allows for decoupled tracking of narrative flow and quantitative behavioral metrics.

## 🛠️ Technical Features

### 1. The "Hybrid" Narrative Engine
* **Ink Integration**: Utilizes the **Ink** language (by Inkle Studios) to manage the linear narrative flow, branching dialogue, and variable tracking. This separates story content from game logic, preventing "spaghetti code."
* **Case System (ScriptableObjects)**: Complex moral dilemmas are encapsulated as Unity `ScriptableObjects`. Ink triggers these "Cases" via Tags, handing control over to a specialized UI for weighing evidence and making judgments.

### 2. Scalable Systems
* **Custom Localization (L10N)**: A lightweight, JSON-based localization system. Supports dynamic language switching (ZH/EN/JP) at runtime without scene reloading, utilizing C# Actions/Delegates for observer-pattern UI updates.
* **Robust Persistence**: A custom `System.IO` based save/load system. Serializes complex game states—including Ink variables, chapter progress, and decision history—into local JSON files.

### 3. AI-Assisted Asset Pipeline
This project leverages generative AI to optimize the solo-developer workflow:
* **Visual Arts**: **Stable Diffusion** for character design and environment conceptualization.
* **Voice Acting**: **VVC (Voice Changer)** technology to generate character voices.

## 💻 Tech 

| Category | Technology | Usage |
| :--- | :--- | :--- |
| **Engine** | Unity 2022 LTS | Core Framework |
| **Language** | C# (.NET Standard 2.1) | Game Logic, Custom Systems |
| **Narrative** | **Ink** & Ink-Unity Integration | Dialogue Flow & Logic |
| **Data** | JSON / ScriptableObject | Localization, Save Data, Game Config |
| **UI** | UGUI & TextMeshPro | Interface & Layout |

## 📸 Showcase

### Main Menu
<img width="100%" alt="Main-UI showcase" src="https://github.com/user-attachments/assets/ec4e1086-2988-416e-b532-c167c426bd5d" />
A fantasy-themed, anime UI designed to set an immersive tone for the visual novel and the data-collection framework.

### Settings Interface
<img width="100%" alt="settings-UI showcase" src="https://github.com/user-attachments/assets/687005ce-58a4-4c8e-b162-a698cc5e48bd" />
Allows adjustment of resolution, audio, and text speed to accommodate participant preferences.

### Save&Load  
<img width="100%" alt="S L-UI showcase" src="https://github.com/user-attachments/assets/049b01ae-5734-4e5e-942e-126928b2d994" />
A visualized save&load system for local data and game progress.

## 👤 Author

**Xinbo Gao**
(Gossip4213)
* MScR Neuroscience Student @ University of Edinburgh

* Focus: Computational Neuroscience, Neural Dynamics, Decision-making, Moral decision, multi-languages cognition.

## 📂 Project Structure

```text
Assets/
├── Main/
│   ├── Scripts/
│   │   ├── Core/           # GameSystem (Singleton), LocalizationManager
│   │   ├── Gameplay/       # DialogueController (Ink Bridge), ChoiceManager
│   │   ├── UI/             # LocalizeUI, Menus
│   │   └── Data/           # PlayerProfile, JSON Data Structures
│   ├── Story/              # .ink files and compiled JSON assets
│   └── Resources/          # ScriptableObjects (Cases) & Audio
└── StreamingAssets/        # localization.json (Hot-swappable text)
