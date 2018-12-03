using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
public class GameStateUI : MonoBehaviour
{

    [SerializeField] RectTransform rectTransform;
    [SerializeField] Button nextStateButton;
    [SerializeField] Image turnImage;
    public Sprite[] TurnSprite;
    private Vector2 startPosition;

    public Button NextStateButton { get { return nextStateButton; } }

    void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        startPosition = rectTransform.localPosition;
    }

    public void ActivateStateUI(GameState state)
    {
        gameObject.SetActive(true);
        switch (state)
        {
            case GameState.Initializing:
                turnImage.sprite = null;
                break;
            case GameState.Ready:
                turnImage.sprite = null;
                break;
            case GameState.Player1:
                turnImage.sprite = TurnSprite[0];
                break;
            case GameState.Player2:
                turnImage.sprite = TurnSprite[1];
                break;
            case GameState.Battle:
                turnImage.sprite = TurnSprite[2];
                break;
            case GameState.Result:
                turnImage.sprite = null;
                break;
        }
        //この辺適当にアニメーション
        // rectTransform.DOLocalMoveX(this.transform.localPosition.x + 100f,1f);
    }
    public void DeactivateStateUI()
    {
        //この辺適当にアニメーション
        // rectTransform.DOLocalMoveX(StateUI.localPosition.x + 100f,1f);
        rectTransform.gameObject.SetActive(false);
    }
}
