using UnityEngine;
using UnityEngine.Networking;

public class PlayerSetup : NetworkBehaviour {

    [SerializeField]
    Behaviour[] componentsToDisable;
	[SerializeField] 
	private GameObject firstPersonCamera;
	[SerializeField] 
	private GameObject thirdPersonCamera;

	public bool firstPerson;
	//private Vector3 camPosition;
	//private Quaternion camRotation;

    Camera sceneCamera;

    void Start()
    {
        if (!isLocalPlayer)
        {
            for (int i = 0; i < componentsToDisable.Length; i++)
            {
                componentsToDisable[i].enabled = false;
            }
			disableAll (firstPersonCamera);		
			disableAll (thirdPersonCamera);
			GetComponent<myViveController> ().enabled = false;
        }
        else
        {
            sceneCamera = Camera.main;
            if (sceneCamera != null)
            {
                sceneCamera.gameObject.SetActive(false);
            }
			 if (!isServer) {
				disableAll (firstPersonCamera);	
				GetComponent<myViveController> ().enabled = false;
				//thirdPersonCamera.SetActive (true);
			} 
			else {
				GetComponent<myPlayerController> ().enabled = false;
				//firstPersonCamera.SetActive (true);
				disableAll (thirdPersonCamera);		
				//GetComponent<PropController> ().enabled = false;
				// disable hunter's mesh so it won't obstruct first person camera view
				//this.transform.Find("Graphics").Find("Player Model").GetComponent<MeshRenderer>().enabled = false;
			}
        }

    }
		

    void OnDisable()
    {
        if (sceneCamera != null)
        {
            sceneCamera.gameObject.SetActive(true);
        }
    }

	void disableAll(GameObject cam) {
		// Disable the object
		cam.SetActive(false);
		foreach (Behaviour comp in cam.GetComponents<Behaviour>()) {
			comp.enabled = false;
		}
	}


}
