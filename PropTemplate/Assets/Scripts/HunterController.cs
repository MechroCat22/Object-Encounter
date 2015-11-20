using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using UnityStandardAssets.CrossPlatformInput;

public class HunterController : NetworkBehaviour {

    private float timeCounter = 0.3f;

    private const float FireRate = 0.3f;
    private const int Damage = 34;
    private const float CamRayLength = 100f;

    [SyncVar]
    private bool isActive;

    private Camera myCamera;
    private ParticleSystem psys;

	// Use this for initialization
	void Start () {
        myCamera = transform.Find("Camera").GetComponent<Camera>();
        Transform gun = myCamera.transform.Find("Gun");
        Transform psh = gun.Find("ParticleSystemHolder");
        Transform ps = psh.Find("Particle System");
        psys = ps.GetComponent<ParticleSystem>();
        //psys = myCamera.transform.Find("Gun").Find("ParticleSystemHolder").Find("Particle System").GetComponent<ParticleSystem>();

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
	}
	
	// Update is called once per frame
	void Update () {
        // return if I am not a hunter player
        if (!isActive || !isLocalPlayer) 
            return;

        // time counter: can only shoot when a certain amount of time (FireRate) has passed
        timeCounter += Time.deltaTime;
        if (CrossPlatformInputManager.GetButton("Fire1") && timeCounter > FireRate) {
            // reset time counter
            timeCounter = 0.0f;

            // play the muzzle flash effect
            psys.Play();

            // the aim is locked at the center of the screen
            Ray camRay = myCamera.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2));
            RaycastHit objectHit;

            if (Physics.Raycast(camRay, out objectHit, CamRayLength)) {
                GameObject obj = objectHit.transform.gameObject;
                // only do the following when the tag is "Player"
                if (obj.tag.Equals("Player")) {
                    //GameObject playerHit = obj.transform.parent.parent.gameObject;
                    GameObject playerHit = obj;
                    playerHit.GetComponent<PropController>().TakeDamage(Damage);
                    Debug.Log("Hit: " + playerHit);
                }
            }
            else {
                Debug.Log("Miss");
            }
        }
	}
}
