using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class DoorController : NetworkBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    [Command]
    public void CmdMoveDoor(GameObject door) {
        DoorMotor dm = door.GetComponent<DoorMotor>();
        dm.Move();
    }
}
