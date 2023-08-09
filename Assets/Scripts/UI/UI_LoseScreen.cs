using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace MatchThreeEngine
{
    public class UI_LoseScreen : MonoBehaviour
    {
        public GameObject[] Star;
        public Button closeBtn;
        public Button retryBtn;
        public Text scoreTxt;
        public Text levelTxt;

        public void Initialize()
        {
            scoreTxt.text = "0";
            scoreTxt.text = "Level " + LevelManager.Instance.currentLevel;
            closeBtn.onClick.AddListener(Hide);
            retryBtn.onClick.AddListener(() =>
            {
                Hide();
                LevelManager.Instance.PlayAgain();  
            });
        }
        public void Show(int numberOfStar)
        {
            HideStars();
            scoreTxt.text = UIManager.Instance.UI_Gameplay.gameScore.ToString();
            levelTxt.text = "Level " + LevelManager.Instance.currentLevel;
            this.gameObject.SetActive(true);
            for (int i = 0; i < numberOfStar; i++)
            {
                Star[i].SetActive(true);
            }
        }

        public void Hide()
        {
            HideStars();
            this.gameObject.SetActive(false);     
        }
       private void HideStars()
        {
            for (int i = 0; i < Star.Length; i++)
            {
                Star[i].SetActive(false);
            }
        }

    }
}