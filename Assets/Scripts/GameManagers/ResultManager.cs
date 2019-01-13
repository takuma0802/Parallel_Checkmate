using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ResultManager : MonoBehaviour
{
    [SerializeField] private Image resultImage;
    [SerializeField] private Sprite[] winnerSprites;

    void Start()
    {

    }

    public void ShowResultUI(int result)
    {
        Sound.StopBgm();
        resultImage.sprite = winnerSprites[result - 1];
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
        this.gameObject.SetActive(true);
    }

    public void HideResultUI()
    {
        this.gameObject.SetActive(false);
    }
}
