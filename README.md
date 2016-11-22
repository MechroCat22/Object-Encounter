# Object Encounter
A virtual reality hide-and-seek game

Authors: Andrew Chase, Sizhuo Ma, Grayson Freking

Date:    December 15th, 2015

Send questions or comments to chase3@wisc.edu

The basic environment was made using Unity's primitive GameObjects, with
free/purchased assets from the Unity Asset store.

--------------------------------------------------------------------------------------------------------

- Description

This game is a Unity project for a hide-and-seek game based on Garry's Mod's PropHunt. 
The game follows a Host/Client structure, where the host is the seeker and the clients
are hiders. The hiders can change into objects within the environment, and the seeker must
find and eliminate them! The hiders have unlimited respawns, and the game lasts for five
minutes.

Both teams score points differently. The seeker constantly gains points, and receives an additional
1000 points for catching a hiding player. The hiders earn points based on how close they stay to the
seeker, with closer rewarding more points. Don't stray too far though, as if you stray too far you'll
eventually be losing points!


- Functionality/Usage

Object Encounter is built using Unity 5's NetworkBehaviour API, with the host of the game
acting as the seeker. There is only one seeker per game, and the game immedately starts as
soon as the seeker starts. All other players connect to the host's IP address as clients, 
and are given control immediately upon entering.

The builds are located in Object-Encounter/Builds. There are two builds, one for monitor/Oculus Rift
gameplay, and one for Vive gameplay for the seeker only (clients still use monitor or Oculus). The 
Oculus Rift version is an older version of the game, and doesn't have the point system. The Vive build
has the point system, and invisible boundaries to prevent players from straying too far apart (this
build was presented at the Wisconsin Science Festival).

To start the game, have the host open up the desired build (Oculus build for just monitor/
Oculus Rift play, Vive build if they are using the Vive). The host can simply press "Host LAN Game"
to start the session. I left the default Network Manager HUD showing in the Vive build because
it is difficult to click the main menu buttons when the game camera is tracked to the headset
(Since I forgot to include this HUD in the Oculus build, starting the game can be difficult due
to not being able to click the button while using an Oculus Rift, though it still works). Thus
in the Vive build, you can use the "LAN Host" button on the default HUD as well.

To have clients connect, have them open up the same build the host is using. Here in either build, 
you can enter the host's local IP address in the text field and click LAN Client/Join LAN Game, and they
should connect immediately and begin playing.

The game has never been tested with matchmaking, and I'm fairly certain it wouldn't work.

The players on screen/Oculus Rift can use the crosshairs to aim their view, and a prompt will appear on
the screen when they can interact with something (except for hunter shooting, just shoot!). In the Vive,
the hunter will have a laser guide that originates from the Vive controller and points in the direction
the controller is facing. This is used to help the hunter aim at players and doors.

The game ends after 5 minutes, where the screen will fade to black. In the Vive build, you can use the
default Network Manager HUD to start a new session (end the current session and start again), or for both
builds you can just restart the .exe's and have everyone try again.

- Controls

For the Oculus build:

    Hunter: W -          move forward                   Hiders: W -          move forward
            A -          strafe left                            A -          strafe left
            S -          move backwards                         S -          move backwards
            D -          strafe right                           D -          strafe right
            spacebar -   jump                                   spacebar -   jump
            Left-click - open doors, fire gun                   Left-click - open doors, turn into objects

            
For the Vive build:

    Hunter: Touchpad UP    - move forward                   Hiders: W -          move forward
            Touchpad LEFT  - strafe left                            A -          strafe left
            Touchpad DOWN  - move backwards                         S -          move backwards
            Touchpad RIGHT - strafe right                           D -          strafe right
            Grip Button    - jump                                   spacebar -   jump
            Trigger        - open doors, fire gun                   Left-click - open doors, turn into objects


- Gameplay Comments

The colliders on the stairs don't seem to work correctly, and often the player falls through them into the
basement. Players found that constantly jumping up the stairs works best.

The Hunter will most certainly always have the most points, and if a client ends the game with a positive score,
congratulations! Most hiders end up with a negative score, showing that the point system is completely unbalanced.

Spawn camping is definitely an issue, though only small children seemed to take advantage of it.

The invisible boundaries around the house in the Vive build were absolutely necessary to keep players from running
to the corner of the map and hiding/waiting until the timer ran out.


- BUGS

Currently in Unity there is an error pertaining to the MeshCollider of a certain object (fridge).
The error doesn't cause any issues in the editor or building in any way. There are also a few
warnings due to deprecations in Unity, which don't cause any issues for the moment.

Players can perform multiple jumps mid-air if the jump button is spammed.

The myViveController script throw runtime errors/controller never reconnects if it loses
tracking while playing. The only solution currently is to restart the game with the controller tracked.
Two controllers can be tracked while playing, but only the right controller input will be received.

Sometimes a Hunter will aim and shoot at a player, but the player won't die. Similarly, sometimes the
laser light mysteriously stops and hits "an invisible wall." I believe this may have to do with the hunter's 
collider interfering with the raycast.

Clients randomly disconnect at times, though I'm unsure if this is due to the game itself or the network setup.

I accidentally left a player prefab object in the scene in the Oculus Build. Since that build was an older build, and
because I'm not sure which commit to revert to that would be associated with this version, I just decided t leave it there.
It doesn't cause any errors, nor does it interfere with gameplay outside of being an environment artifact.
