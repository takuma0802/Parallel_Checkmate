using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class TitleBack : MonoBehaviour {
	void Start () {
		GetComponent<Button>().OnClickAsObservable()
			.First()
			.Subscribe(_ => SceneManager.LoadScene("Title",LoadSceneMode.Single));
	}
}
