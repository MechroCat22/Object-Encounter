///////////////////////////////////////////////////////////////////////////////
// File:             PropController.cs
// Date:			 November 20 2016
//
// Authors:          Andrew Chase chase3@wisc.edu
//					 Sizhuo Ma sizhuoma@cs.wisc.edu
///////////////////////////////////////////////////////////////////////////////
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using System.Collections;

/// <summary>
/// Handles prop-based functionality, including chaning into objects
/// and opening doors
/// </summary>
public class PropController : NetworkBehaviour {

	// Default Model for the player (skeleton)
    public GameObject DefaultModel;

	// Texts based on the camera used
	private Text UIText;
	public Text firstPersonText;
	public Text thirdPersonText;

	// Made the health of the player 1, since it was difficult to
	// tag players with the Vive controller multiple times
    public int MaxHealth = 1;
	// Distance player can open a door
    public float InteractDistance = 10f;
	// Layer Mask to prevent raycasts from being blocked in third person view
	public LayerMask ignorePlayer;

	// Current player model
    private GameObject playerModel;
    private GameObject graphics;
    private GameObject cam;
    private Camera myCamera;
    private Rigidbody rigidBody;

	// Open doors
    private DoorController doorController;
	// Reference to the timer
    private Timer timer;
	// Initial position and rotation when spawned
    private Vector3 spawnPosition;
    private Quaternion spawnRotation;
	// Sound effects - LOCAL ONLY
    private AudioSource playerAudio;
    public AudioClip damageSound;
    public AudioClip deathSound;
    public AudioClip respawnSound;
    public AudioClip objChangeSound;
    public AudioClip doorSound;
	private int updateCount = 0;
    private bool tookDamage = false;

	// Flag indicating player is active in the scene
    [SyncVar]
    private bool isActive;

	// Player health, updated to all clients constantly by the server
    [SyncVar, HideInInspector]
    public int health;

	// Flag for if the player runs out of health
    [SyncVar]
    private bool dead;

	// Message to pass to the server, based on how the state of the player changed
	// Change - Prop changed into a different object
	// Death - Player died
	// Respawn - Player respawned after death
	// Tag - Tag message to flag who the hunter is
    public class PropMessage : MessageBase {
        public enum Type { Change, Death, Respawn, Tag };
        public static short TypeId = 555;
		// Type of message passed
        public Type msgType;
		// Player associated with the message
        public NetworkInstanceId player;
		// If Type.Change, the prop associated with the message
        public NetworkInstanceId prop;
    }
		
    // Initialization
    void Start() {

		// If the player is the seeker, tag their player object as "Hunter" and 
		// send a message to all clients that this object is the Hunter object
		if (isServer && isLocalPlayer) {
			this.tag = "Hunter";
			SendPropMessageTag ();
		}

		// Setup camera based on player role
		string whichCamera;
		if (isServer) {
			whichCamera = "FirstPerson";
			UIText = firstPersonText;
		} else {
			whichCamera = "ThirdPerson";
			UIText = thirdPersonText;
		}

        // hide and lock the cursor
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        // Get Audio Source
        playerAudio = GetComponent<AudioSource>();

        // get necessary references
        graphics = transform.Find("Graphics").gameObject;
        playerModel = graphics.transform.Find("Player Model").gameObject;
        cam = transform.Find(whichCamera).gameObject;
        myCamera = cam.GetComponent<Camera>();
        rigidBody = GetComponent<Rigidbody>();
        doorController = GetComponent<DoorController>();
        timer = GameObject.Find("Timer").GetComponent<Timer>();

        // life status
        health = MaxHealth;
        dead = false;

        // record the spawn place, used to respawn
        spawnPosition = transform.position;
        spawnRotation = transform.rotation;

        // disable UI for other players
        if (!isLocalPlayer) {
            cam.transform.Find("Canvas").gameObject.SetActive(false);
        }

        // ignore local player model to avoid clipping with first person camera
        if (isLocalPlayer && isServer) {
            playerModel.GetComponent<MeshRenderer>().enabled = false;
        }

        // setup handlers for messages
        if (isServer) {
            NetworkServer.RegisterHandler(PropMessage.TypeId, OnPropMessageServer);
        }
        if (isLocalPlayer) {
            NetworkClient.allClients[0].RegisterHandler(PropMessage.TypeId, OnPropMessageClient);
        }

        // TEMPORARY SOLUTION: the server is the hunter permanantly
        if (isServer) {
            if (isLocalPlayer) {
                isActive = false;
            }
            else {
                isActive = true;
            }
        }
    }

