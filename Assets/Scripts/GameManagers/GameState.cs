using System;
using UniRx;
using UnityEngine;

public enum GameState
{
    Initializing,
    Ready,
    Player1,
    Player2,
    Battle,
    Result,
    Finished
}

public class GameStateReactiveProperty : ReactiveProperty<GameState>
{
    public GameStateReactiveProperty()
    {
    }

    public GameStateReactiveProperty(GameState initialValue)
        : base(initialValue)
    {
        Sound.LoadBgm("3", "3_battle");
        Sound.PlayBgm("3");
    }
}

