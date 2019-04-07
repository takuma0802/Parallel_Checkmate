using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UniRx;

public class ResultManager : MonoBehaviour
{
    [SerializeField] private Image resultImage;
    [SerializeField] private Sprite[] winnerSprites;
    [SerializeField] private Button backToHomeButton;

    // 引数result => 1:1P勝利, 2:2P勝利, 3:引き分け
    public void ShowResult(int result)
    {
        StartResultBGM(result);

        resultImage.sprite = winnerSprites[result - 1];
        ActivateResultUI();
        ObserveButton();
    }

    private void StartResultBGM(int result)
    {
        Sound.StopBgm();

        if (result == 3)
        {
            Sound.LoadBgm("19", "19_draw");
            Sound.PlayBgm("19");
        }
        else
        {
            Sound.LoadBgm("4", "4_result_?");
            Sound.PlayBgm("4");
        }
    }

    private void ActivateResultUI()
    {
        this.gameObject.SetActive(true);
    }

    public void DiactivateResultUI()
    {
        this.gameObject.SetActive(false);
    }

    private void ObserveButton()
    {
        backToHomeButton.OnClickAsObservable()
            .FirstOrDefault()
            .Subscribe(_ => BackToHome())
            .AddTo(this);
    }

    public void BackToHome(){
		SceneManager.LoadScene("Title",LoadSceneMode.Single);
	}
}
