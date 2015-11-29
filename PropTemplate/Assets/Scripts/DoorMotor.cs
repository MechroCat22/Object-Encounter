using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

[RequireComponent(typeof(Rigidbody), typeof(NetworkTransform))]
public class DoorMotor : NetworkBehaviour {

    // axis to rotate around
    public float AxisX;
    public float AxisY;
    public float AxisZ;
    // degree when the door is completely open
    public float MaxDegree;
    // time it takes to reach the max degree
    public float OpenTime = 0.5f;

    //[SyncVar]
    private float currDegree;
    private enum State { Opening, Closing, Static };
    //[SyncVar]
    private State currState;

    // Use this for initialization
    void Start() {
        currDegree = 0;
        currState = State.Static;
        GetComponent<Rigidbody>().isKinematic = true;
        GetComponent<NetworkTransform>().transformSyncMode = NetworkTransform.TransformSyncMode.SyncTransform;
    }

    void FixedUpdate() {
        if (!isServer)
            return;

        float currSpeed;
        float deltaDegree;
        switch (currState) {
            case State.Opening:
                // current velocity depends on current position
                currSpeed = Mathf.Sqrt(2 * MaxDegree * (MaxDegree - currDegree)) / OpenTime;
                deltaDegree = currSpeed * Time.deltaTime;
                if (currDegree + deltaDegree > MaxDegree) {
                    deltaDegree = MaxDegree - currDegree;
                    currDegree = MaxDegree;
                    currState = State.Static;
                }
                else {
                    currDegree += deltaDegree;
                }
                transform.RotateAround(transform.TransformPoint(new Vector3(AxisX, AxisY, AxisZ)), new Vector3(0, 1, 0), deltaDegree);
                break;

            case State.Closing:
                currSpeed = -Mathf.Sqrt(2 * MaxDegree * currDegree) / OpenTime;
                deltaDegree = currSpeed * Time.deltaTime;
                if (currDegree + deltaDegree < 0) {
                    deltaDegree = -currDegree;
                    currDegree = 0;
                    currState = State.Static;
                }
                else {
                    currDegree += deltaDegree;
                }
                transform.RotateAround(transform.TransformPoint(new Vector3(AxisX, AxisY, AxisZ)), new Vector3(0, 1, 0), deltaDegree);
                break;

        }
    }

    //[Command]
    public void Move() {
        Debug.Log(isServer + " " + currState + " " + currDegree);
        switch (currState) {
            case State.Closing:
                currState = State.Opening;
                break;
            case State.Opening:
                currState = State.Closing;
                break;
            case State.Static:
                if (currDegree == 0)
                    currState = State.Opening;
                else
                    currState = State.Closing;
                break;
        }
    }
}