	// Remove the cursor when application opens
	// Use Alt+F4 to close program
    void OnApplicationFocus() {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

	// When destoryed, give back the mouse pointer
    void OnDestroy() {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    // Update is called once per frame
    void Update() {
		if (isLocalPlayer && isServer && updateCount > 30) {
			SendPropMessageTag ();
			updateCount = 0;
		} else {
			updateCount++;
		}
        if (tookDamage)
        {
            playerAudio.PlayOneShot(damageSound, 2f);
            tookDamage = false;
        }
        // only do the following things on local player
        if (!isLocalPlayer)
            return;

        // return if I am not a prop player
        if (!isActive)
            return;

        // return if game is over
        if (timer.GameOver()) {
            GetComponent<myPlayerController>().enabled = false;
			GetComponent<PointCounter> ().stopCounting ();
            UIText.text = "Game Over!";
            return;
        }

        // if dead, no more action
        if (dead) {
            return;
        }
        else if (health <= 0) {
            DieLocal();
            SendPropMessageDie();
            return;
        }

        // the aim is locked at the center of the screen for the hiders
        Ray camRay = myCamera.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2));
        RaycastHit objectHit;
		if (Physics.Raycast(camRay, out objectHit, InteractDistance, ignorePlayer)) {
            GameObject obj = objectHit.transform.gameObject;
            // aiming at a prop
            if (obj.tag.Equals("Prop")) {
                UIText.text = "Press \"Fire1\" to change into the " + obj.name;
                // if Fire1 down, change into that object
                if (Input.GetButtonDown("Fire1")) {
                    SendPropMessageChange(obj);
                }
            }
            else if (obj.tag.Equals("Door")) { // aiming at a door
                UIText.text = "Press \"Fire2\" to open/close the door";
                // if Fire1 down, open/close that door
                if (Input.GetButtonDown("Fire2")) {
                    GetComponent<AudioSource>().PlayOneShot(doorSound, 2f);
                    doorController.CmdMoveDoor(obj);
                }
            }
            else {
                UIText.text = "Health: " + health;
            }
        }
        else {
            UIText.text = "Health: " + health;
        }
    }

    // send message when the player change to a prop
    private void SendPropMessageChange(GameObject objHit) {
        NetworkIdentity objIdtt = objHit.GetComponent<NetworkIdentity>() as NetworkIdentity;
        NetworkIdentity playerIdtt = gameObject.GetComponent<NetworkIdentity>() as NetworkIdentity;
        if (objIdtt == null || playerIdtt == null)
            return;

        PropMessage msg = new PropMessage();
        msg.msgType = PropMessage.Type.Change;
        msg.prop = objIdtt.netId;
        msg.player = playerIdtt.netId;
        NetworkClient.allClients[0].Send(PropMessage.TypeId, msg);
        Debug.Log("Client sent: " + msg.player + " " + msg.prop);
    }

	// Send message periodically to tell clients who the hunter is
	// Must send periodically in case clients join mid-game
	private void SendPropMessageTag() {
		PropMessage msg = new PropMessage ();
		NetworkIdentity playerIdtt = this.GetComponent<NetworkIdentity>() as NetworkIdentity;
		msg.msgType = PropMessage.Type.Tag;
		msg.prop = playerIdtt.netId;
		msg.player = playerIdtt.netId;
		NetworkClient.allClients [0].Send (PropMessage.TypeId, msg);
	}

    // send message when the player dies
    private void SendPropMessageDie() {
        NetworkIdentity playerIdtt = gameObject.GetComponent<NetworkIdentity>() as NetworkIdentity;
        if (playerIdtt == null)
            return;

        PropMessage msg = new PropMessage();
        msg.msgType = PropMessage.Type.Death;
        msg.player = playerIdtt.netId;
        NetworkClient.allClients[0].Send(PropMessage.TypeId, msg);
        Debug.Log("Client sent: " + msg.player);
    }

    // send message when the player is respawned
    private void SendPropMessageRespawn() {
        NetworkIdentity playerIdtt = gameObject.GetComponent<NetworkIdentity>() as NetworkIdentity;
        if (playerIdtt == null)
            return;

        PropMessage msg = new PropMessage();
        msg.msgType = PropMessage.Type.Respawn;
        msg.player = playerIdtt.netId;
        NetworkClient.allClients[0].Send(PropMessage.TypeId, msg);
        Debug.Log("Client sent: " + msg.player);
    }

    // server message handler
    private void OnPropMessageServer(NetworkMessage netMsg) {
        PropMessage msg = netMsg.ReadMessage<PropMessage>();
        NetworkServer.SendToAll(PropMessage.TypeId, msg);
        Debug.Log("Server received: " + msg.player + " " + msg.prop);
    }

