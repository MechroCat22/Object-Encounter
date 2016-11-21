///////////////////////////////////////////////////////////////////////////////
// File:             myViveController.cs
// Date:			 November 20 2016
//
// Author:           Andrew Chase chase3@wisc.edu
///////////////////////////////////////////////////////////////////////////////
using UnityEngine;
using UnityEngine.Networking;

/// <summary>
/// Reworked version of the hunter controller to use the Vive Controller
/// </summary>
[RequireComponent(typeof(PlayerMotor))]
public class myViveController : NetworkBehaviour {

	// Steam controllers
	// TODO: Only one controller is used, so getting input from the left controller
	// is unnecessary
	private SteamVR_TrackedObject leftTrackedObject;
	private SteamVR_Controller.Device leftController;

	private SteamVR_TrackedObject rightTrackedObject;
	private SteamVR_Controller.Device rightController;

	// Movement variables
	[SerializeField]
	private float speed = 8f;
	[SerializeField]
	private float lookSensitivity = 3f;
	private float sprintMultiplier = 2f;
	private bool isFalling = false;
	private float finalSpeed = 5f;
	[SerializeField]
	private Camera firstPersonCam;
	private AudioSource playerAudio;

	//Player sounds - LOCAL ONLY
	public AudioClip jumpSound;
	public AudioClip footSteps;

	// Script to move the player
	private PlayerMotor motor;

	// Initialization
	void Start ()
	{
		// For safety
		if (!isServer || !isLocalPlayer)
			return;
		
		motor = GetComponent<PlayerMotor>();
		playerAudio = GetComponent<AudioSource>();

		// Setting up the Vive tracked objects - for getting index of controller
		leftTrackedObject = this.transform.Find("[CameraRig]").Find("Controller (left)").GetComponent<SteamVR_TrackedObject>();
		rightTrackedObject = this.transform.Find("[CameraRig]").Find("Controller (right)").GetComponent<SteamVR_TrackedObject>();
	}
		
	void Update ()
	{
		if (!isServer || !isLocalPlayer)
			return;
		float xLeftInput = 0;
		float zLeftInput = 0;
		float xRightInput = 0;
		float zRightInput = 0;
		bool leftActive = true;
		// Setting up the Vive controllers
		bool leftGripPressed = false;
		try {
			leftController = SteamVR_Controller.Input((int)leftTrackedObject.index);
			leftGripPressed = leftController.GetPressDown(Valve.VR.EVRButtonId.k_EButton_Grip);
			if (leftController.GetPress(Valve.VR.EVRButtonId.k_EButton_SteamVR_Touchpad)) {
				// Get the left inputs
				xLeftInput = leftController.GetAxis().x;
				zLeftInput = leftController.GetAxis ().y;
			}
	
		} catch (System.Exception) {
			//Just roll with it
			leftActive = false;
		}
		bool rightGripPressed = false;
		try {
			rightController = SteamVR_Controller.Input((int)rightTrackedObject.index);
			rightGripPressed = rightController.GetPressDown(Valve.VR.EVRButtonId.k_EButton_Grip);
			if (rightController.GetPress(Valve.VR.EVRButtonId.k_EButton_SteamVR_Touchpad)) {
				// Get the left inputs
				xRightInput = rightController.GetAxis().x;
				zRightInput = rightController.GetAxis ().y;
			}
		} catch (System.Exception) {
			if (!leftActive) {
				Debug.Log ("No controllers Connected");
			}
		}

		//Calculate movement velocity  as a 3D vector
		// Strafe
		float _xMov = xLeftInput + xRightInput;
		// Forward
		float _zMov = zLeftInput + zRightInput;


		Vector3 _movHorizontal;
		Vector3 _movVertical;

		// Move relative to the first person camera
		if (isServer) {
			_movHorizontal = firstPersonCam.transform.right * _xMov;
			_movVertical = firstPersonCam.transform.forward * _zMov;
		}

		finalSpeed = speed;


		// Final movement vector
		Vector3 _velocity = (_movHorizontal + _movVertical).normalized * finalSpeed;
		bool gripsPressed = (leftGripPressed || rightGripPressed);
		//if the player is jumping
		if (gripsPressed && !isFalling)
		{
			isFalling = true;
			playerAudio.PlayOneShot(jumpSound, 0.6f);
			motor.Jump();
		}

		// Apply movement
		motor.Move(_velocity);


		// Calculate rotation as a 3D vector (turning around)
		float _yRot = Input.GetAxisRaw("Mouse X");

		Vector3 _rotation = new Vector3(0f, _yRot, 0f) * lookSensitivity;

		// Apply rotation
		motor.Rotate(_rotation);

		// Calculate camera rotation as a 3D vector (turning around)
		float _xRot = Input.GetAxisRaw("Mouse Y");


		// REVERT: Change yRot to 0f
		Vector3 _cameraRotation = new Vector3(_xRot, _yRot, 0f) * lookSensitivity;

		// Apply rotation
		motor.RotateCamera(_cameraRotation);

	}

	void OnCollisionStay()
	{
		isFalling = false;
	}


}
