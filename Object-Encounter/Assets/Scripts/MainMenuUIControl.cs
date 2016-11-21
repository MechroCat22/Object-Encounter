using UnityEngine;
using System.Collections;
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
