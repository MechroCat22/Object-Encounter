using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using System.Collections;
//using UnityStandardAssets.CrossPlatformInput;

public class HunterController : NetworkBehaviour {

	//[SerializeField]
	//private SteamVR_TrackedObject leftTrackedObject;
	//private SteamVR_Controller.Device leftController;
	//[SerializeField]
	private SteamVR_TrackedObject rightTrackedObject;
	private SteamVR_Controller.Device rightController;
	private Text UIText;
	public Text firstPersonText;
	public Text thirdPersonText;
    public float FireRate = 0.03f;
    public int Damage = 100;
    public float ShootDistance = 500f;
    public float InteractDistance = 150f;
    public int WaitTime = 15;
    public AudioClip doorSound;

    private float timeCounter = 0;

    [SyncVar]
    private bool isActive;

    private Camera myCamera;
    private ParticleSystem psys;
    private DoorController doorController;
    private Timer timer;

    private Vector3 spawnPosition;
    private Quaternion spawnRotation;
    private Rigidbody rigidBody;

    private bool waiting;



    // Use this for initialization
    void Start() {

		if (isServer && isLocalPlayer) {
			//leftTrackedObject = this.transform.Find("[CameraRig]").Find("Controller (left)").GetComponent<SteamVR_TrackedObject>();
			rightTrackedObject = this.transform.Find("[CameraRig]").Find("Controller (right)").GetComponent<SteamVR_TrackedObject>();
		}
		string whichCamera;
		if (isServer && isLocalPlayer) {
			whichCamera = "FirstPerson";
			UIText = firstPersonText;
			//this.tag = "Hunter";
			//UIText = gameObject.transform.Find ("ThirdPerson").Find ("Canvas").Find ("MessageText").GetComponent<Text> ();
		} else {
			whichCamera = "ThirdPerson";
			UIText = thirdPersonText;
		}
		myCamera = transform.Find(whichCamera).GetComponent<Camera>();
        Transform gun = myCamera.transform.Find("Gun");
        Transform psh = gun.Find("Shot Effect");
        Transform ps = psh.Find("Particle System");
        psys = ps.GetComponent<ParticleSystem>();
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
        if (!isActive) {
            gun.gameObject.SetActive(false);
        }

        // wait at the beginning of the game
        if (isActive) {
            waiting = true;
            GetComponent<myViveController>().enabled = false;
        }

		// Set up Vive?
    }

    // Update is called once per frame
    void Update() {
        // return if I am not a hunter player
        if (!isActive || !isLocalPlayer)
            return;
		bool leftActive = true;
		bool leftTriggerPulled = false;
		bool rightTriggerPulled = false;
		bool leftGripPressed = false;
		bool rightGripPressed = false;
		// Setup Vive Controllers
		/*try {
			leftController = SteamVR_Controller.Input((int)leftTrackedObject.index);
			leftTriggerPulled = leftController.GetPressDown(Valve.VR.EVRButtonId.k_EButton_SteamVR_Trigger);
			leftGripPressed = leftController.GetPressDown(Valve.VR.EVRButtonId.k_EButton_Grip);
		} catch (System.Exception) {
			//Just roll with it
			leftActive = false;
		}*/

		try {
			rightController = SteamVR_Controller.Input((int)rightTrackedObject.index);
			rightTriggerPulled = rightController.GetPressDown(Valve.VR.EVRButtonId.k_EButton_SteamVR_Trigger);
			rightGripPressed = rightController.GetPressDown(Valve.VR.EVRButtonId.k_EButton_Grip);
		} catch (System.Exception) {
			//if (!leftActive) {
				Debug.Log ("No controllers Connected");
			//}
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
            return;
        }

        // time counter: can only shoot when a certain amount of time (FireRate) has passed
        timeCounter += Time.deltaTime;

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

				// play the muzzle flash effect
				//psys.Play();

				// play the gun sound effect
				GetComponent<AudioSource>().Play();


				if (obj != null && obj.tag.Equals("Player")) {
					GameObject playerHit = obj;
					playerHit.GetComponent<PropController>().TakeDamage(Damage);
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

				// play the muzzle flash effect
				//psys.Play();

				// play the gun sound effect
				GetComponent<AudioSource>().Play();


				if (obj != null && obj.tag.Equals("Player")) {
					GameObject playerHit = obj;
					playerHit.GetComponent<PropController>().TakeDamage(Damage);
					this.gameObject.GetComponent<PointCounter> ().numPoints = 5;
					Debug.Log("Hit: " + playerHit);
				}
			}
		}
        
    }
}
