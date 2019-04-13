using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using UniRx;

public class GameStateUI : MonoBehaviour
{
    private RectTransform rectTransform;
    private Tweener moveSequence;

    [SerializeField] private RectTransform startPosition;
    [SerializeField] private RectTransform endPosition;
    [SerializeField] private float moveTime = 0.6f;
    [SerializeField] private Sprite[] strategyTurnSprite; // ここ最終的に他に移動
    [SerializeField] private Sprite[] stateTurnSprite;
    [SerializeField] private Image strategyTurnImage; // ここ最終的に他に移動
    [SerializeField] private Image stateTurnImage;
    [SerializeField] private GameObject tapToStartImage;
    [SerializeField] private Button nextStateButton;
    public Button NextStateButton { get { return nextStateButton; } }

    
    

    public void Initialize(GameStateReactiveProperty gameState)
    {
        rectTransform = GetComponent<RectTransform>();
        ResetStateUIPosition();

        gameState.Subscribe(state =>
            {
                ActivateStateUI(state);
            });

        nextStateButton.OnClickAsObservable().Subscribe(_ =>
        {
            Sound.LoadSe("5", "5_start");
            Sound.PlaySe("5");
        });
    }

    private void ActivateStateUI(GameState state)
    {
        switch (state)
        {
            case GameState.Initializing:
                break;

            case GameState.Ready:
                break;

            case GameState.Player1:
                ChangeStateUI(0);
                StartCoroutine(AppearStateUI());
                strategyTurnImage.sprite = strategyTurnSprite[0]; // ここ最終的に他に移動
                break;

            case GameState.Player2:
                ChangeStateUI(1);
                StartCoroutine(AppearStateUI());
                strategyTurnImage.sprite = strategyTurnSprite[1]; // ここ最終的に他に移動
                break;

            case GameState.Battle:
                ChangeStateUI(2);
                StartCoroutine(AppearStateUI());
                break;

            case GameState.Result:
                break;
        }
    }

    // stateNum => 0:1P,1:2P,3:Battle
    private void ChangeStateUI(int stateNum)
    {
        stateTurnImage.sprite = stateTurnSprite[stateNum];
        stateTurnImage.gameObject.SetActive(true);
        tapToStartImage.SetActive(true);
        gameObject.SetActive(true);
    }

    public IEnumerator AppearStateUI()
    {
        moveSequence = rectTransform.DOLocalMoveX(0, moveTime);
        yield return moveSequence.WaitForCompletion();
        nextStateButton.interactable = true;
    }

    public IEnumerator DisappearStateUI()
    {
        moveSequence = rectTransform.DOLocalMoveX(endPosition.localPosition.x, moveTime);
        yield return moveSequence.WaitForCompletion();
        ResetStateUIPosition();
    }

    private void ResetStateUIPosition()
    {
        rectTransform.localPosition = startPosition.localPosition;
        gameObject.SetActive(false);
        tapToStartImage.SetActive(false);
        nextStateButton.interactable = false;
    }
}
