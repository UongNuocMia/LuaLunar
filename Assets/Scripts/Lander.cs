using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
public class Lander : MonoBehaviour
{
    public static Lander Instance { get; private set; }

    public event EventHandler OnUpForce;
    public event EventHandler OnLeftForce;
    public event EventHandler OnRightForce;
    public event EventHandler OnCoinPickup;
    public event EventHandler OnBeforeForce;

    public event EventHandler<OnLandedEventArgs> OnLanded;
    public class OnLandedEventArgs : EventArgs
    {
        public int score;
    }

    private Transform tf;
    private Rigidbody2D landerRigidbody2D;

    private int score;

    private float dotVector;
    private float relativeVelocityMagnitude;
    private float force = 700f;
    private float fuelAmount = 10f;
    private float addFuelAmount = 10f;
    private float turnSpeed = 100f;
    private float minDotVector = .90f;
    private float fuelConsumptionAmount = 1f;
    private float scoreDotVectorMultiplier = 10f;
    private float softLandingVelocityMagnitude = 4f;
    private float maxScoreAmountLandingAngle = 100f;
    private float maxScoreAmountLandingSpeed = 100f;
    private void Awake()
    {
        Instance = this;
        landerRigidbody2D = GetComponent<Rigidbody2D>();
        tf = transform;
    }

    private void FixedUpdate()
    {
        OnBeforeForce?.Invoke(this, EventArgs.Empty);

        if(fuelAmount <= 0f)
        {
            return;
        }

        if(Keyboard.current.upArrowKey.isPressed||
           Keyboard.current.leftArrowKey.isPressed||
           Keyboard.current.rightArrowKey.isPressed)
        {
            ConsumeFuel();
        }

        if (Keyboard.current.upArrowKey.isPressed)
        {
            landerRigidbody2D.AddForce(force * Time.deltaTime * tf.up);
            OnUpForce?.Invoke(this, EventArgs.Empty);
        }
        if (Keyboard.current.leftArrowKey.isPressed)
        {
            landerRigidbody2D.AddTorque(turnSpeed * Time.deltaTime);
            OnLeftForce?.Invoke(this, EventArgs.Empty);
        }
        if (Keyboard.current.rightArrowKey.isPressed)
        {
            landerRigidbody2D.AddTorque(-turnSpeed * Time.deltaTime);
            OnRightForce?.Invoke(this, EventArgs.Empty);
        }
    }
    private void OnCollisionEnter2D(Collision2D collision2D)
    {
        if(!collision2D.gameObject.TryGetComponent(out LandingPad landingPad))
        {
            Debug.Log("Crashed on the Terrain!");
            return;
        }
        relativeVelocityMagnitude = collision2D.relativeVelocity.magnitude;
        if (relativeVelocityMagnitude > softLandingVelocityMagnitude)
        {
            Debug.Log("Landed too hard!");
            return;
        }
        dotVector = Vector2.Dot(Vector2.up, transform.up);
        
        if (dotVector < minDotVector)
        {
            Debug.Log("Landed on a too steep angle!");
            return;
        }

        Debug.Log("Successful landing");
        float landingAngleScore = maxScoreAmountLandingAngle - Mathf.Abs(dotVector - 1f) * scoreDotVectorMultiplier * maxScoreAmountLandingAngle;

        float landingSpeedScore = (softLandingVelocityMagnitude - relativeVelocityMagnitude) * maxScoreAmountLandingSpeed;

        Debug.Log("Landing angle score: "+ landingAngleScore);
        Debug.Log("Landing speed score: "+ landingSpeedScore);

        score = Mathf.RoundToInt((landingAngleScore + landingSpeedScore) * landingPad.GetScoreMultiplier());

        Debug.Log("score: " + score);
        OnLanded?.Invoke(this, new OnLandedEventArgs
        {
            score = score,
        });
    }

    private void OnTriggerEnter2D(Collider2D collider2D)
    {
        if (collider2D.gameObject.TryGetComponent(out FuelPickup fuelPickup))
        {
            fuelAmount += addFuelAmount;
            fuelPickup.DestroySelf();
        }

        if (collider2D.gameObject.TryGetComponent(out CoinPickup coinPickup))
        {
            OnCoinPickup?.Invoke(this, EventArgs.Empty);
            coinPickup.DestroySelf();
        }
    }

    private void ConsumeFuel()
    {
        fuelAmount -= fuelConsumptionAmount * Time.deltaTime;
        if (fuelAmount < 0) fuelAmount = 0;
    }

}
