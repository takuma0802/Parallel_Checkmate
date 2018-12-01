using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class HorseManager : MonoBehaviour
{
    private float sizeRate;
    private int columnSize;
    private int buffer;
    private Vector3 horsePos;

    public void DestroyHorse()
    {
        Destroy(this.gameObject);
    }

    public IEnumerator SetHorse(Vector3 targetPos, float sizeRate, int columnSize)
    {
        this.sizeRate = sizeRate;
        this.columnSize = columnSize;
        SetImageSize();

        buffer = 5 * (columnSize - 3);
        targetPos = new Vector3(targetPos.x, targetPos.y - (60 + buffer), targetPos.z);
        GetComponent<RectTransform>().localPosition = targetPos;
        horsePos = GetComponent<RectTransform>().localPosition;
        yield return null;
    }

    private void SetImageSize()
    {
        float newSize = 252 * sizeRate;
        GetComponent<RectTransform>().sizeDelta = new Vector2(newSize, newSize);
    }

    public IEnumerator MoveHorse(Vector3 targetPos, bool isUndo)
    {
        targetPos = new Vector3(targetPos.x, targetPos.y - (60 + buffer), targetPos.z);
        if (!isUndo)
        {
            var targets = CalcMovePoints(targetPos);
            for (var i = 0; i < targets.Length - 1; i++)
                yield return MoveHorse(targets[i], targets[i + 1]);
        }
        GetComponent<RectTransform>().localPosition = targetPos;
        horsePos = gameObject.transform.localPosition;
        yield return null;
    }

    private Vector3[] CalcMovePoints(Vector3 targetPos)
    {
        Vector3[] movePoints;
        float distanceX = targetPos.x - horsePos.x;
        float distanceY = targetPos.y - horsePos.y;
        if (Mathf.Abs(distanceX) > Mathf.Abs(distanceY))
        {
            Vector3 movePos1 = new Vector3(horsePos.x + (distanceX / 2), horsePos.y, targetPos.z);
            Vector3 movePos2 = new Vector3(targetPos.x, horsePos.y, targetPos.z);
            movePoints = new Vector3[] { horsePos, movePos1, movePos2, targetPos };
        }
        else
        {
            Vector3 movePos1 = new Vector3(horsePos.x, horsePos.y + (distanceY / 2), targetPos.z);
            Vector3 movePos2 = new Vector3(horsePos.x, targetPos.y, targetPos.z);
            movePoints = new Vector3[] { horsePos, movePos1, movePos2, targetPos };
        }
        return movePoints;
    }

    private IEnumerator MoveHorse(Vector3 nowPos, Vector3 nextPos)
    {
        float distanceX = nowPos.x - nextPos.x;
        if (distanceX != 0) // x軸方向への移動
        {
            var horse = GetComponent<RectTransform>();
            Sequence sequence = DOTween.Sequence()
                .Append(horse.DOLocalMoveY(horse.localPosition.y + (60 - buffer), 0.1f).SetEase(Ease.OutQuart))
                .Join(horse.DOLocalMoveX(nextPos.x, 0.2f).SetEase(Ease.Linear))
                .Append(horse.DOLocalMoveY(horse.localPosition.y, 0.1f).SetEase(Ease.OutQuart));
            yield return new WaitForSeconds(0.22f);
        }
        else // y軸方向への移動
        {
            var horse = GetComponent<RectTransform>();
            Sequence sequence = DOTween.Sequence()
                .Append(horse.DOScale(1.2f, 0.1f).SetEase(Ease.OutQuart))
                .Join(horse.DOLocalMoveY(nextPos.y, 0.2f).SetEase(Ease.Linear))
                .Append(horse.DOScale(1f, 0.1f).SetEase(Ease.OutQuart));
            yield return new WaitForSeconds(0.22f);
        }
    }
}