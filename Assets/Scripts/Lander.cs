using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;
public class Lander : MonoBehaviour
{
    public const float GRAVITY_NORMAL = 0.7f;
    public static Lander Instance { get; private set; }

    public event EventHandler OnUpForce;
    public event EventHandler OnLeftForce;
    public event EventHandler OnRightForce;
    public event EventHandler OnCoinPickup;
    public event EventHandler OnBeforeForce;
    
    public event EventHandler<OnStateChangedEventArgs> OnStateChange;
    
    public class OnStateChangedEventArgs : EventArgs
    {
        public State state;
    }

    public event EventHandler<OnLandedEventArgs> OnLanded;
    public class OnLandedEventArgs : EventArgs
    {
        public LandingType landingType;
        public int score;
        public float dotVector;
        public float landingSpeed;
        public float scoreMultiplier;
    }

    private State state;

    private Transform tf;
    private Rigidbody2D landerRigidbody2D;

    private int score;

    private float dotVector;
    private float relativeVelocityMagnitude;
    private float force = 700f;
    private float fuelAmount;
    private float fuelAmountMax = 10f;
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
        landerRigidbody2D.gravityScale = 0;
        tf = transform;
        fuelAmount = fuelAmountMax;
        state = State.WaitingToStart;
    }

    private void FixedUpdate()
    {
        OnBeforeForce?.Invoke(this, EventArgs.Empty);

        switch (state)
        {
            case State.None:
                break;
            case State.WaitingToStart:
                if (Keyboard.current.upArrowKey.isPressed ||
                    Keyboard.current.leftArrowKey.isPressed ||
                    Keyboard.current.rightArrowKey.isPressed)
                {
                    landerRigidbody2D.gravityScale = GRAVITY_NORMAL;
                    SetState(State.Normal);
                }
                break;
            case State.Normal:
                if (fuelAmount <= 0f)
                {
                    return;
                }

                if (Keyboard.current.upArrowKey.isPressed ||
                   Keyboard.current.leftArrowKey.isPressed ||
                   Keyboard.current.rightArrowKey.isPressed)
                {
                    ConsumeFuel();
                    landerRigidbody2D.gravityScale = GRAVITY_NORMAL;
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
                break;
            case State.GameOver:
                
                break;
            default:
                break;
        }
    }
    private void OnCollisionEnter2D(Collision2D collision2D)
    {
        if (!collision2D.gameObject.TryGetComponent(out LandingPad landingPad))
        {
            Debug.Log("Crashed on the Terrain!");
            OnLanded?.Invoke(this, new OnLandedEventArgs
            {
                score = 0,
                dotVector = 0,
                landingType = LandingType.WrongLandingArea,
                landingSpeed = 0,
                scoreMultiplier = 0,
            });
            SetState(State.GameOver);
            return;
        }
        relativeVelocityMagnitude = collision2D.relativeVelocity.magnitude;
        if (relativeVelocityMagnitude > softLandingVelocityMagnitude)
        {
            Debug.Log("Landed too hard!");
            OnLanded?.Invoke(this, new OnLandedEventArgs
            {
                score = 0,
                dotVector = 0,
                landingType = LandingType.TooFastLanding,
                landingSpeed = relativeVelocityMagnitude,
                scoreMultiplier = 0,
            });
            SetState(State.GameOver);
            return;
        }
        dotVector = Vector2.Dot(Vector2.up, transform.up);

        if (dotVector < minDotVector)
        {
            Debug.Log("Landed on a too steep angle!");
            OnLanded?.Invoke(this, new OnLandedEventArgs
            {
                score = 0,
                dotVector = dotVector,
                landingType = LandingType.TooSteepAngle,
                landingSpeed = relativeVelocityMagnitude,
                scoreMultiplier = 0,
            });
            SetState(State.GameOver);
            return;
        }

        Debug.Log("Successful landing");
        float landingAngleScore = maxScoreAmountLandingAngle - Mathf.Abs(dotVector - 1f) * scoreDotVectorMultiplier * maxScoreAmountLandingAngle;

        float landingSpeedScore = (softLandingVelocityMagnitude - relativeVelocityMagnitude) * maxScoreAmountLandingSpeed;

        Debug.Log("Landing angle score: " + landingAngleScore);
        Debug.Log("Landing speed score: " + landingSpeedScore);

        score = Mathf.RoundToInt((landingAngleScore + landingSpeedScore) * landingPad.GetScoreMultiplier());

        Debug.Log("score: " + score);
        OnLanded?.Invoke(this, new OnLandedEventArgs
        {
            score = score,
            dotVector = dotVector,
            landingType = LandingType.Success,
            landingSpeed = relativeVelocityMagnitude,
            scoreMultiplier = landingPad.GetScoreMultiplier(),
        });
        SetState(State.GameOver);
    }

    private void OnTriggerEnter2D(Collider2D collider2D)
    {
        if (collider2D.gameObject.TryGetComponent(out FuelPickup fuelPickup))
        {
            fuelAmount += addFuelAmount;
            if(fuelAmount > fuelAmountMax)
                fuelAmount = fuelAmountMax;
            fuelPickup.DestroySelf();
        }

        if (collider2D.gameObject.TryGetComponent(out CoinPickup coinPickup))
        {
            OnCoinPickup?.Invoke(this, EventArgs.Empty);
            coinPickup.DestroySelf();
        }
    }

    public void SetPosition(Vector3 newPosition)
    {
        tf.position = newPosition;
    }

    private void ConsumeFuel()
    {
        fuelAmount -= fuelConsumptionAmount * Time.deltaTime;
        if (fuelAmount < 0) fuelAmount = 0;
    }

    private void SetState(State state)
    {
        this.state = state;
        OnStateChange?.Invoke(this, new OnStateChangedEventArgs
        {
            state = state
        });
    }

    public float GetFuel()
    {
        return fuelAmount;
    }
    public float GetFuelAmountNormalized()
    {
        return fuelAmount / fuelAmountMax;
    }
    public float GetSpeedX()
    {
        return landerRigidbody2D.velocity.x;
    }
    public float GetSpeedY()
    {
        return landerRigidbody2D.velocity.y;
    }
}

public enum LandingType
{
    None,
    Success,
    WrongLandingArea,
    TooSteepAngle,
    TooFastLanding,
}

public enum State
{
    None,
    WaitingToStart,
    Normal,
    GameOver
}
