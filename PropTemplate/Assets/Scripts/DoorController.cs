using UnityEngine;
using System.Collections;

public class DoorController : MonoBehaviour {

    // axis to rotate around
    public float AxisX;
    public float AxisY;
    public float AxisZ;
    // degree when the door is completely open
    public float MaxDegree;
    // time it takes to reach the max degree
    public float OpenTime = 0.5f;

    private float currDegree;
    private enum State { Opening, Closing, Static };
    private State currState;

    // Use this for initialization
    void Start() {
        currDegree = 0;
        currState = State.Static;
    }

    void FixedUpdate() {
        float currSpeed;
        float deltaDegree;
        switch (currState) {
            case State.Opening:
                // current velocity depends on current position
                currSpeed = Mathf.Sqrt(2 * MaxDegree * currDegree) / OpenTime;
                deltaDegree = currSpeed * Time.deltaTime;
                if (currDegree + deltaDegree > MaxDegree) {
                    deltaDegree = MaxDegree - currDegree;
                    currDegree = MaxDegree;
                    currState = State.Static;
                }
                else {
                    currDegree += deltaDegree;
                }
                transform.RotateAround(transform.position, new Vector3(AxisX, AxisY, AxisZ), deltaDegree);
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
                transform.RotateAround(transform.position, new Vector3(AxisX, AxisY, AxisZ), deltaDegree);
                break;

        }
    }
}
