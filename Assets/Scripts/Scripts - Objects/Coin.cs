using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Coin : MonoBehaviour
{
    public Sector mySector;
    public bool isMulti = false;
    private SpriteRenderer sprite;


    private void Start()
    {
        sprite = GetComponent<SpriteRenderer>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        bool collected = EventManager.OnCollectCoin?.Invoke() ?? false;
        if (collected && !isMulti)
        {
            mySector.coinsRemaining--;
            mySector.CalcCompState();
            Destroy(gameObject);
        }
        else if (isMulti)
        {
            isMulti = false;
            sprite.color = new Color(1,1,1,0.5f);
        }
    }

}
