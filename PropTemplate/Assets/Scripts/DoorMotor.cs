using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

[RequireComponent(typeof(Rigidbody))]
public class DoorMotor : NetworkBehaviour
{

    // axis to rotate around
    public float AxisX = 0.0f;
    public float AxisY = 0.0f;
    public float AxisZ = 0.0f;
    // degree when the door is completely open
    public float MaxDegree = 90.0f;
    // time it takes to reach the max degree
    public float OpenTime = 0.5f;

    //private NetworkTransform networkTransform;

    //[SyncVar]
    private float currDegree;
    private enum State { Opening, Closing, Static };
    [SyncVar]
    private State currState;

    // Use this for initialization
    void Start()
    {
        currDegree = 0;
        currState = State.Static;
        GetComponent<Rigidbody>().isKinematic = true;
        //networkTransform = GetComponent<NetworkTransform>();
        //networkTransform.transformSyncMode = NetworkTransform.TransformSyncMode.SyncTransform;
        //networkTransform.sendInterval = 29;
    }

    void FixedUpdate()
    {
        //if (!isServer)
        //    return;

        float currSpeed;
        float deltaDegree;
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
                transform.RotateAround(transform.TransformPoint(new Vector3(AxisX, AxisY, AxisZ)), new Vector3(0, 1, 0), deltaDegree);

                //networkTransform.SetDirtyBit(1u);
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
                transform.RotateAround(transform.TransformPoint(new Vector3(AxisX, AxisY, AxisZ)), new Vector3(0, 1, 0), deltaDegree);
                //networkTransform.SetDirtyBit(1u);
                break;

        }
    }

    //[Command]
    public void Move()
    {
        Debug.Log(isServer + " " + currState + " " + currDegree);
        switch (currState)
        {
            case State.Closing:
                currState = State.Opening;
                //GetComponents<AudioSource>()[0].Play();
                break;
            case State.Opening:
                currState = State.Closing;
                //GetComponents<AudioSource>()[1].Play();
                break;
            case State.Static:
                if (currDegree == 0)
                {
                    currState = State.Opening;
                    //GetComponents<AudioSource>()[0].Play();
                }
                else
                {
                    currState = State.Closing;
                    //GetComponents<AudioSource>()[1].Play();
                }
                break;
        }
    }
}
