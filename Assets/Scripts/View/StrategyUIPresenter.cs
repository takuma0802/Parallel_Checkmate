using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UniRx;
using DG.Tweening;

public class StrategyUIPresenter : MonoBehaviour
{
    public Button TurnEndButton;
    public Button UndoButton;

	// 移動アニメーション周り
	private Tweener moveSequence = null;
	[SerializeField] private RectTransform lowerArea;
    [SerializeField] private RectTransform visiblePosition;
    [SerializeField] private RectTransform invisiblePosition;
    [SerializeField] private float moveTime = 0.3f;
	[SerializeField] private Ease appearEase;
	[SerializeField] private Ease hideEase;

	void Start()
	{
		ResetPosition();
	}

	public IEnumerator AppearLowerArea()
	{
		moveSequence = lowerArea.DOLocalMoveY(visiblePosition.localPosition.y, moveTime).SetEase(appearEase);
		yield return moveSequence.WaitForCompletion();
	}

	public IEnumerator HideLowerArea()
	{
		moveSequence = lowerArea.DOLocalMoveY(invisiblePosition.localPosition.y, moveTime).SetEase(hideEase);
        yield return moveSequence.WaitForCompletion();
	}

	private void ResetPosition()
    {
        lowerArea.localPosition = invisiblePosition.localPosition;
    }
}
