using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LanderUI : MonoBehaviour
{
    [SerializeField] private Button nextButton;
    [SerializeField] private Text buttonText;
    [SerializeField] private TextMeshProUGUI titleTextMesh;
    [SerializeField] private TextMeshProUGUI statsTextMesh;

    private Action nextButtonAction;
    private void Start()
    {
        Lander.Instance.OnLanded += Lander_OnLanded;
        ShowLanderUI(false);
        nextButton.onClick.AddListener(OnNextButtonClick);
    }

    private void OnNextButtonClick()
    {
        nextButtonAction?.Invoke();
    }

    private void Lander_OnLanded(object sender, Lander.OnLandedEventArgs e)
    {
        if (e.landingType == LandingType.Success)
        {
            titleTextMesh.text = "SUCCESSFUL LANDING!";
            nextButtonAction = GameManager.Instance.GoToNextLevel;
            buttonText.text = "CONTINUE";
        }
        else
        {
            titleTextMesh.text = "<color=#ff0000>CRASH!</color>";
            nextButtonAction = GameManager.Instance.RetryLevel;
            buttonText.text = "RESTART";
        }
        statsTextMesh.text =
            Mathf.Round(e.landingSpeed * 2f) + "\n" +
            Mathf.Round(e.dotVector * 100f) + "\n" +
            "x" + e.scoreMultiplier + "\n" +
            e.score;
        ShowLanderUI(true);
    }

    private void ShowLanderUI(bool isShow) => gameObject.SetActive(isShow);
}
