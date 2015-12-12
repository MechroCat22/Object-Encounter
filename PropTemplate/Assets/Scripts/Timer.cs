using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class Timer : NetworkBehaviour {

    [HideInInspector, SyncVar]
    public int MinutesLeft;

    [HideInInspector, SyncVar]
    public int SecondsLeft;

    // time for each round in seconds
    public int RoundSeconds = 180;

	// Use this for initialization
	void Start () {
        if (!isServer)
            return;

        // TEMPORARY SOLUTION: timer starts when the server (the local client) enters the game scene
        int absoluteTime = RoundSeconds - (int)Time.timeSinceLevelLoad;
        SecondsLeft = absoluteTime % 60;
        MinutesLeft = absoluteTime / 60;
	}
	
	// Update is called once per frame
	void Update () {
        if (!isServer)
            return;

        int absoluteTime = RoundSeconds - (int)Time.timeSinceLevelLoad;
        if (absoluteTime < 0)
            absoluteTime = 0;

        int newSeconds = absoluteTime % 60;
        int newMinutes = absoluteTime / 60;

        if (newSeconds != SecondsLeft)
            SecondsLeft = newSeconds;

        if (newMinutes != MinutesLeft)
            MinutesLeft = newMinutes;
	}

    public bool GameOver() {
        return MinutesLeft == 0 && SecondsLeft == 0;
    }
}
