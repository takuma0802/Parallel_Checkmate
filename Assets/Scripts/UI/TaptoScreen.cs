using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UniRx;

public class TaptoScreen : MonoBehaviour {

	public Button button;
	public GameObject TapImage;
	public bool TapState;
	void Start () {
		TapState = true;
        StartCoroutine(loop());
	}
	
	private IEnumerator loop()
    {
        while (TapState)
        {
            TapImage.SetActive(true);
            yield return new WaitForSeconds(0.8f); ;
            TapImage.SetActive(false);
            yield return new WaitForSeconds(0.5f); ;
        }
    }

	public void DisTap()
	{
		TapState = false;
	}
}
