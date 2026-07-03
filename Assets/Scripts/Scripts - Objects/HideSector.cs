using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class HideSector : MonoBehaviour
{
    private void Awake()
    {
        transform.GetChild(0).gameObject.SetActive(false);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Player Sector Sensor"))
            transform.GetChild(0).gameObject.SetActive(true);

    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Player Sector Sensor"))
            transform.GetChild(0).gameObject.SetActive(false);
    }
}
