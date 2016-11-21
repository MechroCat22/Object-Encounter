using UnityEngine;
using System.Collections;

public class scroll : MonoBehaviour {
	RectTransform rTrans;
	private float right_scroll = -0.001f;
	float left_scroll = 0.001f;
	float scroll_final;
	public float bpm_scroll = 90.0F;
	public int beatsPerMeasure = 4;
	private double singleMeasureTime;
	private double delayEvent;
	private bool running = false;
	private int scroll_count;
	void Start () {
		bpm_scroll = 90.0F;
		beatsPerMeasure = 4;
		scroll_count = 1;
		singleMeasureTime = AudioSettings.dspTime + 2.0F;
		running = true;
		rTrans = (RectTransform) transform.GetComponent<RectTransform>();
	}
	
	// Update is called once per frame
	void Update () {
		if (!running)
			return;
		double time = AudioSettings.dspTime;

		//THE most important part of this script: this is the metronome, keeping count of the measures and making sure the audio is in sync
		if (time + 1.0F > singleMeasureTime) {

			if (scroll_count==1){
				scroll_final = right_scroll;
			}
			if (scroll_count==2){
				scroll_final = left_scroll;
				scroll_count = 0;
			}

			scroll_count +=1;
			singleMeasureTime += 60.0F / bpm_scroll * beatsPerMeasure;
		}

		Vector3 temp = new Vector3(0,scroll_final,0);
		rTrans.transform.localScale += temp;
	}
}
