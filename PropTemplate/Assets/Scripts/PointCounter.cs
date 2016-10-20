using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
using UnityEngine.UI;

public class PointCounter : NetworkBehaviour {

	private Text pointText;
	public Text firstPersonPointText;
	public Text thirdPersonPointText;
	private float numPoints = 0;
	// Use this for initialization
	void Start () {
		if (isServer) {
			pointText = firstPersonPointText;
		} else {
			pointText = thirdPersonPointText;
		}
	}
	
	// Update is called once per frame
	void Update () {
		pointText.text = "Points: " + numPoints;

		if (isServer) {
			numPoints += (Time.deltaTime * 50f);
			numPoints = Mathf.Round (numPoints);
		} else {
			GameObject hunter = GameObject.FindGameObjectWithTag ("Hunter");
			if (Vector3.Distance(this.transform.position, hunter.transform.position) <= 8) {
				numPoints += (Time.deltaTime * 100f);
				numPoints = Mathf.Round (numPoints);
			}
		}
	}
}
