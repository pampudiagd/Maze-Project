using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerHitbox : MonoBehaviour
{
    private Player player;

    // Start is called before the first frame update
    void Start()
    {
        player = GetComponentInParent<Player>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Bank"))
        {
            Bank bank = collision.GetComponent<Bank>();

            if (bank != null && player.coinCount > 0)
            {
                int deposited = bank.DepositCoins(player.coinCount);

                GlobalVar.Score += (deposited * 10);
                print("Current Score: " + GlobalVar.Score);

                player.coinCount -= deposited;
                player.CalculateWeight();
            }
        }
        else if (collision.gameObject.layer == LayerMask.NameToLayer("Enemy"))
        {
            if (Time.time > player.InvincibleUntil)
            {
                GetComponent<CircleCollider2D>().gameObject.SetActive(false);
                player.isDead = true;
                print("<<<<<<<<<<GAME OVER>>>>>>>>>>>"); // Move Scene change logic later!!!!
                SceneManager.LoadScene(0);
            }
        }
    }
}
