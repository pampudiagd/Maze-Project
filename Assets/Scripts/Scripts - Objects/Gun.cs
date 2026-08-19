using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gun : MonoBehaviour
{
    public Bullet.BulletType myType;

    public void Initialize(Bullet.BulletType type)
    {
        myType = type;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        print("Collected powerup");
        EventManager.OnCollectPowerup.Invoke(myType);
        Destroy(gameObject);
    }

}
