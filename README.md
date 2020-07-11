# Propellion
A sci-fi parkour game built in Unity 3D

## Characters
The protagonist is a misfit twenty-something named Proto from planet Pellion, who always wanted to be a starship captain for the Galactic Alliance like his older brother. However, he flunked out of the Galactic Fleet Academy and lost his direction. With nowhere to turn, he decided to go through the rigorous training required to become a space ninja. He now has only a simulated agility course left to master before his training is complete and he finally earns full-fledged ninja status.

This final phase of training is administered by a snarky robot AI named B3-M1S (pronounced “Bemis”). This robot is programmed to demean the trainees so that they become determined to master the course to free themselves of these machine-learning-generated insults. B3-M1S is also repurposed in some of the space ninja equipment as a voice-assistant.

Space ninjas are a highly respected organization of mercenaries allied with the Galactic Alliance, who specialize in eliminating space yakuza and protecting asteroid mining fields from hostile alien forces (such as Fungos) who try to steal the Galactic Alliance’s precious ore. The organization goes by the name of S.N.A.K.E. - Space Ninja Agency of Killing Evil.

Fungos are a species of mutant mushroom aliens who have always refused to join forces with the Galactic Alliance, and instead resort to stealing their ore and destroying their ships. These Fungos are simulated as hostile forces to avoid in the agility course.

The Galactic Alliance is a large interspecies government body with dominion over most planets of the galaxy.

## Story Narrative
The game opens aboard the S.N.A.K.E space station training facility with B3-M1S explaining the final phase of training to Proto: an agility course administered as a series of simulated lessons. Each lesson furthers the player in their training, eventually leading to the second stage of the course where they gain permission to use a space katana, the most deadly tool in a space ninja’s arsenal. Once the lessons are complete, Proto is congratulated by B3-M1S, and they exit the simulation room and enter an equipment room containing Proto’s very own grapple gun, space katana, space suit, and comms helmet. When the helmet is placed on his head, he’s greeted by the voice assistant version of B3-M1S, who says something snarky. 

At that very moment, the door to the room shuts, and the player hears a loud explosion and an alarm. Suddenly, the door opens again, and the room appears to be floating out in space with debris from the S.N.A.K.E facility drifting around and no one in sight. Off in the distance is an alien spaceship flying away. B3-M1S informs the player that the training facility might have been attacked by vengeful Fungos who stole a Galactic Alliance war vessel allowing them to get past the S.N.A.K.E facility’s cloaking system. The Fungos hijacked the space station’s navigation system, and steered it into an asteroid field, where it exploded from the numerous high-velocity impacts. Proto must now use his experience from the simulation in real life. The player navigates through the wreckage amidst the asteroid field to the Fungo ship and slices it with their katana. B3-M1S thanks Proto and makes a snarky comment about how there’s going to be a lot of cleaning up to do.

*(Fade to black and credits roll)*

## Game World
The space ninjas’ headquarters and ninja student training facility are located in a space station, orbiting the planet NJ-64209, cloaked from all ships that are not registered with the Galactic Alliance. They accept any and all misfits to undergo training as space ninjas — if they can handle it.

The simulations take place in an asteroid field, with a large, expanding black hole behind the player, sucking in asteroids and the player themself if they get too close.

## Gameplay
The core gameplay consists of the player grappling through an asteroid field, trying to reach the simulation’s exit portal, while escaping from a black hole expanding and chasing them from behind. The parkour mechanics are fast-paced because the player is in constant threat of being sucked into the black hole or smashing into an asteroid if they don’t keep moving. The player must also avoid hostile Fungos who will try to damage the player with their laser guns. If we have time, we will add an endless mode with no black hole and no exit portal, just to see how long the player can last.

The player is playing the game as part of a training simulation for an organization of space ninjas — the asteroid field is an agility course. The long-term game goal is to complete the space ninja training. The game falls into several genres, including sci-fi, arcade, and parkour.

## Game Mechanics

*(**Bold** means only if we have time to implement it)*

#### All Agents and Objects
- Affected by zero gravity physics

#### Player
- Make contact with asteroid
  - Take damage
  - Lose momentum
- Grapple with Grapple Gun (limited range)
  - Hit an Asteroid
    - Shoots to point of click and clamps down
  - Hit an Alien
    - **Player flings towards alien and alien flings towards the player**
    - *If we don’t have time to implement the above, grapple fails to clamp down, and auto retracts with a delay of control*
  - Shoot at target out of range
    - Fails to clamp down, and auto retracts with a delay of control
  - Hit a Medical Canister
    - Grapple auto-retracts back to the player, holding the Medical Canister, with a delay of control
