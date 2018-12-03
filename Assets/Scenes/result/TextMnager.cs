using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TextMnager : MonoBehaviour {
	
	public GameObject win1;
	public GameObject win2;
	public GameObject draw;
	public GameObject button;

	// Use this for initialization
	void Start () {
		win1.SetActive (false);
		win2.SetActive (false);
		draw.SetActive (false);
		button.SetActive (false);
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetKey (KeyCode.A))
		Player1();
		if (Input.GetKey (KeyCode.S))
		Player2();
		if (Input.GetKey (KeyCode.D))
		Player3();

		}
	void Player1(){
		Sound.StopBgm();
		Sound.LoadBgm("4", "4_result_?");
		Sound.PlayBgm("4");
		win1.SetActive (true);
		button.SetActive (true);

	}
	void Player2(){
		Sound.StopBgm();
		Sound.LoadBgm("4", "4_result_?");
        Sound.PlayBgm("4");
		win2.SetActive (true);
		button.SetActive (true);

	}
	void Player3(){
		Sound.StopBgm();
		Sound.LoadBgm("19", "19_draw");
		Sound.PlayBgm("19");
		draw.SetActive (true);
		button.SetActive (true);

	}

	public void ReplayGame(){
		SceneManager.LoadScene("Title");
	}

	
}
