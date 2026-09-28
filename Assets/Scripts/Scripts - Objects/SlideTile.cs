using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlideTile : MonoBehaviour
{
    public static int touchingPlayer = 0;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            touchingPlayer++;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            touchingPlayer--;
        }
    }

}