- Retract Grapple Hook
  - While Clamped
    - Player propels towards point of grapple
    - *We will test out which movement mechanics feel better, only auto-retracting while clamped or manually starting/stopping retracting while clamped*
  - While NOT Clamped (Auto-retract)
    - Happens when the player clamps on a lighter entity (e.g. medical canister), or misses
    - **Player is given some weak propulsion force (towards the grapple point/direction of missed grapple) as it retracts**
- Release Grapple Hook (while clamped to an asteroid)
  - Grapple hook de-clamps and auto-retracts with a delay of control
- Slash With Space Katana
  - Hit an Alien
    - Kill: Explode/sliced into pieces
  - **Hit an Asteroid**
    - **Breaks Apart: Asteroid slices in half/several pieces and fragments fly out of view**
    - **Slows Player Down: Player loses some momentum (normal force backwards)**
  - **Hit an Alien Laser Bolt**
    - **Parry: bullet flies off**
- Propel with Pneumatic Thruster
  - Player propels towards the direction they’re facing (force applied)
  - Air canister capacity decreases
  - If grapple is clamped, player will swing on grapple
- Get within too close range of Black Hole
  - Die: Player is sucked into Black Hole
  - B3-M1S gives snarky quip
  - Level restarts
- Get hit by alien or an alien laser bolt
  - Take damage
  - Lose momentum (small force backwards, originating from the direction of laser bolt)
- Die from too much damage (either from asteroids or laser bolts)
  - B3-M1S gives snarky quip
  - Level restarts
- Touch medical canister/heal pod
  - Heal damage
- Move through close range of atmospheric asteroid
  - Refill pneumatic thruster canister (more fuel to propel around with)
- **Generic interact**
  - **Talk (e.g. to B3-M1S)**
  - **Start simulation**
  - **Restart a previously completed “lesson” to improve best time**
  - *Non-simulation will all be cutscenes or simple GUI if we don’t have time to implement them as scenes with interactions*
  
#### Fungos (alien enemies)
- Shoot laser bolt
  - Laser bolt propels towards the player
  - **varying size, speed, and damage**
- Move
  - Use thrusters to propel around space
- **Shoot rocket at asteroids (primarily in front of player/what they’re grappling on)**
  - **Asteroid breaks apart**
  
#### B3-M1S
- Performs dialogue
  - Instructions
  - Feedback
  - Story-related
  
#### Black Hole
- Expands towards player as they progress through level
- **Exerts a constant gravitational force on player and asteroids (stronger when closer)**

#### Asteroids
- Vary in size, speed, type, frequency, **shape**
- **Crash into each other, creating smaller asteroids**
- Different Types (Decreasing in importance)
  - Normal
    - Can be grappled to
    - Take damage on contact
  - Atmospheric Asteroid
    - Allows player to refill pneumatic thruster
    - Can be grappled to
    - Take (less) damage on contact
  - **Dense Asteroid**
    - **Exert small gravitational force on player towards the asteroid**
    - **Can be grappled to**
    - **Take damage on contact**
  - **Sandy Asteroid**
    - **Can be grappled to**
    - **Take no damage on contact**
  - **Water Asteroid**
    - **Can mostly be grappled to**
    - **Can’t grapple to pools of water**
    - **Take damage on contact**
  - **Volcanic Asteroids**
    - **Volcanoes erupt → lava damages space suit significantly**
    - **Can be grappled to**
    - **Take damage on contact**

## Game Rules
#### Bringing everything together, what are some game rules for winning, losing, beating levels, health, and damage?

To win, the player must get from the starting point of the level, through the asteroid field, to the exit portal. They must grapple into the portal, at which point the player loses control and the grapple hook retracts, pulling the player into the portal and ending the level. This means the player has passed this lesson in the agility course. To beat the game, the player must complete all lessons (followed by the non-simulated finale level after the Fungos destroy the S.N.A.K.E. facility).

When the player takes too much damage to their space suit, they lose. This damage can occur from hitting asteroids and/or getting shot by aliens. The player also loses if they don’t progress fast enough and get too close to the black hole, and it sucks them in. If the player runs out of capacity on their pneumatic thruster and is too far away from any asteroid to grapple, this is most likely a losing scenario, so they will have to restart (which will either happen manually or automatically depending on time).

## Target Audience
#### Who is this game for?
This game is for coordinated gamers who like a good adrenaline rush and enjoy games with fast, open-ended, dynamic game play. Fans of parkour mechanics like those found in Mirror’s Edge should find this game entertaining. The ability to beat personal best times on a level will attract people who like to improve upon their scores, especially if we add an endless mode. Sci-fi fans will also be intrigued.
Artwork
What artwork do you need? Have you searched online? Are you planning to use any of the assets provided? 

Our art style will probably be low-poly. If we can figure out how to do it in time, we would love a cell shaded art style. We will use a mix of Sci-Fi asset packs (provided and self-found) and self-developed 3D models/art. Our cutscenes will be still images, unless we have a lot of extra time.

