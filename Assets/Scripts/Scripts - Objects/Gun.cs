using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gun : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        print("Collected powerup");
        EventManager.OnCollectPowerup.Invoke();
        Destroy(gameObject);
    }

}
