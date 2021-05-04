# GE2-Assignment: A Space Battle

Name: Damian Wojtowicz<br>
Student Number: C17413722<br>
Course Code: DT228/4<br>

# Production
This section of the report will discuss the final end-result of the project. For the idea generation and proposal see the next section (Proposal).

## Sequence of Events/Story Summary (Spoilers)
- A scout arrives at the station, a supposed anomaly was discovered in an asteroid field.
- The scout turns around to face the asteroid field and creates a squad.
- Idle ships at the station join the squad and follow the scout (now leader of squad) into the asteroid field.
- The squad encounteres enemies and a battle follows.
- The enemies are defeated and the squad exits the asteroid field

Video of Project:
[![Scene Video](https://i.imgur.com/vfdmwen.png)](https://youtu.be/Z0EBRiennwo)

### States & Behaviours
Below is a diagram of the states in this project. The sequence is as followes:
1. First a fighter ship (scout) with the DeliverMessage state heads and arrives at the station.
2. Scout states changes to PrepareScoutDeployment which involves turning around to face the asteroid field. This is achieved with a custom behaviour, FaceDestination. This behaviour was designed to make the ship turn around naturally without changing the original position. The behaviour was also designed to be dynamic so that the starting orientation of the ship and the location they want to face can be changed.
4. Once the ship faces the correct location it creates a squad. A squad allows for a specified number of ships to fly together in formation. The formation is scalible and is generated. The squad is also dynamic, meaning that if the leader dies a new leader is automatically allocated from within the squad and if a member dies the other ships rearrange to maintain the formation.
5. When the scout becomes a squad leader the other ships that are idle within the vicinity will join the squad until the squad's maxiumum predefined capacity is reached. These ships go from the IdleState to the FollowerState.
6. The leader changes to the DeployToAsteroidField state and moves towards the asteroid field. 
7. Looking at the diagram below you may notice that there is no further state after DeployToAsteroidField. This is because within the asteroid field are enemies and the global Alive state changes the the Attack state when the fighter has a target selected. Once all the enemies die the Attack state changes to the IdleState. From the IdleState, since there are no more enemies the leader changes to ExitAsteroidField and then to ReturnToStation. In other words the DeployToAsteroidField state is only needed to reach the enemies.
8. When the squad encounters enemies the squad breaks up momentarily as the state of the followers changes from the FollowerState to the AttackState. Once the fight is over the followers return to the FollowerState via the IdleState. On the side note, althogh the reallocation of members within the squad may look a little messy when a member dies this is not a problem because the members do not maintain formation when fighting (and dying as a result).
9. The patrol state does not change to anything because the ships in the beginning scene will move around the station endlessly. The patrol state is also predefined.
10. When the fighter ship gets too close to the enemy ship the TemporaryRetreatState is activated. It involves the Flee behaviour. Once the ship is far enough the SmoothTurnaround behaviour is activated. This is so that the ship does not turn 180 degrees in an instant to face the enemy again but instead turns in a more natural manner.

Diagram of States:
![States](Images/States.png?raw=true "States")

More Indepth View of States:
![WrittenStates](https://i.imgur.com/m89D5e1.png)


### Scripts & Behaviours
Explanation of FaceDestination and Squad behaviours:
https://youtu.be/dvBfNuWoA2E

1. FaceDestination<br>
This behaviour is used to make the ship turn naturally to face any direction and end up at the same starting position.
It works by generating points in a circle around the ship such that after following those points the ship will face the desired position. I'm not sure if it appears complex but it took a long time to figure out and get right.<br>

2. MainManager<br>
This script is used to manage the camera and the music. It works by using a sequence number that is incremented as the story progresses.<br>

3. SmoothTurnaround<br>
This behaviour is used to turn the ship around progressively (without regard for position). Used in the TemporaryRetreatState when the fighter gets too close to the enemy target, needs to retreat, and then turn around. This behaviour also took a while to figure out as several implementations/ideas were tried such as using the "right" direction to incrementally steer the ship. Another challege was the math of getting the difference between two degrees. For example, the difference between 10 degrees and 350 degrees is 20 degrees and not 340. The difference was used to determine if the ship has turned around enough and took a while to wrap my head around.<br>

4. Squad<br>
This script is used to manage the desired positions of members in the squad. The fighter calls a method on this class to get the position it needs to go to. Should the leader or other members die the positions of the members will be rearranged. To go to the desired position the fighter uses an modified OffsetPursue behaviour that was covered on the course. The code has to be refactored and reworked as additional requirements were added and this behaviour also took a while to get right.<br>

6. Asteroid<br>
This script simply removes the bullet that collides with it.<br>

7. RotatingAsteroid<br>
This script generates a random rotation for the asteroid and rotates it. Adds more life to the asteroid field.

8. Explosion<br>
Invoked by the Dead state when the fighter dies. The explodable parts of the ship are given a random direction and a set velocity. If the fighter has a mesh then explodable parts are substituted and the mesh is hidden. Explosion sound is played. To create the particle effect a tutorial by Masanori Takano (https://styly.cc/tips/explosion01/). The process followed is the same, however the parameters such as time, size, colour etc. were tweaked to suit my liking. The code for the explosion was written by me.<br>

9. State Initiallizer<br>
This script allows for the global and non-global state to be provided as a string and that state will be set as the initial state of the ship. 

Scripts taken from the Games Engines 2 module (Dr. Bryan Duggan's GE2-2020-2021 repository):
Arrive, Boid, Bullet, Flee, FollowPath, ObstacleAvoidance (added collision ignore), Path, Pursue, Seek, StateMachine, SteeringBehaviour, OffsetPursue (modified).<br>
Although the code for the scripts taken from the module may not change much (if at all) the parameters were changed a lot of the time and their application is specific to the project.


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
Below are some of the images of the story.
![Space Station](Images/SpaceStation.png?raw=true "Space Station (Beginning)")
![Scouts Warping](Images/ScoutWarp.png?raw=true "Scouts Warping")
![Asteroid Field Investigation](Images/Investigation.png?raw=true "Asteroid Field Investigation")
![Reinforcements](Images/Reinforcements.png?raw=true "Reinforcements")

## Components/Behaviours That Could be Implemented:
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

