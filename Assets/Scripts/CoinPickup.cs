using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoinPickup : MonoBehaviour
{
    public void DestroySelf()
    {
        Destroy(gameObject);
    }
}
