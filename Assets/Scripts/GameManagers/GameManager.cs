using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using System;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public GameStateReactiveProperty CurrentState
            = new GameStateReactiveProperty(GameState.Initializing);

    public GameStateReactiveProperty PreviousState
            = new GameStateReactiveProperty(GameState.Player2);

	public GameObject StateUI;
	public Button NextStateButton;

    void Start()
    {
        CurrentState.Subscribe(state =>
            {
                //state.Red();
                OnStateChanged(state);
            });
    }

    /// <summary>
    /// ステートが変移した
    /// </summary>
    void OnStateChanged(GameState nextState)
    {
        switch (nextState)
        {
            case GameState.Initializing:
                StartCoroutine(InitializeCoroutine());
                break;
            case GameState.Ready:
                StartCoroutine(ReadyCoroutine());
                break;
            case GameState.Player1:
                StartCoroutine(StrategyTimeCoroutine(GameState.Player1));
                PreviousState.Value = GameState.Player1;
                break;
            case GameState.Player2:
                StartCoroutine(StrategyTimeCoroutine(GameState.Player2));
                PreviousState.Value = GameState.Player2;
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
        // ルール説明画像表示

        //

        // 画面がタップされるまで待つ
        yield return NextStateButton.OnClickAsObservable().First().ToYieldInstruction();

        CurrentState.Value = GameState.Ready;
    }

    // Player確認UI表示
    private IEnumerator ReadyCoroutine()
    {
        // PreviousStateに応じて、"Player ◯ のターン"と表示

        // 画面がタップされるまで待つ
        yield return null;

        CurrentState.Value = GameState.Player1;
    }

    private IEnumerator StrategyTimeCoroutine(GameState gameState)
    {
        // タイマー作動

        // コストを管理する

        // Stateに応じて、そのPlayerの駒をタッチ可能にする

        // 決定ボタンが押される or 制限時間が過ぎるまで待つ
        yield return null;

        if (gameState == GameState.Player1)
        {
            CurrentState.Value = GameState.Ready;
        }
        else if (gameState == GameState.Player2)
        {
            CurrentState.Value = GameState.Battle;
        }
    }

    private IEnumerator Battle()
    {
        // 移動を行う

        // 攻撃を行う

        // 破壊を行う

        // 全て終わるまで待機
        yield return null;

        CurrentState.Value = GameState.Result;
    }

    private IEnumerator Result()
    {
        // 王様が生きているかチェック

        // 両方生きてたら、次はReady

		// 王様が死んでたら、次はFinished

        yield return null;
    }

    private void Finished()
    {
        // Resultシーンへ遷移
    }
}
