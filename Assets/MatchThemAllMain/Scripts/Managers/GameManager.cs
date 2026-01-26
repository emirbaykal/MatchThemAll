using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using MatchThemAll.Scripts;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace MatchThemAllMain.Scripts.Managers
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager instance;
        
        private EGameState gameState;

        private void Awake()
        {
            if (instance == null)
                instance = this;
            else
                Destroy(gameObject);
        }

        private void Start()
        {
            SetGameState(EGameState.MENU);
        }

        public void SetGameState(EGameState gameState)
        {
            this.gameState = gameState;

            IEnumerable<IGameStateListener> gameStateListener
                = FindObjectsByType<MonoBehaviour>(FindObjectsSortMode.None)
                    .OfType<IGameStateListener>();

            foreach (IGameStateListener dependency in gameStateListener)
                dependency.GameStateChangedCallback(gameState);
        }

        public void StartGame()
        {
            SetGameState(EGameState.GAME);
        }

        public void NextButtonCallback()
        {
            SceneManager.LoadScene(0);
        }

        public void RetryButtonCallback()
        {
            SceneManager.LoadScene(0);
        }

        public bool IsGame() => gameState == EGameState.GAME;
    }
}