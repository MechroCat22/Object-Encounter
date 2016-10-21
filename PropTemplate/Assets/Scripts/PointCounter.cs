using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
using UnityEngine.UI;

public class PointCounter : NetworkBehaviour {

	private Text pointText;
	public Text firstPersonPointText;
	public Text thirdPersonPointText;
	private float numPoints = 0;
	private bool keepCounting = true;
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
		if (!keepCounting) {
			return;
		}
		if (isServer) {
			numPoints += (Time.deltaTime * 50f);
			numPoints = Mathf.Round (numPoints);

		} else {
			GameObject hunter = GameObject.FindGameObjectWithTag ("Hunter");
			if (hunter == null) {
				return;
			}
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

	public void addKillPoints() {
		numPoints += 1000f;
	}
	public void stopCounting() {
		keepCounting = false;
	}
}
