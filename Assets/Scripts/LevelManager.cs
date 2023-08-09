using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MatchThreeEngine
{
    public class LevelManager : Singleton<LevelManager>
    {
        public LevelData[] levels;
        public int currentLevel;
        public void PlayGame()
        {
            UIManager.Instance.UI_Gameplay.Initialize(levels[currentLevel]);
            AudioManager.Instance.PlayBGM();
        }

        public void NextLevel()
        {
            currentLevel++;
            if(currentLevel >= levels.Length)
            {
                currentLevel = 0;
            }
            UIManager.Instance.UI_Gameplay.Reset();
            PlayGame();
        }

        public void PlayAgain()
        {
            UIManager.Instance.UI_Gameplay.Reset();
            PlayGame();
        }
    }
}

