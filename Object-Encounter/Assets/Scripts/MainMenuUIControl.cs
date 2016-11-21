///////////////////////////////////////////////////////////////////////////////
// File:             MainMenuUIControl.cs
// Date:			 November 20 2016
//
// Author:           Andrew Chase chase3@wisc.edu
///////////////////////////////////////////////////////////////////////////////
using UnityEngine;
using System.Collections;

/// <summary>
/// Controller used for interacting with the main menu
/// For debugging only
/// </summary>
public class MainMenuUIControl : MonoBehaviour {

    private Camera myCamera;
	// Use this for initialization
	void Start () {

        myCamera = GetComponent<Camera>();
	}
	
	// Update is called once per frame
	void Update () {
        
        Ray camRay = myCamera.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2));
        RaycastHit objectHit;
        if (Physics.Raycast(camRay, out objectHit, 60))
        {
            GameObject obj = objectHit.transform.gameObject;
            // aiming at a button
            if (obj.tag.Equals("Button") && Input.GetButtonDown("Fire1"))
            {
                Debug.Log("button hit");
            }
        }
    }
}
