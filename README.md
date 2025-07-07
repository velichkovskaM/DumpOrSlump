
# Dump or Slump 🧹🧺🗑️ - Android Game
<img src="https://github.com/user-attachments/assets/a5cf3a1e-730e-4ecc-88cc-631f7477b1d2"
     align="left"
     width="120"
     alt="Project logo">
Race against time to clean your bedroom before you unleash Mom's wrath - or slump in defeat! Sort clothes, vacuum dust, and throw out paper while your mischievous brother and playful dog add to the mayhem. Dodge unexpected messes and keep your stamina intact — Mom’s scolding is no joke! Will you beat the ticking clock, and solve the puzzled game? This energetic, quirky game challenges your reflexes while balancing tidiness and disaster. Keep calm and clean on — before it’s too late! 

<br clear="left"> 

> Android game built with **MonoGame 3.8**, **C#** and **HLSL**

## Gameplay 🎮
|            | What you do | Win / Lose conditions |
|------------|-------------|-----------------------|
| **Goal**   | Pick up **dust, clutter and clothes**, then drop them in the right holder (trash can or wardrobe) before the timer expires | Clean *everything* **and** put the tools away before Mum’s final inspection |
| **Hazards**| A dog scatters extra dust when in close proximity; a sibling throws clutter at player; Mum barges in after a delay and will chase & hit player when in close range | Lose all stamina hearts **or** let the timer reach 0 while the room is still messy |
| **Levels** | Four story levels + main menu. Each level adds more mess, tighter time-limits, AI characters and gesture requirements (e.g. draw a circle around items in later levels) | Beat all 4 levels |

## Actions 💥
| Action | Gesture |
|--------|---------|
| Move player | Drag the **virtual joystick** |
| Pick up item | **Double-tap** the object (Levels 1-2); **Draw a circle** around it (Levels 3-4) |
| Interact / Use tool | Tap the context button pop-up |
| Pause | Allows access to Pause Menu with multiple setting options |
| Camera pan | Automatic when key events happen (Mum enters, Child appears) |

## Features 🚀
* **3D room with perspective camera**, hand-drawn sprites, 3D modeled environment in Blender lit by a simple per-pixel shader.
* **QuadTreeScene** spatial index keeps collision checks fast even on mobile.
* **Pixelation post-effect** for a retro vibe.
* **Finite-state AI** for Dog, Child and Mother – all in one update loop.
* **Dynamic music** that switches tracks as scenes change.
* **Save API** to remember volume, last-run stats and whether the player won or lost.

## Official Trailer 🎬
[![Dump or Slump Official Trailer](https://markdown-videos-api.jorgenkh.no/youtube/6R5senvOHmc)
](https://www.youtube.com/watch?v=6R5senvOHmc&list=PLX0hM3dSFDjWd2ZuCHAV66EeVbNAUSmIG "Opens on YouTube")

## Phone Gameplay Screenshots 🤳
<p align="center">
  <img src="https://github.com/user-attachments/assets/bac5a339-55b4-4d65-a76f-e23163e8bb89" width="400" alt="Basket screen">
  <img src="https://github.com/user-attachments/assets/08a61623-da9b-4595-9155-edc93f6020fa" width="400" alt="Boy screen"><br>
  <img src="https://github.com/user-attachments/assets/d6130855-adda-49b7-8500-ca01266b4003" width="400" alt="Dog screen">
  <img src="https://github.com/user-attachments/assets/5e26af03-8469-49c9-a0b3-f7f5fbfbc663" width="400" alt="Mom screen">
</p>

## Tablet Gameplay Screenshots 📱
<p align="center">
  <img src="https://github.com/user-attachments/assets/a4a546e6-30b3-4fff-9ceb-940e1a4de5b5" width="400" alt="Basket screen">
  <img src="https://github.com/user-attachments/assets/5c8d1c75-7495-40c8-be21-7e0966bbd192" width="400" alt="Boy screen"><br>
  <img src="https://github.com/user-attachments/assets/a2f46606-497b-415a-8bd8-5873ead38622" width="400" alt="Dog screen">
  <img src="https://github.com/user-attachments/assets/31550918-c419-4852-aea8-111c5697d2b0" width="400" alt="Mom screen">
</p>

