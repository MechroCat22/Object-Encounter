///////////////////////////////////////////////////////////////////////////////
// File:             PointCounter.cs
// Date:			 November 20 2016
//
// Author:           Andrew Chase chase3@wisc.edu
///////////////////////////////////////////////////////////////////////////////
using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
using UnityEngine.UI;

/// <summary>
/// Keeps track of the score for each player, and displays it to screen
/// </summary>
public class PointCounter : NetworkBehaviour {

	// Text fields, differ based on which camera is in use
	private Text pointText;
	public Text firstPersonPointText;
	public Text thirdPersonPointText;

	// Start points at 0
	private float numPoints = 0;
	private bool keepCounting = true;

	// Initialization
	void Start () {
		// Assign the correct text field
		if (isServer) {
			pointText = firstPersonPointText;
		} else {
			pointText = thirdPersonPointText;
		}
	}
	
	// Update is called once per frame
	void Update () {
		pointText.text = "Points: " + numPoints;
		// Stop counting if the game is over
		if (!keepCounting) {
			return;
		}
		// Seeker gets points constantly
		if (isServer) {
			numPoints += (Time.deltaTime * 50f);
			numPoints = Mathf.Round (numPoints);

		// Hiders get points based on how close they are to the Hunter
		} else {
			GameObject hunter = GameObject.FindGameObjectWithTag ("Hunter");
			if (hunter == null) {
				return;
			}

			// Different levels of points
			// Close - 150 points per frame
			// Mid - 50 points per frame
			// Far - No point changed
			// Very Far - Hider loses points!
			if (Vector3.Distance(this.transform.position, hunter.transform.position) <= 4) {
				numPoints += (Time.deltaTime * 150f);
				numPoints = Mathf.Round (numPoints);
			} else if (Vector3.Distance(this.transform.position, hunter.transform.position) <= 8) {
				numPoints += (Time.deltaTime * 50f);
				numPoints = Mathf.Round (numPoints);
			} else if (Vector3.Distance(this.transform.position, hunter.transform.position) >= 20) {
				numPoints -= (Time.deltaTime * 30f);
				numPoints = Mathf.Round (numPoints);
			}
		}
	}

	// Give seeker 1000 points for catching a seeker
	public void addKillPoints() {
		numPoints += 1000f;
	}

	// When the game ends, stop counting points
	public void stopCounting() {
		keepCounting = false;
	}
}
