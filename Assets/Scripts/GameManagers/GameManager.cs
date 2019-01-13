using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using System;
using UnityEngine.UI;
using DG.Tweening;

public class GameManager : MonoBehaviour
{
    [SerializeField] private GameStateReactiveProperty CurrentState = new GameStateReactiveProperty(GameState.Initializing);
    [SerializeField] private GameStateReactiveProperty PreviousState = new GameStateReactiveProperty(GameState.Player2);

    [SerializeField] GameStateUI stateUI;
    [SerializeField] BoardManager boardManager;
    [SerializeField] PlayerManager playerManager;
    [SerializeField] ResultManager resultManager;
    private int result;


    void Start()
    {
        playerManager = GetComponent<PlayerManager>();
        if (!playerManager) gameObject.AddComponent<PlayerManager>();
        if (!boardManager) GameObject.FindObjectOfType<BoardManager>();
        if (!resultManager) GameObject.FindObjectOfType<ResultManager>();

        StartStream();
    }

    private void StartStream()
    {
        stateUI.NextStateButton.OnClickAsObservable().Subscribe(_ =>
        {
            Sound.LoadSe("5", "5_start");
            Sound.PlaySe("5");
        });

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
                Ready();
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
                Result();
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
        Debug.Log(CurrentState.Value);
        yield return boardManager.CreateBoard();
        yield return playerManager.InitializePlayer(boardManager);

        // 画面がタップされるまで待つ
        // yield return stateUI.NextStateButton.OnClickAsObservable().First().ToYieldInstruction();
        stateUI.DeactivateStateUI();

        CurrentState.Value = GameState.Ready;
    }

    // Player確認UI表示
    private void Ready()
    {
        Debug.Log(CurrentState.Value);
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
        Debug.Log(CurrentState.Value);
        yield return stateUI.NextStateButton.OnClickAsObservable().First().ToYieldInstruction();
        
        Sound.LoadSe("13", "13_junbi");
        Sound.PlaySe("13");
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
        Debug.Log(CurrentState.Value);
        yield return stateUI.NextStateButton.OnClickAsObservable().First().ToYieldInstruction();
        Sound.LoadSe("7", "7_koutai");
        Sound.PlaySe("7");
        stateUI.DeactivateStateUI();
        
        // 移動
        yield return playerManager.StartMove();

        // 移動が重なることで破壊されるPieceを確認
        yield return playerManager.ExcecuteMoveDestroy();

        // 攻撃
        yield return playerManager.StartBattle();

        // 破壊
        yield return playerManager.ExcecuteAttackDestroy();

        CurrentState.Value = GameState.Result;
    }

    private void Result()
    {
        Debug.Log(CurrentState.Value);
        stateUI.DeactivateStateUI();
        // 王様が生きているかチェック
        result = playerManager.GetResult();
        if(result == 0)
        {
            CurrentState.Value = GameState.Ready;
        }
        else
        {
            CurrentState.Value = GameState.Finished;
        }
    }

    private void Finished()
    {
        Debug.Log(CurrentState.Value);
        resultManager.ShowResultUI(result);
    }
}
