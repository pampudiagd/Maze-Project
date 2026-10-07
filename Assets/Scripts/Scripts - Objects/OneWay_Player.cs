using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OneWay_Player : MonoBehaviour
{
    public bool playerPassThru = false;
    private Vector3Int myGridPos;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        myGridPos = MapManager.currentTilemap.WorldToCell(transform.position);
        playerPassThru = true;
        EventManager.OnOneWayEnter.Invoke(playerPassThru, myGridPos);
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        playerPassThru = false;
        EventManager.OnOneWayEnter.Invoke(playerPassThru, myGridPos);
    }

}
