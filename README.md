# Heads, Tails, or Justice? (HToJ)
> A scalable, data-driven Visual Novel (AVG) framework developed in Unity & C#. Designed as a potential platform for behavioral data collection and decision-making analysis.


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

### 1. Dialogue & Portrait System
Dynamic character highlighting and typewriter text effects ensure an immersive reading experience.

### 2. Decision Making (The "Thought Bubbles")
Interactive choice generation with hover animations (scaling/rotation) to enhance user engagement during critical decision points.

### 3. Save/Load Architecture
A visualized interface for managing local persistence data across multiple slots.

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
