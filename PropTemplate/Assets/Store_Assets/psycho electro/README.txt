PSYCHO ELECTRO MUSIC ASSET (FREE): SETUP GUIDE

Dear Unity Developer,
Thank you for downloading this package and supporting my work!

Here are some indications as to how to use this pack.

********************************************************************************************************************************************************************************************************
GENERAL REMARKS:

- DO NOT, under any circumstance, change the folder structure of the "Resources" folder.  The resources folder HAS to be located in your asset folder and the folder structure HAS to remain untouched.
  The reason is that many scrips rely on this folder structure to load the samples into arrays. Many scripts could stop working if you change the folder structure.

- Be sure to add an audio listener to your camera or the game object you attach the script on.  Apparently, this helps with the overall performance of the script.

*********************************************************************************************************************************************************************************************************

How to use the Psycho_master script:
-------------------------------------------------

The Psycho_master script is inside the "Scripts" folder.
All you need to do is to drag and drop the script on a game object with an audio listener component.  
Drag/drop the "psycho_mixer" mixer into the corresponding box in the "Psycho_master" script.

Upon attaching the script to a game object, you will see several checkboxes which are public booleans that activate or deactivate the playback of the different samples.

So in your game, whenever you want to activate one of the sample loops (linked to a specific event, trigger...) 
simply access the booleans
 listed inside the script and set one of them to "true".

To help you in writing the script, there is an example of how to use it in the "code_snippet_script".

Upon setting the "reset" boolean to "true", the samples will finish playing and will be assigned new ones randomly.
Be sure to set "reset" to "false" once you've used it, otherwise the script will not run.

Not all samples use fade in/out.  The Terror percussion loop starts and stops playing at the beginning of a measure so that it is always in tempo.
It also stops after the loop is finished.  

The "hits" boolean plays a "hit" sample which can be used for dramatic effect.  It will be played at the start of each measure and only once.
If you want to play it several times, make sure you set it to "true" several times as it resets itself automatically.

Example of code that you need to include inside the script that has your triggers/events:
-----------------------------------------------------------------------------------------



//creates a public variable - drag and drop the game object to which the "Psycho_master" script 
is attached into the cell created in your script.


public Psycho_master psycho_script;

public bool no_enemies;


void Update(){
	
	if (no_enemies) {
		psycho_script.ambiant_long_bool = true;

		psycho_script.tension_long_bool = false;
		psycho_script.tension_short_bool = false;
		psycho_script.tension_percs_bool = false;

		psycho_script.terror_long_bool = false;
		psycho_script.terror_bass_bool = false;
		psycho_script.terror_percs_bool = false;
		psycho_script.terror_intense_bool = false;
		psycho_script.terror_background_bool = false;
	}

}
// sets the booleans inside the "Psycho_master" script to play the "ambiant" samples if there are no enemies present.


_____________________________________________________________________________________________________


The "Psycho_basic" script is essentially a stripped down version of the "Psycho_master" script.
It allows you to get full control over the mixer and transitions if you want to handle them yourself.
Activate the "start" boolean when you want the sound to start (don't forget to pull down all the mixer tracks or everything will start playing at once!).
Activate the "reset" boolean if you want to stop everything and reassign a fresh combination of samples randomly (make sure all mixer tracks are 
pulled down or the sound will end abruptly).
Activate the "hit" boolean if you want to play a "hit" sample at the beginning of a measure.

-------------------------------------------------------------------------------------------------------------------------
IMPORTANT THINGS TO REMEMBER:

- Make sure only ONE boolean is set to "true" at all times, otherwise the script may not function properly!
- The "Psycho_demo" script is used in the "Psycho Demo" scene in the "Scenes" folder.  You can play with it by running the scene, but it is not usable directly for your game.

-------------------------------------------------------------------------------------------------------------------------

I hope you'll be able to make use of this!
Don't forget to leave a review and suggestions/ideas for how to make this work even better!

Thanks again for your support and don't hesitate to contact me if you have any questions/suggestions! 

sincerely,

Marma

CONTACT: marma.developer@gmail.com
WEBSITE: http://marmadeveloper.wix.com/marmamusic