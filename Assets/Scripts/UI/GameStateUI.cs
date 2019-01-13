using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
public class GameStateUI : MonoBehaviour
{

    private RectTransform rectTransform;
    [SerializeField] private Button nextStateButton;
    [SerializeField] private Sprite[] strategyTurnSprite;
    [SerializeField] private Sprite[] stateTurnSprite;

    [SerializeField] private Image strategyTurnImage;
    [SerializeField] private Image stateTurnImage;
    [SerializeField] private GameObject tapToStartImage;
    

    public Button NextStateButton { get { return nextStateButton; } }

    void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        //startPosition = rectTransform.localPosition;
    }

    public void ActivateStateUI(GameState state)
    {
        gameObject.SetActive(true);
        switch (state)
        {
            case GameState.Initializing:
                stateTurnImage.gameObject.SetActive(false);
                break;
            case GameState.Ready:
                stateTurnImage.gameObject.SetActive(false);
                break;
            case GameState.Player1:
                tapToStartImage.SetActive(true);
                stateTurnImage.sprite = stateTurnSprite[0];
                stateTurnImage.gameObject.SetActive(true);
                strategyTurnImage.sprite = strategyTurnSprite[0];
                break;
            case GameState.Player2:
                stateTurnImage.sprite = stateTurnSprite[1];
                stateTurnImage.gameObject.SetActive(true);
                strategyTurnImage.sprite = strategyTurnSprite[1];
                tapToStartImage.SetActive(true);
                break;
            case GameState.Battle:
                stateTurnImage.sprite = stateTurnSprite[2];
                stateTurnImage.gameObject.SetActive(true);
                break;
            case GameState.Result:
                stateTurnImage.gameObject.SetActive(false);
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
