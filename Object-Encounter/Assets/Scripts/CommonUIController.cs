///////////////////////////////////////////////////////////////////////////////
// File:             CommonUIController.cs
// Date:			 November 20 2016
//
// Authors:          Sizhuo Ma sizhuoma@cs.wisc.edu
// 					 Andrew Chase chase3@wisc.edu
///////////////////////////////////////////////////////////////////////////////
using UnityEngine;
using UnityEngine.UI;
using System.Collections;

/// <summary>
/// Controller used for timer prompt and screen blackout at end
/// </summary>
public class CommonUIController : MonoBehaviour {

	// Prompt for timer
    private Text timerText;
	// Timer itself
    private Timer timer;
	// Texture used for screen blackout
    private RawImage screenBackground;
	// Flag for determining when the game ends
    private bool gameOver;

	// Initialization
	void Start () {
        timerText = gameObject.transform.Find("TimerText").GetComponent<Text>();
        timer = GameObject.Find("Timer").GetComponent<Timer>();
        screenBackground = gameObject.transform.Find("ScreenBackground").GetComponent<RawImage>();
        gameOver = false;
	}
	
	// Update is called once per frame
	void Update () {
        timerText.text = string.Format("{0:D2}:{1:D2}", timer.MinutesLeft, timer.SecondsLeft);

		// If the timer runs out, fade the screen to black
        if (!gameOver && timer.GameOver()) {
            gameOver = true;
            screenBackground.enabled = true;
            screenBackground.canvasRenderer.SetAlpha(0.01f);
            screenBackground.CrossFadeAlpha(1.0f, 1.0f, false);
            Debug.Log("Game Over. Screen black out.");
        }
	}
}
