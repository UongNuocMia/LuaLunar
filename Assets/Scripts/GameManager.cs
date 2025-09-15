using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    private static int levelNumber = 1;

    [SerializeField] private List<GameLevel> levelList = new List<GameLevel>();

    private int score;
    private bool isTimerActive;
    private float time;
    private void Awake()
    {
        Instance = this;
    }
    private void Start()
    {
        Debug.Log("levelnumber____" + levelNumber);
        Lander.Instance.OnCoinPickup += OnCoinPickup;
        Lander.Instance.OnLanded += OnLanded;
        Lander.Instance.OnStateChange += OnStateChange;
        LoadCurrentLevel();
    }
    private void Update()
    {
        if (isTimerActive)
            time += Time.deltaTime;
    }

    private void LoadCurrentLevel()
    {
        foreach (GameLevel gameLevel in levelList)
        {
            if(gameLevel.GetLevelNumber() == levelNumber)
            {
                GameLevel spawnedGameLevel = Instantiate(gameLevel, Vector3.zero, Quaternion.identity);
                Lander.Instance.SetPosition(spawnedGameLevel.GetLanderStartPosition());
            }
        }
    }

    private void OnStateChange(object sender, Lander.OnStateChangedEventArgs e)
    {
        isTimerActive = e.state == State.Normal;
    }



    private void OnLanded(object sender, Lander.OnLandedEventArgs e)
    {
        AddScore(e.score);
    }

    private void OnCoinPickup(object sender, System.EventArgs e)
    {
        AddScore(500);
    }

    public void AddScore(int addScoreAmount)
    {
        score += addScoreAmount;
    }

    public int GetScore()
    {
        return score;
    }

    public float GetTime()
    {
        return time;
    }

    public float GetLevel()
    {
        return levelNumber;
    }

    public void GoToNextLevel()
    {
        levelNumber++;
        Debug.Log("levelnumber2____" + levelNumber);
        SceneManager.LoadScene(0);
    }

    public void RetryLevel()
    {
        SceneManager.LoadScene(0);
    }

}
