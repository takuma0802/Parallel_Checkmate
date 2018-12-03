using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TextMnager : MonoBehaviour {
	
	public GameObject win1;
	public GameObject win2;
	public GameObject draw;
	public GameObject race;
	public GameObject button;

	// Use this for initialization
	void Start () {
		win1.SetActive (false);
		win2.SetActive (false);
		draw.SetActive (false);
		race.SetActive (false);
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
		//bgmload
		//bgmplay
		win1.SetActive (true);
		race.SetActive (true);
		button.SetActive (true);

	}
	void Player2(){
		win2.SetActive (true);
		race.SetActive (true);
		button.SetActive (true);

	}
	void Player3(){
		draw.SetActive (true);
		race.SetActive (true);
		button.SetActive (true);

	}

	
}
