using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class StatUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI statsTextMesh;
    [SerializeField] private GameObject speedUpArrowGO;
    [SerializeField] private GameObject speedDownArrowGO;
    [SerializeField] private GameObject speedRightArrowGO;
    [SerializeField] private GameObject speedLeftArrowGO;
    [SerializeField] private Image fuelBarImage;
    private void Update()
    {
        UpdateStatsTextMesh();
    }

    private void UpdateStatsTextMesh()
    {
        speedUpArrowGO.SetActive(Lander.Instance.GetSpeedY() >= 0);
        speedDownArrowGO.SetActive(Lander.Instance.GetSpeedY() < 0);
        speedLeftArrowGO.SetActive(Lander.Instance.GetSpeedX() < 0);
        speedRightArrowGO.SetActive(Lander.Instance.GetSpeedX() >= 0);
        fuelBarImage.fillAmount = Lander.Instance.GetFuelAmountNormalized();
        statsTextMesh.text =
            GameManager.Instance.GetLevel() + "\n" +
            GameManager.Instance.GetScore() + "\n" +
            Mathf.Round(GameManager.Instance.GetTime()) + "\n" +
            Mathf.Abs(Mathf.Round(Lander.Instance.GetSpeedX() * 10f)) + "\n" +
            Mathf.Abs(Mathf.Round(Lander.Instance.GetSpeedY() * 10f)) + "\n";
    }
}
