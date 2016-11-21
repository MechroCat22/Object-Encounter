///////////////////////////////////////////////////////////////////////////////
// File:             HunterController.cs
// Date:			 November 20 2016
//
// Authors:           Andrew Chase chase3@wisc.edu
//                    Sizhuo Ma sizhuoma@cs.wisc.edu
///////////////////////////////////////////////////////////////////////////////
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using System.Collections;

/// <summary>
/// Controls hunter-based gameplay, including shooting hiding players
/// and opening doors
/// </summary>
public class HunterController : NetworkBehaviour {

	// Steam controller objects
	private SteamVR_TrackedObject rightTrackedObject;
	private SteamVR_Controller.Device rightController;

	// GUI textfields
	// Different fields are used based on which player camera is used
	private Text UIText;
	public Text firstPersonText;
	public Text thirdPersonText;

	// Hunter tweaks
    public float FireRate = 0.03f;
    public int Damage = 100;
    public float ShootDistance = 500f;
    public float InteractDistance = 150f;
    public int WaitTime = 15;

	// Door sound
    public AudioClip doorSound;

	// Timer used to control when the hunter can fire again
    private float timeCounter = 0;

	// Determining if the hunter is active in the scene
    [SyncVar]
    private bool isActive;

	// Camera reference
    private Camera myCamera;
    private DoorController doorController;
    private Timer timer;

	// Choosing the spawn direction and position when player enters the game
    private Vector3 spawnPosition;
    private Quaternion spawnRotation;
    private Rigidbody rigidBody;
	// Hunter waits for 15 seconds at the beginning of the game
    private bool waiting;



    // Use this for initialization
    void Start() {

		// If the player is the host and the object in the scene is the localplayer's character, initialize the Steam controller
		if (isServer && isLocalPlayer) {
			rightTrackedObject = this.transform.Find("[CameraRig]").Find("Controller (right)").GetComponent<SteamVR_TrackedObject>();
		}

		// Determining the camera based on if the person is a seeker or hider
		string whichCamera;
		if (isServer && isLocalPlayer) {
			whichCamera = "FirstPerson";
			UIText = firstPersonText;
		} else {
			whichCamera = "ThirdPerson";
			UIText = thirdPersonText;
		}
		myCamera = transform.Find(whichCamera).GetComponent<Camera>();
        doorController = GetComponent<DoorController>();
        timer = GameObject.Find("Timer").GetComponent<Timer>();

        // record the spawn place, used to respawn
        spawnPosition = transform.position;
        spawnRotation = transform.rotation;
        rigidBody = GetComponent<Rigidbody>();

        // TEMPORARY solution: the server is the hunter
        if (isServer) {
            if (isLocalPlayer) {
                isActive = true;
				this.transform.Find ("[CameraRig]").gameObject.SetActive (true);
			}
            else {
                isActive = false;
            }
        }

        // wait at the beginning of the game
        if (isActive) {
            waiting = true;
            GetComponent<myViveController>().enabled = false;
        }
			
    }

    // Update is called once per frame
    void Update() {
        // return if I am not a hunter player
        if (!isActive || !isLocalPlayer)
            return;

		// Flags for determining controller state per frame
		bool leftActive = true;
		bool leftTriggerPulled = false;
		bool rightTriggerPulled = false;
		bool leftGripPressed = false;
		bool rightGripPressed = false;

		// Setup Vive Controller
		try {
			rightController = SteamVR_Controller.Input((int)rightTrackedObject.index);
			rightTriggerPulled = rightController.GetPressDown(Valve.VR.EVRButtonId.k_EButton_SteamVR_Trigger);
			rightGripPressed = rightController.GetPressDown(Valve.VR.EVRButtonId.k_EButton_Grip);
		} catch (System.Exception) {
			Debug.Log ("No controllers Connected");
		}
			
		bool triggersPulled = (leftTriggerPulled || rightTriggerPulled);
		bool gripsPressed = (leftGripPressed || rightGripPressed);
        // waiting for some time at the beginning of the game
        // such that the hiders have enough time to hide
        if (waiting) {
            int secondsElapsed = (int)Time.timeSinceLevelLoad;
            if (secondsElapsed < 15) {
                UIText.text = "Wait for " + (15 - secondsElapsed) + " second(s)";
                return;
            }
            else {
                waiting = false;
                GetComponent<myViveController>().enabled = true;
            }
        }

        // return if game is over
        if (timer.GameOver()) {
            GetComponent<myPlayerController>().enabled = false;
            UIText.text = "Game Over!";
			GetComponent<PointCounter> ().stopCounting ();
            return;
        }

        // time counter: can only shoot when a certain amount of time (FireRate) has passed
        timeCounter += Time.deltaTime;

		// Raycasting - used for shooting and opening doors
		if (isServer) {
			//Ray leftControllerRay = new Ray (leftTrackedObject.transform.position, leftTrackedObject.transform.forward);
			Ray rightControllerRay = new Ray (rightTrackedObject.transform.position, rightTrackedObject.transform.forward);
			RaycastHit objectHit;
			GameObject obj = null;
			if (Physics.Raycast(rightControllerRay, out objectHit, ShootDistance)) {
				obj = objectHit.transform.gameObject;
				// if aiming at a player
				if (obj.tag.Equals("Player")) {

				}
				else if (obj.tag.Equals("Door") && objectHit.distance < InteractDistance) {
					UIText.text = "Press the trigger button to open/close the door"; 

					// VIVE REIMPLEMENT
					if (triggersPulled) {
						GetComponent<AudioSource>().PlayOneShot(doorSound, 2f);
						doorController.CmdMoveDoor(obj);
					}
				}
				else {
					UIText.text = "";
				}
			}
			else {
				UIText.text = "";
			}

			// VIVE REIMPLEMENT
			if (triggersPulled && timeCounter > FireRate) {
				// reset time counter
				timeCounter = 0.0f;

				// play the gun sound effect
				GetComponent<AudioSource>().Play();
				// Haptic feedback!
				rightController.TriggerHapticPulse (1200);

				if (obj != null && obj.tag.Equals("Player")) {
					GameObject playerHit = obj;
					playerHit.GetComponent<PropController>().TakeDamage(Damage);
					this.gameObject.GetComponent<PointCounter> ().addKillPoints ();

					Debug.Log("Hit: " + playerHit);
				}
			}
		} else {
			Ray camRay = myCamera.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2));
			RaycastHit objectHit;
			GameObject obj = null;
			if (Physics.Raycast(camRay, out objectHit, ShootDistance)) {
				obj = objectHit.transform.gameObject;
				// if aiming at a player
				if (obj.tag.Equals("Player")) {

				}
				else if (obj.tag.Equals("Door") && objectHit.distance < InteractDistance) {
					UIText.text = "Press the Grip button to open/close the door"; 

					// VIVE REIMPLEMENT
					if (triggersPulled) {
						GetComponent<AudioSource>().PlayOneShot(doorSound, 2f);
						doorController.CmdMoveDoor(obj);
					}
				}
				else {
					UIText.text = "";
				}
			}
			else {
				UIText.text = "";
			}

			// VIVE REIMPLEMENT
			if (triggersPulled && timeCounter > FireRate) {
				// reset time counter
				timeCounter = 0.0f;

				rightController.TriggerHapticPulse(700);
				// play the gun sound effect
				GetComponent<AudioSource>().Play();

				if (obj != null && obj.tag.Equals("Player")) {
					GameObject playerHit = obj;
					playerHit.GetComponent<PropController>().TakeDamage(Damage);
					Debug.Log("Hit: " + playerHit);
				}
			}
		}
        
    }
}
