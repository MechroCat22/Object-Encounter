///////////////////////////////////////////////////////////////////////////////
// File:             PlayerSetup.cs
// Date:			 November 20 2016
//
// Author:           Andrew Chase chase3@wisc.edu
///////////////////////////////////////////////////////////////////////////////
using UnityEngine;
using UnityEngine.Networking;

/// <summary>
/// Sets up the player for network gameplay, including disabling components on
/// player objects that aren't local authority
/// </summary>
public class PlayerSetup : NetworkBehaviour {

    [SerializeField]
    Behaviour[] componentsToDisable;
	[SerializeField] 
	private GameObject firstPersonCamera;
	[SerializeField] 
	private GameObject thirdPersonCamera;

	public bool firstPerson;

	// Camera viewing the scene, only used at the main menu
    Camera sceneCamera;

	// Initialization
    void Start()
    {
		// If the object in the scene (which has this script, so is a player object)
		// isn't the object we control, disable all the scripts to avoid moving it with
		// input
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
			// Set up player based on whether they are a hider or seeker
            sceneCamera = Camera.main;
            if (sceneCamera != null)
            {
                sceneCamera.gameObject.SetActive(false);
            }
			 if (!isServer) {
				disableAll (firstPersonCamera);	
				GetComponent<myViveController> ().enabled = false;
			} 
			else {
				GetComponent<myPlayerController> ().enabled = false;
				disableAll (thirdPersonCamera);		
			}
        }

    }
		
	// Ensure that the scene camera gets re-enabled to prevent black screens
    void OnDisable()
    {
        if (sceneCamera != null)
        {
            sceneCamera.gameObject.SetActive(true);
        }
    }

	// Disables all scripts in the child objects
	void disableAll(GameObject cam) {
		// Disable the object
		cam.SetActive(false);
		foreach (Behaviour comp in cam.GetComponents<Behaviour>()) {
			comp.enabled = false;
		}
	}


}
