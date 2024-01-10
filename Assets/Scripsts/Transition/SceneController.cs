using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.AI;

public class SceneController : Singleton<SceneController>, IEndGameObserver
{
    public GameObject playerPrefab;
    public SceneFader sceneFaderPrefab;
    GameObject player;
    bool fadeFinished;

    private void Start()
    {
        GameManager.Instance.AddObserver(this);
        // sceneFaderPrefab = Instantiate(sceneFaderPrefab);
        fadeFinished = true;
    }

    public void TransitionToDestination(TransitionPoint transitionPoint)
    {
        switch (transitionPoint.transitionType)
        {
            case TransitionPoint.TransitionType.LobbyScene:
                SaveManager.Instance.SavePlayerData();
                TransitionToMain();
                break;
            case TransitionPoint.TransitionType.SameScene:
                StartCoroutine(Transition(SceneManager.GetActiveScene().name, transitionPoint.destinationTag));
                break;
            case TransitionPoint.TransitionType.DifferentScene:
                StartCoroutine(Transition(transitionPoint.sceneName, transitionPoint.destinationTag));
                break;
        }
    }

    private IEnumerator Transition(string sceneName, TransitionDestonation.DestinationTag destinationTag)
    {
        //TODO:保存數據
        SaveManager.Instance.SavePlayerData();

        if (SceneManager.GetActiveScene().name != sceneName)
        {
            yield return SceneManager.LoadSceneAsync(sceneName);
            yield return Instantiate(playerPrefab, GetDestonation(destinationTag).transform.position, GetDestonation(destinationTag).transform.rotation);
            //保存數據
            SaveManager.Instance.LoadPlayerData();
            yield break;
        }
        else
        {
            player = GameManager.Instance.playerStats.gameObject;
            player.GetComponent<NavMeshAgent>().enabled = false;
            player.transform.SetPositionAndRotation(GetDestonation(destinationTag).transform.position, GetDestonation(destinationTag).transform.rotation);
            player.GetComponent<NavMeshAgent>().enabled = true;
            yield return null;
        }
    }

    private TransitionDestonation GetDestonation(TransitionDestonation.DestinationTag destinationTag)
    {
        var entrances = FindObjectsOfType<TransitionDestonation>();

        for (int i = 0; i < entrances.Length; i++)
        {
            if (entrances[i].destinationTag == destinationTag)
            {
                return entrances[i];
            }
        }
        return null;
    }

    public void TransitionToMain()
    {
        StartCoroutine(LoadMain());
    }
    public void TransitionToLoadGame()
    {
        StartCoroutine(LoadLevel(SaveManager.Instance.SceneName));
    }

    public void TransitionToFristLevel()
    {
        StartCoroutine(LoadLevel("Game"));
    }

    private IEnumerator LoadLevel(string scene)
    {
        if (scene != "")
        {
            yield return StartCoroutine(sceneFaderPrefab.FadeOut(2f));
            yield return SceneManager.LoadSceneAsync(scene);
            yield return player = Instantiate(playerPrefab, GameManager.Instance.GetEntrance().position, GameManager.Instance.GetEntrance().rotation);

            SaveManager.Instance.SavePlayerData();
            yield return StartCoroutine(sceneFaderPrefab.FadeIn(2f));
            yield break;
        }
    }

    private IEnumerator LoadMain()
    {
        yield return StartCoroutine(sceneFaderPrefab.FadeOut(2f));
        yield return SceneManager.LoadSceneAsync("Main");
        yield return StartCoroutine(sceneFaderPrefab.FadeIn(2f));
        yield break;
    }

    public void EndNotify()
    {
        if (fadeFinished)
        {
            fadeFinished = false;
            StartCoroutine(LoadMain());
        }
    }
}
