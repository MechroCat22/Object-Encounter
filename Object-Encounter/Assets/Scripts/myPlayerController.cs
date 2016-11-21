///////////////////////////////////////////////////////////////////////////////
// File:             myPlayerController.cs
// Date:			 November 20 2016
//
// Author:           Andrew Chase chase3@wisc.edu
///////////////////////////////////////////////////////////////////////////////
using UnityEngine;
using UnityEngine.Networking;

/// <summary>
/// Handles user input for moving the player around the scene
/// Based on a tutorial series from Game To Game Developer
/// </summary>
[RequireComponent(typeof(PlayerMotor))]
public class myPlayerController : NetworkBehaviour {

    [SerializeField]
    private float speed = 8f;
    [SerializeField]
    private float lookSensitivity = 3f;
    private float sprintMultiplier = 0.5f;
    private bool isFalling = false;
    private float finalSpeed = 10f;
	[SerializeField]
	private Camera thirdPersonCam;
    private AudioSource playerAudio;

    //Player sounds - LOCAL ONLY
    public AudioClip jumpSound;
    public AudioClip footSteps;

	// Script to more player around
    private PlayerMotor motor;

	// Initialization
    void Start ()
    {
        motor = GetComponent<PlayerMotor>();
        playerAudio = GetComponent<AudioSource>();
    }

    void Update ()
    {
        //Calculate movement velocity  as a 3D vector
        float _xMov = Input.GetAxisRaw("Horizontal");
        float _zMov = Input.GetAxisRaw("Vertical");

        Vector3 _movHorizontal;
        Vector3 _movVertical;

		// If player is the host, we are in first person view, so move
		// relative to their own transform
		if (isServer) {
			_movHorizontal = transform.right * _xMov;
			_movVertical = transform.forward * _zMov;
		}
		// If the player is a hider, move relative to the third person
		// camera
		else {
			_movHorizontal = thirdPersonCam.transform.right * _xMov;
			_movVertical = thirdPersonCam.transform.forward * _zMov;
		}

		// Left shift used for sprinting
        if (Input.GetKey(KeyCode.LeftShift))
        {
            finalSpeed = speed * sprintMultiplier;
        }
        else
        {
            finalSpeed = speed;
        }

        // Final movement vector
        Vector3 _velocity = (_movHorizontal + _movVertical).normalized * finalSpeed;

        //if the player is jumping
        if (Input.GetButtonDown("Jump") && !isFalling)
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

	// For jumping, to prevent infinite jump
	// BUG: Player can still jump 2-3 times in midair if spacebar is spammed
    void OnCollisionStay()
    {
        isFalling = false;
    }


}
