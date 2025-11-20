**🚁 Drone Combat Training -- Prototype**

A Unity-based drone simulation featuring a guided tutorial, enemy AI
combat, dynamic camera views, and realistic drone controls. Designed for
PC and Mobile platforms as part of a drone simulation assessment.

**📸 Preview**

**(Replace these images with your own screenshots/GIFs)**

\<p align=\"center\"\> \<img src=\"images/screenshot1.png\"
width=\"45%\" alt=\"Screenshot of drone flying in an environment\"\>
&nbsp; &nbsp; &nbsp; \<img src=\"images/screenshot2.gif\" width=\"45%\"
alt=\"GIF showcasing combat or movement\"\> \</p\>

**🎮 Controls**

The controls are designed for an arcade-style, smooth flight experience.

  ------------------------------------------------------------------------
  **Action**   **Key       **Description**
               (PC)**      
  ------------ ----------- -----------------------------------------------
  **Start      G           Toggles the drone engine (must be on for
  Engine**                 flight/takeoff).

  **Move       W           Applies forward thrust.
  Forward**                

  **Move       S           Applies backward thrust (uses
  Back**                   backSpeedMultiplier for smoother, slower
                           movement).

  **Rotate     A / D       Applies Yaw rotation.
  Left /                   
  Right**                  

  **Move Up**  Q           Increases altitude (Vertical thrust up).

  **Move       E           Decreases altitude (Vertical thrust down).
  Down**                   

  **Shoot**    Left Mouse  Fires the weapon system.
               Click       

  **Switch     C           Cycles between available camera views.
  Camera**                 
  ------------------------------------------------------------------------

**🧭 Tutorial & Mission Flow**

The prototype guides the user through the mechanics via a step-by-step
tutorial system.

1.  **Start Engine:** Press G to enable drone power.

2.  **Takeoff:** Use Q/E or directional inputs to lift off and reach
    hover height.

3.  **Basic Movement:** Practice free flight to master control inputs.

4.  **Camera Switching:** Press C to cycle between Wide, POV Cockpit,
    and Top View cameras.

5.  **Pass the Rings:** Complete navigation training by flying through
    animated checkpoint rings.

6.  **Shooting Practice:** A dummy target bot spawns for testing
    bullets, hit-VFX, and destruction logic using Left Click.

7.  **Enemy Combat Mode:** Five enemy AI drones activate, unlocking full
    combat scenarios.

**🤖 Features Implemented**

**✈️ Drone System**

-   Smooth, arcade-style drone flight controller (using C# for physics
    and smoothing).

-   Dynamic camera switching: Cockpit (POV), Wide, and Top View cameras.

-   Collision detection and basic health system.

-   Drone explosion VFX upon destruction.

**🔫 Combat System**

-   Player-controlled bullet shooting mechanics.

-   Enemy AI drones with dedicated scripts.

-   Patrol behavior for enemies.

-   Detection radius system triggers engagement and firing logic.

-   Enemy crash animation and destruction VFX.

**🎯 Mission / UI**

-   Ring-based path navigation system.

-   Task-by-task guided tutorial and mission flow.

-   On-screen UI prompts for user guidance.

-   Damage indicators.

**🔧 Tech Stack**

  -----------------------------------------------------------------------------
  **Component**   **Technology**     **Role**
  --------------- ------------------ ------------------------------------------
  **Engine**      Unity 2022/2023    Core game environment and rendering.

  **Scripting**   C#                 All game logic, including drone control,
                                     state machine, and combat.

  **AI**          Unity NavMesh      Enemy movement, pathfinding, and patrol.
                  Agents             

  **VFX**         Unity Particle     Explosions, bullet impacts, and engine
                  System             effects.

  **UI**          Unity Canvas       All user interfaces and heads-up displays.

  **Platform**    PC (EXE) + Mobile  Targeted build platforms.
                  (APK)              
  -----------------------------------------------------------------------------

**🔮 Future Improvements**

-   Enhanced VFX pipeline (muzzle flash, smoke, HDR glare).

-   Integration of high-quality engine and shooting sound effects.

-   Dedicated mobile joystick and touch controls.

-   Improved enemy pathfinding and behavior tree for more dynamic
    combat.

-   Introduction of a final boss fight mission.

-   Persistent save system.

**📦 Build Output**

  -----------------------------------------------------------------------
  **Platform**             **File**
  ------------------------ ----------------------------------------------
  Windows                  DroneCombat.exe

  -----------------------------------------------------------------------

**📝 How to Play**

1.  Download the appropriate build (EXE or APK).

2.  Launch the tutorial scene.

3.  Follow the on-screen steps to complete training.

4.  Engage and defeat the enemy drones.

5.  Complete the mission!
