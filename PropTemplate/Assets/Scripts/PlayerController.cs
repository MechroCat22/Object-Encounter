using UnityEngine;

[RequireComponent(typeof(PlayerMotor))]
public class PlayerController : MonoBehaviour {

    [SerializeField]
    private float speed = 5f;
    [SerializeField]
    private float lookSensitivity = 3f;
    private float sprintMultiplier = 2f;
    private bool isFalling = false;
    private float finalSpeed = 5f;
    
    private AudioSource playerAudio;

    //Player sounds - LOCAL ONLY
    public AudioClip jumpSound;
    public AudioClip footSteps;

    private PlayerMotor motor;

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

        Vector3 _movHorizontal = transform.right * _xMov;
        Vector3 _movVertical = transform.forward * _zMov;

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

        Vector3 _cameraRotation = new Vector3(_xRot, 0f, 0f) * lookSensitivity;

        // Apply rotation
        motor.RotateCamera(_cameraRotation);

    }

    void OnCollisionStay()
    {
        isFalling = false;
    }


}
