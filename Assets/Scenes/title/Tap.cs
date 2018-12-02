using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using  UnityEngine.UI;
public class Tap : MonoBehaviour {
	public GameObject TapEffects;
	public GameObject canvas;
	//[SerializeField]　ParticleSystem tapEffect;
	[SerializeField]　Camera m_camera;
	// Update is called once per frame
	void Start(){
		m_camera = Camera.main;
		Sound.LoadBgm("1","1_title");
		Sound.LoadSe("5","5_start");
		Sound.PlayBgm("1");
		
	}
	void Update () {
		if (Input.GetMouseButtonDown(0)) {
			Vector3 toushScreenPosition = Input.mousePosition;

			GameObject prefab = (GameObject)Instantiate(TapEffects,toushScreenPosition, Quaternion.identity);
			prefab.transform.SetParent (canvas.transform, false); 
			prefab.transform.position = new Vector3(toushScreenPosition.x,toushScreenPosition.y,toushScreenPosition.z);
			Sound.PlaySe("5");

			}
			if (Input.GetMouseButtonUp(0)) {
			Sound.PlaySe("5");

			SceneManager.LoadScene("TestMain");
			Sound.StopBgm();
			}
		}
}
