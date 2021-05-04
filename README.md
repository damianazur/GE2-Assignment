# GE2-Assignment: A Space Battle

Name: Damian Wojtowicz<br>
Student Number: C17413722<br>
Course Code: DT228/4<br>

# Production
This section of the report will discuss the final end-result of the project. For the idea generation and proposal see the next section.

## Sequence of Events/Story Summary
- A scout arrives at the station, an anomaly was discovered in an asteroid field.
- The scout turns around to face the asteroid field and creates a squad.
- Idle ships at the station join the squad and follow the scout (now leader of squad) into the asteroid field.
- The squad encounteres enemies and a battle follows.
- The enemies are defeated and the squad exits the asteroid field

### States & Behaviours
![States](Images/States.png?raw=true "States")
![WrittenStates](Images/WrittenStates.png?raw=true "WrittenStates")




# Proposal
## Idea Generation:
### **Small Ship Models**
  - Fighters, Bombers
  
### **Large ships**
  - Ship carriers, Battle cruizers
  
### **Scenery**
  - Sun, Planets, Asteroids, Destroyed Ship Debris
  
### **Behaviours**
  - Persuit, Flee, Formations, Fire Missles, Defend a Ship, Attack Target, Evade Missles, Fall Back Under Damage, Flanking

### **Animations/Effects**
  - Lazer bullets, Lazer beams, Ship explosions, Bombs, Engine Effects/Particles

### **Audio**
  - Shooting, Ship Explosion, Commander giving order, Crew members respinding
  - Music
    - Peaceful music in beginning
    - Battle music
    - Post battle music/ending

### **Story Elements**
  - Calling for reinforcements (both enemy and allies)
  - Destroying an enemy target (possibly a big ship)
  - Escaping/Fighting into/in an asteroid field
  - Battle fleets fighting or A small crew defeating a tough enemy

## Story:
### Introduction
- An anomaly has been reported to the commander
- Commander sends a small squad of ships to investigate the anomaly
- Ally ships warp near the location of anomaly
- Anomaly is located near an asteroid field 
- Ships investigate the area
- An enemy fleet that is stationed there is discovered

### Middle
- Enemy sends fighter ships to attack allies
- Some ally scouts escape and others stay to fight the squad of enemy ships and buy the escapng allies some time
- (they need time to prepare warp)
- A battle scene takes place, the ally ships are outnumbered and lose
- The surviving scouts report to commander, who requests reinforcements from another planet
- Reinforcements warp to space station
- Enemy fleet grows and once all enemies have arrived they warp to the station
- The enemies start attacking the planes stationed there. The situation is looking hopeless.
- Ally reinforcements arrive shortly and the real battle begins

### End
- The allies eventually win and successfully defend the station
- Ally ships get into formation outside the station

### Storyboard
Below are some of the images of the story
![Space Station](Images/SpaceStation.png?raw=true "Space Station (Beginning)")
![Scouts Warping](Images/ScoutWarp.png?raw=true "Scouts Warping")
![Asteroid Field Investigation](Images/Investigation.png?raw=true "Asteroid Field Investigation")
![Reinforcements](Images/Reinforcements.png?raw=true "Reinforcements")

## Components/Behaviours That Need to be Implemented:
- Fighter ship shoot (bullets with prediction)
- Cruizer ship shoot (beam)
- Warp
- Investigation (Search) behaviour
- Flee (When outnumbered)
- Formations/Squads (Moving as formation, Units are added to or removed from formation)
- Ship destruction
- Fire homing missle
- Evade missle (Barrel roll)
- Defend objective (attack enemies that approach the objective)
- Attack objective (destroy target)
- Arrive (Ships gather at objective)

