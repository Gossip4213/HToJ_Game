<img width="1516" height="855" alt="Main-UI showcase" src="https://github.com/user-attachments/assets/ca116a02-48c3-4a98-a17b-b1a0096656c6" /># Heads, Tails, or Justice? (HToJ)
![Unity](https://img.shields.io/badge/Unity-6.0-black?style=flat&logo=unity)
![Language](https://img.shields.io/badge/Language-C%23-blue?style=flat&logo=csharp)
![Status](https://img.shields.io/badge/Status-Prototype-orange?style=flat)
> A scalable, data-driven Visual Novel (AVG) framework developed in Unity & C#. Designed as a potential platform for behavioral data collection and decision-making analysis.
> *Screenshot coming soon...*

## 📖 Introduction

**HToJ** is an independent development project exploring the intersection of interactive narrative and behavioral data tracking. Built on the **Unity Engine**, this framework provides a robust architecture for non-linear storytelling, featuring a custom serialization system, decoupled UI events, and a modular narrative configuration pipeline.

While presented as a Visual Novel, the underlying architecture is engineered to serve as a **psychological experimental platform**, capable of presenting complex scenarios and logging detailed user decision metrics.

## Features

* **Data-Driven Narrative Engine**: Utilizes **ScriptableObjects** (`CaseData`) to decouple story content from game logic, allowing for rapid iteration of experimental scenarios without recompiling code.
* **Robust Persistence System**: Features a custom **JSON-based multi-slot save/load system** built on `System.IO`, managing complex game states including timestamps, chapter progress, and decision history.
* **Reactive UI & Dialogue System**: 
    * Asynchronous typewriter effects using **Coroutines**.
    * Dynamic portrait management with state-based highlighting.
    * "Thought Bubble" choice system with physics-based hover feedback.
* **Decoupled Architecture**: Implements **Singleton** managers (`GameSystem`) and **C# Actions/Delegates** to minimize dependencies between the logic layer and presentation layer.

## Tech Stack

* **Engine**: Unity 6.0
* **Language**: C# (.NET Standard 2.1)
* **Serialization**: `UnityEngine.JsonUtility` & `System.IO`
* **UI System**: Unity UGUI & TextMeshPro (TMP)

## Showcase

### 1. Main-UI

<img width="1516" height="855" alt="Main-UI showcase" src="https://github.com/user-attachments/assets/ec4e1086-2988-416e-b532-c167c426bd5d" />
A fantasy-themed, anime UI designed to set an immersive tone for the visual novel and the data-collection framework.

### 2. Settings-UI
<img width="1916" height="1079" alt="settings-UI showcase" src="https://github.com/user-attachments/assets/687005ce-58a4-4c8e-b162-a698cc5e48bd" />
Game-setting options allow users to adjust resolution, audio levels, and text playback speed. 
These features ensure accessibility and accommodate individual participant reading preferences during experimental sessions.

### 3. Save/Load Architecture
<img width="1917" height="1076" alt="S L-UI showcase" src="https://github.com/user-attachments/assets/049b01ae-5734-4e5e-942e-126928b2d994" />
A visualized interface for managing local persistence data across multiple slots.

## 👤 Author
**Xinbo Gao**
* MScR Neuroscience Student @ University of Edinburgh
* Focus: Computational Neuroscience, Neural Dynamics, decision-making

## 📂 Project Structure

```text
Assets/Main/
├── Scripts/
│   ├── Core/           # Core Systems (GameSystem.cs, SaveData.cs)
│   ├── UI/             # UI Logic (MainMenu, SaveLoadUI, ButtonEffects)
│   ├── Gameplay/       # Game Logic (DialogueController, ChoiceManager)
│   └── Data/           # ScriptableObject Definitions (CaseData, Options)
├── Resources/
│   └── Cases/          # Narrative Data Files
└── Prefabs/            # UI & Object Prefabs