    // client message handlers
    private void OnPropMessageClient(NetworkMessage netMsg) {
        PropMessage msg = netMsg.ReadMessage<PropMessage>();
        Debug.Log("Client received: " + msg.player + " " + msg.prop);

        switch (msg.msgType) {
            case PropMessage.Type.Change: {
                    GameObject playerChanged = ClientScene.FindLocalObject(msg.player);
                    PropController pc = playerChanged.GetComponent<PropController>();
                    //GameObject prop = ClientScene.prefabs[msg.prop];
                    GameObject obj = ClientScene.FindLocalObject(msg.prop);
                    pc.UpdateModel(obj);
                    break;
                }
            case PropMessage.Type.Death: {
                    GameObject playerDied = ClientScene.FindLocalObject(msg.player);
                    playerDied.GetComponent<PropController>().DieClient();
                    break;
                }
            case PropMessage.Type.Respawn: {
                    GameObject playerDied = ClientScene.FindLocalObject(msg.player);
                    playerDied.GetComponent<PropController>().RespawnClient();
                    break;
                }
			case PropMessage.Type.Tag: {
					GameObject hunter = ClientScene.FindLocalObject(msg.player);
					hunter.tag = "Hunter";
					break;
				}
        }
    }

    // update the model who turned into a prop in all clients
    public void UpdateModel(GameObject prop) {
        playerAudio.PlayOneShot(objChangeSound, 1f);
        Mesh targetMesh = prop.GetComponent<MeshFilter>().mesh;
        // Since the mesh is to be changed, make the rigid body sleep and adjust the height
        rigidBody.Sleep();
        rigidBody.MovePosition(rigidBody.position + new Vector3(0,
            -(targetMesh.bounds.min.y - playerModel.GetComponent<MeshFilter>().mesh.bounds.min.y),
            0));

		// Change the mesh
        Destroy(playerModel);
        playerModel = Instantiate(prop, graphics.transform.position, graphics.transform.rotation) as GameObject;
        playerModel.transform.parent = graphics.transform;
        playerModel.tag = "Player";
		playerModel.layer = LayerMask.NameToLayer("Player");

        MeshCollider meshCollider = playerModel.GetComponent<MeshCollider>();
        if (meshCollider != null) {
            meshCollider.convex = true; // non-kinematic rigid body can only have a convex mesh collider
        }
    }

    // should be called by the hunter who shot me
    public void TakeDamage(int damage) {
        tookDamage = true;
        // already dead: do nothing
        if (health <= 0)
            return;
        playerAudio.PlayOneShot(damageSound, 1f);
        health -= damage;
        if (health <= 0) {
            health = 0;
        }
    }

    // called on death, only local player
    private void DieLocal() {
        playerAudio.PlayOneShot(deathSound, 1f);
        GetComponent<myPlayerController>().enabled = false;
        StartCoroutine(WaitRespawn(5));
    }

    // called on death, every player
    private void DieClient() {
        dead = true;
        playerModel.GetComponent<Renderer>().material.color = new Color(1.0f, 0.0f, 0.0f);
    }

    // display respawn message
    private IEnumerator WaitRespawn(int seconds) {
        string template = "You died.\nRespawn in {0} second(s).";
        for (int i = 0; i < seconds; ++i) {
            UIText.text = string.Format(template, seconds - i);
            yield return new WaitForSeconds(1);
        }
        RespawnLocal();
        SendPropMessageRespawn();
        UIText.text = "";
    }

    // called on respawn, local player
    private void RespawnLocal() {
        playerAudio.PlayOneShot(respawnSound, 1f);
        GetComponent<myPlayerController>().enabled = true;
    }

    // called on respawn, every player
    private void RespawnClient() {
        // FIRST PART: 
        // reset the model to the default model
        Mesh targetMesh = DefaultModel.GetComponent<MeshFilter>().sharedMesh;
        // transport the player to the spawn point
        rigidBody.Sleep();
        if (isLocalPlayer && hasAuthority) {
            rigidBody.position = spawnPosition;
            rigidBody.rotation = spawnRotation;
        }

        // change the mesh
        Destroy(playerModel);
        playerModel = Instantiate(DefaultModel, graphics.transform.position, graphics.transform.rotation) as GameObject;
        playerModel.transform.parent = graphics.transform;
        playerModel.tag = "Player";

        MeshCollider meshCollider = playerModel.GetComponent<MeshCollider>();
        if (meshCollider != null) {
            meshCollider.convex = true; // non-kinematic rigid body can only have a convex mesh collider
        }

        // SECOND PART:
        // reset health the dead status
        // ONLY ON SERVER!
        if (isServer) {
            health = MaxHealth;
            dead = false;
        }
    }
}
