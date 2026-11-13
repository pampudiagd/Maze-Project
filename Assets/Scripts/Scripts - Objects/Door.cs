using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Tilemaps;
using static UnityEditor.PlayerSettings;

public class Door : MonoBehaviour
{
    public List<GameObject> mySectors = new();
    public Vector3Int myGridPos;
    public Tilemap myTileMap;

    private void OnEnable()
    {
        EventManager.SectorHalfway += TryUnlock;
    }

    private void OnDisable()
    {
        EventManager.SectorHalfway -= TryUnlock;
    }

    private void Start()
    {
        myTileMap = GetComponentInParent<Tilemap>();
        myGridPos = myTileMap.WorldToCell(transform.position);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("SectorZone") && !mySectors.Contains(collision.gameObject.GetComponentInChildren<LoadZone>().mySector))
        {
            mySectors.Add(collision.gameObject.GetComponentInChildren<LoadZone>().mySector);
        }
    }

    private void TryUnlock(GameObject sector)
    {
        if (mySectors.Contains(sector))
        {
            DoorTile.ReplaceMe(myGridPos, myTileMap);

            EventManager.SectorHalfway -= TryUnlock;
            Destroy(transform.gameObject);
        }
    }
}
