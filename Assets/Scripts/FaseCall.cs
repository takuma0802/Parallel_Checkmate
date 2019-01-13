using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FaseCall : MonoBehaviour {
	private string a,b,c;
	public Text aText;
	public GameObject canvas;//キャンバス
  public GameObject text;
	//public Rect zan = new Rect(0,0,100,50);
	// Use this for initialization
	void Start () {
		a = "idou";
        b = "battle";
		c = "sen";
		//Awake();
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetKey (KeyCode.W))
		aprint(a);
	    //aText.text = a;
		if (Input.GetKey (KeyCode.A))
		aprint(b);
	    //aText.text = a;
			if (Input.GetKey (KeyCode.D))
		aprint(c);
	    //aText.text = a;

	}
	void aprint(string moji){
		aText.text = moji;
		
	}
	/* void Awake(){
    GameObject prefab = (GameObject)Instantiate (text);
    prefab.transform.SetParent (canvas.transform, false); 
}*/
}
