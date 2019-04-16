using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class HitEffect : MonoBehaviour
{
    public void ChangePosition(Vector3 pos)
    {
        gameObject.transform.localPosition = pos;
    }

    public IEnumerator HitEffectAnimation()
    {
        Sequence sequence = DOTween.Sequence()
            .OnStart(() =>
            {
                gameObject.transform.localScale = new Vector3(0, 0, 0);
                gameObject.SetActive(true);
                Sound.LoadSe("10", "10_komakougeki");
                Sound.PlaySe("10");
            })
            .Append(gameObject.transform.DOScale(1f, 0.15f).SetEase(Ease.OutSine))
            .Append(gameObject.transform.DOScale(0f, 0.15f).SetEase(Ease.OutSine))
            .OnComplete(() =>
            {
                gameObject.SetActive(false);
            });

        sequence.Play();
		yield return sequence.WaitForCompletion();
    }
}
