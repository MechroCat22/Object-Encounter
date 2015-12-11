using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class MenuControl : MonoBehaviour
{

    public void StartLocalGame()
    {
        NetworkManager.singleton.StartHost();
    }

    public void StartLocalGameSound()
    {
        GetComponents<AudioSource>()[0].Play();
        Invoke("StartLocalGame", 1f);
    }

    public void JoinLocalGame()
    {
        if (hostNameInput.text != "Hostname")
        {
            NetworkManager.singleton.networkAddress = hostNameInput.text;
        }
        NetworkManager.singleton.StartClient();
    }

    public void JoinLocalGameSound()
    {
        GetComponents<AudioSource>()[1].Play();
        Invoke("JoinLocalGame", 1f);
    }

    public void StartMatchMaker()
    {
        NetworkManager.singleton.StartMatchMaker();
    }

    public UnityEngine.UI.Text hostNameInput;


    void Start()
    {
        hostNameInput.text = NetworkManager.singleton.networkAddress;
    }

}
