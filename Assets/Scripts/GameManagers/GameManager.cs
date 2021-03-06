﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using System;
using UnityEngine.UI;
using DG.Tweening;

public class GameManager : MonoBehaviour
{
    private GameStateReactiveProperty CurrentState = new GameStateReactiveProperty(GameState.Initializing);
    private GameStateReactiveProperty PreviousState = new GameStateReactiveProperty(GameState.Player2);

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
        if (!stateUI) GameObject.FindObjectOfType<GameStateUI>();

        SatrtBGM();
        Initialize();
    }

    private void SatrtBGM()
    {
        Sound.StopBgm();
        Sound.LoadBgm("2", "2_senryak");
        Sound.PlayBgm("2");
    }

    private void Initialize()
    {
        stateUI.Initialize(CurrentState);

        CurrentState.Subscribe(state =>
            {
                OnStateChanged(state);
            });
    }

    void OnStateChanged(GameState nextState)
    {
        switch (nextState)
        {
            case GameState.Initializing:
                StartCoroutine(InitializeCoroutine());
                break;
            case GameState.Ready:
                PlayerChangeState();
                break;
            case GameState.Player1:
                StartCoroutine(StrategyTimeState());
                break;
            case GameState.Player2:
                StartCoroutine(StrategyTimeState());
                break;
            case GameState.Battle:
                StartCoroutine(BattleState());
                break;
            case GameState.Result:
                ButtleResultState();
                break;
            case GameState.Finished:
                GameFinishState();
                break;
            default:
                break;
        }
    }

    private IEnumerator InitializeCoroutine()
    {
        boardManager.CreateBoard();
        yield return null;
        yield return playerManager.InitializePlayer(boardManager);
        yield return null;
        CurrentState.Value = GameState.Ready;
    }

    // Player確認UI表示
    private void PlayerChangeState()
    {
        if (PreviousState.Value == GameState.Player2)
        {
            CurrentState.Value = GameState.Player1;
        }
        else if (PreviousState.Value == GameState.Player1)
        {
            CurrentState.Value = GameState.Player2;
        }
    }

    private IEnumerator StrategyTimeState()
    {
        yield return stateUI.NextStateButton.OnClickAsObservable().First().ToYieldInstruction();
        
        Sound.LoadSe("13", "13_junbi");
        Sound.PlaySe("13");
        yield return stateUI.DisappearStateUI();

        // 戦略タイムが終わるまで待つ
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

    private IEnumerator BattleState()
    {
        yield return stateUI.NextStateButton.OnClickAsObservable().First().ToYieldInstruction();
        
        Sound.LoadSe("7", "7_koutai");
        Sound.PlaySe("7");
        yield return stateUI.DisappearStateUI();

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

    private void ButtleResultState()
    {
        // 王様が生きているかチェック
        result = playerManager.GetResult();
        if (result == 0)
        {
            CurrentState.Value = GameState.Ready;
        }
        else
        {
            CurrentState.Value = GameState.Finished;
        }
    }

    private void GameFinishState()
    {
        resultManager.ShowResult(result);
    }
}
