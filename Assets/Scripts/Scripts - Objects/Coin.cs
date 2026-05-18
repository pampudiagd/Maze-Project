using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Coin : MonoBehaviour
{
    public Sector mySector;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        bool collected = EventManager.OnCollectCoin?.Invoke() ?? false;
        if (collected)
        {
            mySector.coinsRemaining--;
            mySector.CalcCompState();
            Destroy(gameObject);
        }
    }

}
