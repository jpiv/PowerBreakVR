using UnityEngine;
using System.Collections;

public static class GameLifecycle {
    public delegate void StartGameAction();
    public static event StartGameAction OnStartGame;
    public delegate void ResetAction();
    public static event ResetAction OnReset;
    public delegate void GameOverAction();
    public static event GameOverAction OnGameOver;

    public static void StartGame() {
      GameLifecycle.OnStartGame();
    }

    public static void Reset() {
      GameLifecycle.OnReset();
    }

    public static void GameOver() {
      GameLifecycle.OnGameOver();
    }
}