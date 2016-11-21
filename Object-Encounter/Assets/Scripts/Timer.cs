///////////////////////////////////////////////////////////////////////////////
// File:             Timer.cs
// Date:			 November 20 2016
//
// Author:           Sizhuo Ma sizhuoma@cs.wisc.edu
//					 Andrew Chase chase3@wisc.edu
///////////////////////////////////////////////////////////////////////////////
using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

/// <summary>
/// Global timer for the game, which is synced across the network
/// Starts at 5 minutes, and game ends when the timer reaches 0
/// </summary>
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

        // TEMPORARY SOLUTION: timer starts when the server starts the game (and
		// enters first)
        int absoluteTime = RoundSeconds - (int)Time.timeSinceLevelLoad;
        SecondsLeft = absoluteTime % 60;
        MinutesLeft = absoluteTime / 60;
	}
	
	// Update is called once per frame
	void Update () {

		// Only the server updates the timer
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

	// Check if the game has ended
    public bool GameOver() {
        return MinutesLeft == 0 && SecondsLeft == 0;
    }
}
