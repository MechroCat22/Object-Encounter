///////////////////////////////////////////////////////////////////////////////
// File:             DoorMotor.cs
// Date:			 November 20 2016
//
// Authors:          Sizhuo Ma sizhuoma@cs.wisc.edu
//					 Andrew Chase chase3@wisc.edu
///////////////////////////////////////////////////////////////////////////////
using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

/// <summary>
/// Controls the door transform across the network.
/// The server moves the door, and updates the position to the clients
/// </summary>
[RequireComponent(typeof(Rigidbody))]
public class DoorMotor : NetworkBehaviour
{

    // Axis to rotate around
    public float AxisX = 0.0f;
    public float AxisY = 0.0f;
    public float AxisZ = 0.0f;

    // Degree when the door is completely open
    public float MaxDegree = 90.0f;
    // Max degree is positive or negative
    public bool PositiveDirection = true;
    // Time it takes to reach the max degree
    public float OpenTime = 0.5f;
	// Current degree of the door
    private float currDegree;
	// Enum for the current state of the door
    private enum State { Opening, Closing, Static };

	// State of the door across the network, updated continuously by the server
    [SyncVar]
    private State currState;

    // Initialization
    void Start()
    {
        currDegree = 0;
        currState = State.Static;
        GetComponent<Rigidbody>().isKinematic = true;
    }

    void FixedUpdate()
    {
        float currSpeed;
        float deltaDegree;

		// Update the door position based on the state
		// Do nothing of the door is static
        switch (currState)
        {
            case State.Opening:
                // current velocity depends on current position
                currSpeed = Mathf.Sqrt(2 * MaxDegree * (MaxDegree - currDegree)) / OpenTime;
                deltaDegree = currSpeed * Time.deltaTime;
                if (currDegree + deltaDegree > MaxDegree)
                {
                    deltaDegree = MaxDegree - currDegree;
                    currDegree = MaxDegree;
                    currState = State.Static;
                }
                else
                {
                    currDegree += deltaDegree;
                }
				// Move the door
                transform.RotateAround(transform.TransformPoint(new Vector3(AxisX, AxisY, AxisZ)), new Vector3(0, 1, 0), 
                    PositiveDirection ? deltaDegree : -deltaDegree);
                break;

            case State.Closing:
                currSpeed = -Mathf.Sqrt(2 * MaxDegree * currDegree) / OpenTime;
                deltaDegree = currSpeed * Time.deltaTime;
                if (currDegree + deltaDegree < 0)
                {
                    deltaDegree = -currDegree;
                    currDegree = 0;
                    currState = State.Static;
                }
                else
                {
                    currDegree += deltaDegree;
                }
				// Move the door
                transform.RotateAround(transform.TransformPoint(new Vector3(AxisX, AxisY, AxisZ)), new Vector3(0, 1, 0), 
                    PositiveDirection ? deltaDegree : -deltaDegree);
                break;

        }
    }
		
	/*
	 * Moves initiates movement of the door, called by a client
	 **/
    public void Move()
    {
        Debug.Log(isServer + " " + currState + " " + currDegree);
        switch (currState)
        {
            case State.Closing:
                currState = State.Opening;
                break;
            case State.Opening:
                currState = State.Closing;
                break;
            case State.Static:
                if (currDegree == 0)
                {
                    currState = State.Opening;
                }
                else
                {
                    currState = State.Closing;
                }
                break;
        }
    }
}
