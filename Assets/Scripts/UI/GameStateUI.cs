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

    public Image turnImage2;
    public Image turnImage3;

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
                turnImage.gameObject.SetActive(false);
                turnImage3.gameObject.SetActive(false);
                break;
            case GameState.Ready:
                turnImage.gameObject.SetActive(false);
                turnImage3.gameObject.SetActive(true);
                break;
            case GameState.Player1:
                turnImage.sprite = TurnSprite[0];
                turnImage2.sprite = TurnSprite[3];
                turnImage.gameObject.SetActive(true);
                break;
            case GameState.Player2:
                turnImage.sprite = TurnSprite[1];
                turnImage2.sprite = TurnSprite[4];
                turnImage.gameObject.SetActive(true);
                break;
            case GameState.Battle:
                turnImage.sprite = TurnSprite[2];
                turnImage.gameObject.SetActive(true);
                break;
            case GameState.Result:
                turnImage.gameObject.SetActive(false);
                turnImage3.gameObject.SetActive(false);
                break;
        }
    }
    public void DeactivateStateUI()
    {
        //この辺適当にアニメーション
        // rectTransform.DOLocalMoveX(StateUI.localPosition.x + 100f,1f);
        rectTransform.gameObject.SetActive(false);
    }
}
