using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MatchThreeEngine
{
    public class UIManager : Singleton<UIManager>
    {
        public UI_GameplayManager UI_Gameplay;
        public UI_MainMenuManager UI_MainMenu;
        public UI_WinScreen UI_WinScreen;
        public UI_LoseScreen UI_LoseScreen;
        private void Start()
        {
            UI_WinScreen.Initialize();
            UI_LoseScreen.Initialize();
        }

        public void PlayGame()
        {
            UI_MainMenu.gameObject.SetActive(false);
            UI_Gameplay.gameObject.SetActive(true);
            LevelManager.Instance.PlayGame();
        }
    }

}
