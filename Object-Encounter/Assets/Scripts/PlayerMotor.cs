using UnityEngine;
using UnityEngine.Networking;

[RequireComponent(typeof(Rigidbody))]
public class PlayerMotor : NetworkBehaviour {

	[SerializeField]
	private Transform playerPosition;
	[SerializeField]
	private Camera firstPersonCam;
	[SerializeField]
	private Camera thirdPersonCam;
	private Camera cam;
    private Vector3 velocity = Vector3.zero;
    private Vector3 rotation = Vector3.zero;
    private Vector3 cameraRotation = Vector3.zero;
    private Rigidbody rb;

    void Start ()
    {
		if (isServer) {
			cam = firstPersonCam;
			cam.transform.position += new Vector3 (0f, 0.5f, 0f);
		}
		else {
			cam = thirdPersonCam;
			//cam.transform.localPosition = new Vector3 (0f, 2f, -4f);
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
		//if (isServer) {
			rb.MoveRotation(rb.rotation * Quaternion.Euler(rotation));
		//}
       
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
            //cam.transform.Rotate(-cameraRotation);
            // clamp the rotation
            // Third person camera movement
			else {
				
				// ALL of this is random
				//eulerAngles.y -= cameraRotation.y;
				//cam.transform.localEulerAngles = eulerAngles;
				//Vector3 xAxis = Vector3.Cross(playerPosition.up, Vector3.up);
				Vector3 xAxis = -playerPosition.right;
				//Vector3 yAxis = playerPosition.up;
				cam.transform.RotateAround (playerPosition.position, xAxis, cameraRotation.x);
				//cam.transform.RotateAround (playerPosition.position, yAxis, cameraRotation.y);
				//Vector3 newCamPos = rb.position;
				//newCamPos -= new Vector3 (0f, 2f, -4f);
				//cam.transform.position = newCamPos;
			}
            
        }
    }
}
