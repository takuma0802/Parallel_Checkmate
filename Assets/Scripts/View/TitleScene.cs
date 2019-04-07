using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UniRx;

public class TitleScene : MonoBehaviour
{
    [SerializeField] private Button screenButton;

    void Start()
    {
        StartBGM();
        ObserveScreenButton();
    }

    private void StartBGM()
    {
        Sound.LoadBgm("1", "1_title");
        Sound.LoadSe("5", "5_start");
        Sound.PlayBgm("1");
    }

    private void ObserveScreenButton()
    {
        screenButton.OnClickAsObservable()
            .FirstOrDefault()
            .Subscribe(_ => StartCoroutine(TapScreen()))
            .AddTo(this);
    }


    private IEnumerator TapScreen()
    {
        Sound.PlaySe("5");
        Sound.StopBgm();
        Sound.LoadBgm("2", "2_senryak");
        Sound.PlayBgm("2");
        yield return new WaitForSeconds(1f);
        SceneManager.LoadScene("Main", LoadSceneMode.Single);
    }
}


