using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadZone : MonoBehaviour
{
    public GameObject mySector;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Player") && transform.parent.gameObject != MapManager.currentSector)
            EventManager.OnZoneEnter.Invoke(mySector.transform.GetChild(0).gameObject);
    }
}
