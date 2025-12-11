# Slime Lab (SLAB)

Slime Lab is a short experimental Unity game built as part of the **University of Central Florida’s AI in Game Programming course**.  
The project explores the design of a **procedural puzzle dungeon**: a dynamically generated maze filled with hazards, stealth segments, environmental puzzles, item pickups, and a challenging multi-phase boss encounter.

The player controls a small slime creature navigating through interconnected rooms, solving puzzles, collecting upgrades, and preparing for the final showdown.

This prototype received the **“Most Complete Game” award** for the course.

---

## Core Gameplay Features

### Procedural Dungeon Generation
Rooms are assembled dynamically from modular prefabs using rules that ensure:
- Valid dungeon layouts  
- Traversable paths  
- Varied gameplay pacing  
- A mix of puzzle, stealth, and challenge rooms  

Includes a **dynamic minimap** that updates as the player explores.

---

### Ice Puzzle Rooms
Some rooms contain ice tiles that reduce the player’s control and cause sliding.  
This mechanic:
- Forces careful movement  
- Opens the door to more thoughtful puzzle design  
- Combines with other hazards for increased tension  

---

### Patrolling Enemies (Stealth Rooms)
Enemy slimes patrol in fixed patterns and react when the player enters their vision cone.  
These rooms create:
- Lightweight stealth scenarios  
- Risk–reward navigation  
- A contrast between faster action rooms and slower puzzle rooms  

---

## Boss Encounter
The dungeon culminates in a **two-phase boss battle** with an intermission.

The boss uses:
- **3 unique abilities**
- **Multiple attack patterns**
- **Phase-dependent behaviors**

The fight is intentionally challenging if the player enters unprepared, but the dungeon contains:
- Movement speed boosts  
- Health containers  
- Other pick-ups to support different strategies  

---

## Technical Design & Systems

This project involved a variety of interconnected gameplay systems, including:

### **Procedural Level Generation**
- Modular room definitions via ScriptableObjects  
- Weighted room selection  
- Connection rules to maintain solvable layouts  
- Runtime room loading/unloading  
- Data-driven approach enabling future expansion  

### **Object-Oriented Systems**
The overall project showcased solid OOP structure through:
- Algorithmic level construction  
- Boss ability management  
- Minimap state tracking  
- Entity behaviors  

### **Data Structures**
Runtime efficiency was improved through:
- Queues for boss behavior cycles  
- Dictionaries/maps for room metadata  
- Simple caches to avoid redundant lookups  

---

## Team Context & My Role

This was a **team project**, and responsibilities were shared across design, programming, and implementation.

**My personal contributions included:**
- Implementing gameplay mechanics and puzzle interactions  
- Contributing to procedural generation logic and room systems  
- Implementing or refining movement and traversal systems  
- Helping integrate gameplay elements into the generated rooms  
- Debugging and iteration during the final development cycle  

Though I did not lead the project, I played a key part in core system development and overall gameplay implementation.

---

## Screenshots
(Place your images in a `/Screenshots` folder and link them here.)

Example:
```md
![screenshot1](Screenshots/screenshot1.png)
![screenshot2](Screenshots/screenshot2.png)
