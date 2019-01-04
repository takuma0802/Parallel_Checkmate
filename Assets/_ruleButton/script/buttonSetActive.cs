using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class buttonSetActive : MonoBehaviour {
    public GameObject ruleImageButton;

	// Use this for initialization
	void Start () {
        ruleImageButton.gameObject.SetActive(false);
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void PushTrueButton()
    {
        ruleImageButton.gameObject.SetActive(true);
    }
    public void PushFalseButton()
    {
        ruleImageButton.gameObject.SetActive(false);
    }
}
