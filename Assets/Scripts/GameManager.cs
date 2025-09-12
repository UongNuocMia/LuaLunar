using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private int score;


    private void Start()
    {
        Lander.Instance.OnCoinPickup += OnCoinPickup;
        Lander.Instance.OnLanded += OnLanded;
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
        Debug.Log("SCORE "+score);
    }
}
