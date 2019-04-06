using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GoHomeButton : MonoBehaviour {
	
	public void ReplayGame(){
		SceneManager.LoadScene("Title",LoadSceneMode.Single);
	}
}
