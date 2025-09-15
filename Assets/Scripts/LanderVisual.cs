using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LanderVisual : MonoBehaviour
{
    [SerializeField] private GameObject landerExplosionVfx;

    [SerializeField] private ParticleSystem leftThrusterParticleSystem;
    [SerializeField] private ParticleSystem middleThrusterParticleSystem;
    [SerializeField] private ParticleSystem rightThrusterParticleSystem;


    private Lander lander;

    private void Awake()
    {
        lander = GetComponent<Lander>();

        lander.OnRightForce += Lander_OnRightForce;
        lander.OnLeftForce += Lander_OnLeftForce;
        lander.OnUpForce += Lander_OnUpForce;
        lander.OnBeforeForce += Lander_OnBeforeForce;
        lander.OnLanded += Lander_OnLanded; ;

        SetEnabledThrusterParticleSystem(leftThrusterParticleSystem, false);
        SetEnabledThrusterParticleSystem(middleThrusterParticleSystem, false);
        SetEnabledThrusterParticleSystem(rightThrusterParticleSystem, false);
    }

    private void Lander_OnLanded(object sender, Lander.OnLandedEventArgs e)
    {
        switch (e.landingType)
        {
            case LandingType.WrongLandingArea:
            case LandingType.TooSteepAngle:
            case LandingType.TooFastLanding:
                Instantiate(landerExplosionVfx, transform.position, Quaternion.identity);
                gameObject.SetActive(false);
                break;
        }
    }

    private void Lander_OnBeforeForce(object sender, System.EventArgs e)
    {
        SetEnabledThrusterParticleSystem(leftThrusterParticleSystem, false);
        SetEnabledThrusterParticleSystem(middleThrusterParticleSystem, false);
        SetEnabledThrusterParticleSystem(rightThrusterParticleSystem, false);
    }

    private void Lander_OnUpForce(object sender, System.EventArgs e)
    {
        SetEnabledThrusterParticleSystem(leftThrusterParticleSystem, true);
        SetEnabledThrusterParticleSystem(middleThrusterParticleSystem, true);
        SetEnabledThrusterParticleSystem(rightThrusterParticleSystem, true);
    }

    private void Lander_OnLeftForce(object sender, System.EventArgs e)
    {
        SetEnabledThrusterParticleSystem(leftThrusterParticleSystem, true);
    }

    private void Lander_OnRightForce(object sender, System.EventArgs e)
    {
        SetEnabledThrusterParticleSystem(rightThrusterParticleSystem, true);
    }

    private void SetEnabledThrusterParticleSystem(ParticleSystem particleSystem, bool enabled)
    {
        ParticleSystem.EmissionModule emissionModule = particleSystem.emission;
        emissionModule.enabled = enabled;
    }
}


