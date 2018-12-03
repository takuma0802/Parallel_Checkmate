using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Tap : MonoBehaviour
{
    //public float interval = 1.0f;   // 点滅周期

    public GameObject TapImage;
    public GameObject TapEffects;
    public bool Tapstate;

    public GameObject canvas;

    //[SerializeField]　ParticleSystem tapEffect;
    [SerializeField] Camera m_camera;
    // Update is called once per frame
    void Start()
    {
        Tapstate = true;
        m_camera = Camera.main;
        Sound.LoadBgm("1", "1_title");
        Sound.LoadSe("5", "5_start");
        Sound.PlayBgm("1");
        StartCoroutine(loop());

    }
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector3 toushScreenPosition = Input.mousePosition;
            GameObject prefab = (GameObject)Instantiate(TapEffects, toushScreenPosition, Quaternion.identity);
            prefab.transform.SetParent(canvas.transform, false);
            prefab.transform.position = new Vector3(toushScreenPosition.x, toushScreenPosition.y, toushScreenPosition.z);
            Sound.PlaySe("5");
        }
        if (Input.GetMouseButtonUp(0))
        {
            Sound.PlaySe("5");
            SceneManager.LoadScene("TestMain");
            Sound.StopBgm();
            Sound.LoadBgm("2", "2_senryak");
            Sound.PlayBgm("2");
            Tapstate = false;

        }
        if (Input.GetKey(KeyCode.A))
        {
            SceneManager.LoadScene("resultscene");
            Sound.StopBgm();
        }
    }
    private IEnumerator loop()
    {
        while (Tapstate)
        {
            TapImage.SetActive(true);
            yield return new WaitForSeconds(0.8f); ;
            TapImage.SetActive(false);
            yield return new WaitForSeconds(0.5f); ;
            Debug.Log("ok");


        }
    }
}


