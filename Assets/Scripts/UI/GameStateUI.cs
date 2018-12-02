using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
public class GameStateUI : MonoBehaviour
{

    [SerializeField] RectTransform rectTransform;
    [SerializeField] Text stateText;
    [SerializeField] Button nextStateButton;

    public Button NextStateButton { get { return nextStateButton; } }

    // Use this for initialization
    void Start()
    {

    }

    public void ActivateStateUI(GameState state)
    {
        gameObject.SetActive(true);
        switch (state)
        {
            case GameState.Initializing:
                break;
        }
        stateText.text = state.ToString();

        //この辺適当にアニメーション
        // rectTransform.DOLocalMoveX(StateUI.localPosition.x + 100f,1f);
    }
    public void DeactivateStateUI()
    {
        //この辺適当にアニメーション
        // rectTransform.DOLocalMoveX(StateUI.localPosition.x + 100f,1f);
        rectTransform.gameObject.SetActive(false);
    }
}
