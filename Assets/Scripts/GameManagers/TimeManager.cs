using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeManager : MonoBehaviour {

	public void OnClickButtonSE()
	{
		Sound.LoadSe("9","9_komaidou");
        Sound.PlaySe("9");
	}	
}
