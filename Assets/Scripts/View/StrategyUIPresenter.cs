using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UniRx;
using DG.Tweening;

public class StrategyUIPresenter : MonoBehaviour
{
    public Button turnEndButton;
    public Button undoButton;

	// 移動アニメーション周り
	private RectTransform rectTransform;
	private Tweener moveSequence = null;
	[SerializeField] private RectTransform lowerArea;
    [SerializeField] private RectTransform visiblePosition;
    [SerializeField] private RectTransform invisiblePosition;
    [SerializeField] private float moveTime = 0.4f;

	void Start()
	{
		rectTransform = GetComponent<RectTransform>();
		ResetPosition();
	}

	public IEnumerator AppearLowerArea()
	{
		moveSequence = lowerArea.DOLocalMoveY(visiblePosition.localPosition.y, moveTime);
		yield return moveSequence.WaitForCompletion();
	}

	public IEnumerator HideLowerArea()
	{
		moveSequence = lowerArea.DOLocalMoveY(invisiblePosition.localPosition.y, moveTime);
        yield return moveSequence.WaitForCompletion();
	}

	private void ResetPosition()
    {
        lowerArea.localPosition = invisiblePosition.localPosition;
    }
}
