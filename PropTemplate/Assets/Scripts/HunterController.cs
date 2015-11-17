using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using UnityStandardAssets.CrossPlatformInput;

public class HunterController : NetworkBehaviour {

    private float timeCounter = 0.3f;
    private int playerMask; 

    private const float FireRate = 0.3f;
    private const int Damage = 34;
    private const float CamRayLength = 100f;

    private Camera myCamera;

	// Use this for initialization
	void Start () {
        playerMask = LayerMask.GetMask("Player");
        myCamera = transform.Find("Camera").GetComponent<Camera>();
	}
	
	// Update is called once per frame
	void Update () {
        // TEMPORARY solution: the server is the hunter
        if (!isServer)
            return;

        // time counter: can only shoot when a certain amount of time (FireRate) has passed
        timeCounter += Time.deltaTime;
        if (CrossPlatformInputManager.GetButton("Fire1") && timeCounter > FireRate) {
            // reset time counter
            timeCounter = 0.0f;

            // the aim is locked at the center of the screen
            Ray camRay = myCamera.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2));
            RaycastHit objectHit;

            if (Physics.Raycast(camRay, out objectHit, CamRayLength, playerMask)) {
                GameObject obj = objectHit.transform.gameObject;
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
