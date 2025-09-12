using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class LandingPadVisual : MonoBehaviour
{
    [SerializeField] private TextMeshPro scoreMultiplierTextMesh;


    private void Awake()
    {
        LandingPad landingPad = GetComponent<LandingPad>();
        scoreMultiplierTextMesh.text = "x" + landingPad.GetScoreMultiplier();

    }
    private void Start()
    {
    }
}
