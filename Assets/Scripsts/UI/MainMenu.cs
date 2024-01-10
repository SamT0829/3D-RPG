using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Playables;

public class MainMenu : MonoBehaviour
{
    Button newGameBtn, continueBtn, quitBtn;
    PlayableDirector director;

    private void Awake()
    {
        newGameBtn = transform.GetChild(1).GetComponent<Button>();
        continueBtn = transform.GetChild(2).GetComponent<Button>();
        quitBtn = transform.GetChild(3).GetComponent<Button>();

        newGameBtn.onClick.AddListener(PlayTimeline);
        continueBtn.onClick.AddListener(ContinueGame);
        quitBtn.onClick.AddListener(QuitGame);

        director = FindObjectOfType<PlayableDirector>();
        director.stopped += NewGame;
    }

    private void PlayTimeline()
    {
        director.Play();
    }

    private void NewGame(PlayableDirector director)
    {
        PlayerPrefs.DeleteAll();
        //轉換場景
        SceneController.Instance.TransitionToFristLevel();
    }

    private void ContinueGame()
    {
        SceneController.Instance.TransitionToLoadGame();
    }

    private void QuitGame()
    {
        Application.Quit();
    }
}
