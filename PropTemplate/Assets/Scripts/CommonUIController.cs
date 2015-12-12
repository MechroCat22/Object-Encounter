using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class CommonUIController : MonoBehaviour {

    private Text timerText;
    private Timer timer;

    private RawImage screenBackground;

    private bool gameOver;

	// Use this for initialization
	void Start () {
        timerText = gameObject.transform.Find("TimerText").GetComponent<Text>();
        timer = GameObject.Find("Timer").GetComponent<Timer>();
        screenBackground = gameObject.transform.Find("ScreenBackground").GetComponent<RawImage>();

        gameOver = false;
	}
	
	// Update is called once per frame
	void Update () {
        timerText.text = string.Format("{0:D2}:{1:D2}", timer.MinutesLeft, timer.SecondsLeft);

        if (!gameOver && timer.GameOver()) {
            gameOver = true;
            screenBackground.enabled = true;
            screenBackground.canvasRenderer.SetAlpha(0.01f);
            screenBackground.CrossFadeAlpha(1.0f, 1.0f, false);
            Debug.Log("Game Over. Screen black out.");
        }
	}
}
