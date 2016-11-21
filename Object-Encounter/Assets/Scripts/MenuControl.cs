///////////////////////////////////////////////////////////////////////////////
// File:             MenuControl.cs
// Date:			 November 20 2016
//
// Author:           Andrew Chase chase3@wisc.edu
///////////////////////////////////////////////////////////////////////////////
using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

/// <summary>
/// Wrapper for starting the network game from the Main Menu UI
/// </summary>
public class MenuControl : MonoBehaviour
{
	// Start a local game
    public void StartLocalGame()
    {
        NetworkManager.singleton.StartHost();
    }

	// Sound effect
    public void StartLocalGameSound()
    {
        GetComponents<AudioSource>()[0].Play();
        Invoke("StartLocalGame", 1f);
    }

	// Join game as a client
    public void JoinLocalGame()
    {
        if (hostNameInput.text != "Hostname")
        {
            NetworkManager.singleton.networkAddress = hostNameInput.text;
        }
        NetworkManager.singleton.StartClient();
    }

	// Sound effect
    public void JoinLocalGameSound()
    {
        GetComponents<AudioSource>()[1].Play();
        Invoke("JoinLocalGame", 1f);
    }

	// Matchmaker functionality, not implemented in this game
    public void StartMatchMaker()
    {
        NetworkManager.singleton.StartMatchMaker();
    }

	// text for host address
    public UnityEngine.UI.Text hostNameInput;

	// Set up address text
    void Start()
    {
        hostNameInput.text = NetworkManager.singleton.networkAddress;
    }

}
