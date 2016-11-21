using UnityEngine;
using System.Collections;

public class rotate_eyes : MonoBehaviour {
	public float xSpeed = 0.0f;
	public float ySpeed = 0.0f;
	public float zSpeed = 0.0f;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
		transform.Rotate(
			xSpeed * Time.deltaTime,
			ySpeed * Time.deltaTime,
			zSpeed * Time.deltaTime
			);

	}
}
