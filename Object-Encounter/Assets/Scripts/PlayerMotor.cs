///////////////////////////////////////////////////////////////////////////////
// File:             PlayerMotor.cs
// Date:			 November 20 2016
//
// Author:           Sizhuo Ma sizhuoma@cs.wisc.edu
///////////////////////////////////////////////////////////////////////////////
using UnityEngine;
using UnityEngine.Networking;

/// <summary>
/// Handles player movement, when given player input from 
/// myPlayerController/myViveController
/// Based on a tutorial by Game To Game Developer
/// </summary>
[RequireComponent(typeof(Rigidbody))]
public class PlayerMotor : NetworkBehaviour {

	[SerializeField]
	private Transform playerPosition;

	// References for both cameras, workaround for
	// only having one spawnable player prefab
	[SerializeField]
	private Camera firstPersonCam;
	[SerializeField]
	private Camera thirdPersonCam;

	// Final camera variable, assigned to one of the above
	private Camera cam;

	// States for the player
    private Vector3 velocity = Vector3.zero;
    private Vector3 rotation = Vector3.zero;
    private Vector3 cameraRotation = Vector3.zero;
    private Rigidbody rb;

	// Initialization
    void Start ()
    {
		// Choose camera based on the role of the player
		if (isServer) {
			cam = firstPersonCam;
			cam.transform.position += new Vector3 (0f, 0.5f, 0f);
		}
		else {
			cam = thirdPersonCam;
		}
        rb = GetComponent<Rigidbody>();
    }

    // Gets a movement vector
    public void Move (Vector3 _velocity)
    {
        velocity = _velocity;
    }


    // Gets a rotational vector
    public void Rotate(Vector3 _rotation)
    {
        rotation = _rotation;
    }

    // Gets a rotational vector for the camera
    public void RotateCamera(Vector3 _cameraRotation)
    {
        cameraRotation = _cameraRotation;
    }

    public void Jump()
    {
        rb.AddForce(Vector3.up * 600);
    }

    // Run every physics iteration
    void FixedUpdate ()
    {
		if (playerPosition.position.y <= -50f) {
			playerPosition.position = new Vector3(playerPosition.position.x, 30f, playerPosition.position.z);
		}
			
        PerformMovement();
        PerformRotation();
    }

    // Perform movement based on velocity variable
    void PerformMovement()
    {
        if (velocity != Vector3.zero)
        {
            rb.MovePosition(rb.position + velocity * Time.fixedDeltaTime);
        }
    }

    void PerformRotation()
    {
		rb.MoveRotation(rb.rotation * Quaternion.Euler(rotation));
       
        if (cam != null)
        {
			if (isServer) {
				Vector3 eulerAngles = cam.transform.localEulerAngles;
				eulerAngles.x -= cameraRotation.x;
				if (eulerAngles.x > 90 && eulerAngles.x < 180)
					eulerAngles.x = 90;
				else if (eulerAngles.x >= 180 && eulerAngles.x < 270)
					eulerAngles.x = 270;
				cam.transform.localEulerAngles = eulerAngles;
			}

            // Third person camera movement
			else {
				Vector3 xAxis = -playerPosition.right;
				cam.transform.RotateAround (playerPosition.position, xAxis, cameraRotation.x);
			}
            
        }
    }
}
