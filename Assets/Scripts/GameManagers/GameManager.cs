using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using System;
using UnityEngine.UI;
using DG.Tweening;

public class GameManager : MonoBehaviour
{
    public GameStateReactiveProperty CurrentState
            = new GameStateReactiveProperty(GameState.Initializing);

    public GameStateReactiveProperty PreviousState
            = new GameStateReactiveProperty(GameState.Player2);

    [SerializeField] GameStateUI stateUI;
    [SerializeField] BoardManager boardManager;
    [SerializeField] PlayerManager playerManager;


    void Start()
    {
        playerManager = GetComponent<PlayerManager>();
        if(!playerManager) gameObject.AddComponent<PlayerManager>();
        if(!boardManager) GameObject.FindObjectOfType<BoardManager>();

        CurrentState.Subscribe(state =>
            {
                //state.Red();
                OnStateChanged(state);
            });
    }

    void OnStateChanged(GameState nextState)
    {
        stateUI.ActivateStateUI(CurrentState.Value);

        switch (nextState)
        {
            case GameState.Initializing:
                StartCoroutine(InitializeCoroutine());
                break;
            case GameState.Ready:
                StartCoroutine(ReadyCoroutine());
                break;
            case GameState.Player1:
                StartCoroutine(StrategyTimeCoroutine());
                break;
            case GameState.Player2:
                StartCoroutine(StrategyTimeCoroutine());
                break;
            case GameState.Battle:
                StartCoroutine(Battle());
                break;
            case GameState.Result:
                StartCoroutine(Result());
                break;
            case GameState.Finished:
                Finished();
                break;
            default:
                break;
        }
    }

    private IEnumerator InitializeCoroutine()
    {
        yield return playerManager.InitializePlayer(boardManager);
        yield return boardManager.CreateBoard();

        // 画面がタップされるまで待つ
        yield return stateUI.NextStateButton.OnClickAsObservable().First().ToYieldInstruction();
        stateUI.DeactivateStateUI();

        CurrentState.Value = GameState.Ready;
    }

    // Player確認UI表示
    private IEnumerator ReadyCoroutine()
    {
        yield return new WaitForSeconds(1.0f);
        // 画面がタップされるまで待つ
        yield return stateUI.NextStateButton.OnClickAsObservable().First().ToYieldInstruction();
        stateUI.DeactivateStateUI();

        if (PreviousState.Value == GameState.Player2)
        {
            CurrentState.Value = GameState.Player1;
        }
        else if (PreviousState.Value == GameState.Player1)
        {
            CurrentState.Value = GameState.Player2;
        }
    }

    private IEnumerator StrategyTimeCoroutine()
    {
        yield return new WaitForSeconds(1.0f);
        stateUI.DeactivateStateUI();
        
        // 戦略タイム
        yield return playerManager.StartStrategy(CurrentState.Value);

        if (CurrentState.Value == GameState.Player1)
        {
            PreviousState.Value = GameState.Player1;
            CurrentState.Value = GameState.Ready;
        }
        else if (CurrentState.Value == GameState.Player2)
        {
            PreviousState.Value = GameState.Player2;
            CurrentState.Value = GameState.Battle;
        }
    }

    private IEnumerator Battle()
    {
        yield return new WaitForSeconds(2.0f);
        stateUI.DeactivateStateUI();
        // 移動を行う
        //yield return PlayerManager.

        // 攻撃を行う

        // 破壊を行う

        // 全て終わるまで待機
        yield return new WaitForSeconds(3.0f);

        CurrentState.Value = GameState.Result;
    }

    private IEnumerator Result()
    {
        yield return new WaitForSeconds(2.0f);
        stateUI.DeactivateStateUI();
        // 王様が生きているかチェック

        // 両方生きてたら、次はReady

        // 王様が死んでたら、次はFinished

        yield return new WaitForSeconds(3.0f);
        CurrentState.Value = GameState.Ready;
    }

    private void Finished()
    {
        // Resultシーンへ遷移
    }
}
