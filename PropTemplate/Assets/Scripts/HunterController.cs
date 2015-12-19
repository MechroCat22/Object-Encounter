using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using System.Collections;
//using UnityStandardAssets.CrossPlatformInput;

public class HunterController : NetworkBehaviour {


    public Text UIText;
    public float FireRate = 0.3f;
    public int Damage = 34;
    public float ShootDistance = 100f;
    public float InteractDistance = 10f;
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
        myCamera = transform.Find("Camera").GetComponent<Camera>();
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
            GetComponent<PlayerController>().enabled = false;
        }
    }

    // Update is called once per frame
    void Update() {
        // return if I am not a hunter player
        if (!isActive || !isLocalPlayer)
            return;

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
                GetComponent<PlayerController>().enabled = true;
            }
        }

        // return if game is over
        if (timer.GameOver()) {
            GetComponent<PlayerController>().enabled = false;
            UIText.text = "Game Over!";
            return;
        }

        // time counter: can only shoot when a certain amount of time (FireRate) has passed
        timeCounter += Time.deltaTime;

        // the aim is locked at the center of the screen
        Ray camRay = myCamera.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2));
        RaycastHit objectHit;
        GameObject obj = null;
        if (Physics.Raycast(camRay, out objectHit, ShootDistance)) {
            obj = objectHit.transform.gameObject;
            // if aiming at a player
            if (obj.tag.Equals("Player")) {
                
            }
            else if (obj.tag.Equals("Door") && objectHit.distance < InteractDistance) {
                UIText.text = "Press \"Fire2\" to open/close the door"; 
                if (Input.GetButtonDown("Fire2")) {
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

        if (Input.GetButtonDown("Fire1") && timeCounter > FireRate) {
            // reset time counter
            timeCounter = 0.0f;

            // play the muzzle flash effect
            psys.Play();
           
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
