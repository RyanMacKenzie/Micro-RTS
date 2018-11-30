using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour {

	
	public void LoadLobby () {
        SceneManager.LoadScene("lobby");
	}
	
	
	public void Quit () {
        Application.Quit();
	}
}
