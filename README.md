# Wheel of Fortune - Unity Demo

### Project Overview
This project is a 3D Wheel of Fortune game developed in **Unity 2021 LTS** as part of the Vertigo Games Game Developer Demo. Players spin the wheel to collect various rewards across multiple zones. The game features a risk-reward mechanic where hitting a bomb results in the loss of all collected rewards unless the player chooses to "Leave" or uses a revive.

### Core Features
* **Dynamic Wheel System**: Slice content and rewards are fully changeable from the Unity Editor using Scriptable Objects.
* **Zone Logic**: 
    * **Normal Zones**: Includes multiple rewards and a bomb.
    * **Safe Zones**: Every 5th zone is a risk-free silver spin without a bomb.
    * **Super Zones**: Every 30th zone is a risk-free golden spin with special rewards.
* **Leave & Collect**: Players can choose to walk away and collect all rewards when the wheel is not spinning, provided they are in a safe or super zone.
* **Revive System (Bonus)**: Implementation of a continue system using currency as a bonus feature.

### Technical Implementation
* **SOLID & OOP**: Developed with a focus on a reusable, maintainable, and scalable codebase using OOP principles.
* **UI Architecture**:
    * **Aspect Ratio Compatibility**: Designed to be visually acceptable and compatible with 20:9, 16:9, and 4:3 ratios.
    * **No Editor Events**: Does not use Unity OnClick or event references from the Editor.
    * **Code-Driven References**: Button references are automatically set via `OnValidate`.
    * **Naming Conventions**: Changeable UI elements end with the `_value` suffix (e.g., `ui_text_cash_value`).
    * **Separated Animators**: UI animators are placed in separated transforms rather than the root transform.
* **Optimization**:
    * **Raycast Targets**: Disabled for unnecessary Image components to optimize performance.
    * **TextMeshPro**: Utilized for all UI text elements.
    * **Asset Usage**: Proper usage of Dotween for animations and Sprite Atlases for UI efficiency.

### Tech Stack
* **Engine**: Unity 2021 LTS
* **UI**: TextMeshPro
* **Animation**: DOTween
* **Design Patterns**: SOLID and Refactoring Guru principles

### How to Run
1. Download the **Release APK** from the GitHub repository.
2. Install on an Android device or emulator to test the game functionality.
3. The project source code is available in the repository for review of the technical implementation.
