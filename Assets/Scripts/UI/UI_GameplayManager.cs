using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace MatchThreeEngine
{
    public class UI_GameplayManager : MonoBehaviour
    {
        public Board gameplayBoard;
        public int gameScore;
        public int gameTurn;
        public Text gameTurnTxt;
        public LevelData levelData;
        public bool _1stStarUnlocked;
        public bool _2ndStarUnlocked;
        public bool _3rdStarUnlocked;
        public Sprite emtyStar;
        public Sprite fullStar;
        public Image[] Stars;
        public void Initialize(LevelData levelData)
        {
            gameScore = 0;
            this.levelData = levelData;
            gameTurn = levelData.totalTurns;
            UpdateMove();
            gameplayBoard.Initialize(this,levelData);
            gameplayBoard.OnAddScore += addScore;
            gameplayBoard.OnMove += UpdateMove;
        }

        private void addScore(int score) {
            gameScore += score;
            CheckUnlockStar();
        }

        private void UpdateMove() {
            gameTurnTxt.text = gameTurn.ToString();
        }

        public void Reset()
        {
            gameScore = 0;
            _1stStarUnlocked = _2ndStarUnlocked = _3rdStarUnlocked = false;
            foreach (var item in Stars)
            {
                item.sprite = emtyStar;
            }
            gameplayBoard.OnAddScore -= addScore;
            gameplayBoard.OnMove -= UpdateMove;
            gameplayBoard.Reset();
        }

        private void CheckUnlockStar()
        {
            if (gameScore >= levelData._1stStar)
            {
                _1stStarUnlocked = true;
                Stars[0].sprite = fullStar;
            }
            if (gameScore >= levelData._2ndStar)
            {
                _2ndStarUnlocked = true;
                Stars[1].sprite = fullStar;
            }
            if (gameScore >= levelData._3rdStar)
            {
                _3rdStarUnlocked = true;
                Stars[2].sprite = fullStar;
            }
        }

        public void CheckWinCondition()
        {
            if (levelData.gameMode is EGameMode.Normal)
            {
                
                if (gameTurn == 0)
                {
                    if (_3rdStarUnlocked)
                    {
                        UIManager.Instance.UI_WinScreen.Show(3);
                        return;
                    }
                    if (_2ndStarUnlocked)
                    {
                        UIManager.Instance.UI_WinScreen.Show(2);
                        return;
                    }
                    if (_1stStarUnlocked)
                    {
                        UIManager.Instance.UI_WinScreen.Show(1);
                        return;
                    }
                    else
                    {
                        UIManager.Instance.UI_LoseScreen.Show(0);
                        return;
                    }
                }
            }
        }
    }
}

