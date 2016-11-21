///////////////////////////////////////////////////////////////////////////////
// File:            DoorController.cs
// Date:			November 20 2016
//
// Authors:         Sizhuo Ma sizhuoma@cs.wisc.edu
//				    Andrew Chase chase3@wisc.edu
///////////////////////////////////////////////////////////////////////////////
using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

/// <summary>
/// Controller used to interact with doors, signalling them to open
/// and close across the network
/// </summary>
public class DoorController : NetworkBehaviour {

	// Command used to force the door to move on the server
    [Command]
    public void CmdMoveDoor(GameObject door) {
		if (!door.tag.Equals("Door")) {
			return;
		}

		// Get the script that handles door movement, and call Move
        DoorMotor dm = door.GetComponent<DoorMotor>();
        dm.Move();
    }
}
