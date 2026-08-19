using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[CreateAssetMenu(fileName = "GunTile", menuName = "Gun Tile")]

public class GunTile : Tile
{
    public Bullet.BulletType myType;

    public override bool StartUp(Vector3Int position, ITilemap tilemap, GameObject go)
    {
        if (go != null)
        {
            if (go.TryGetComponent<Gun>(out var gun))
                gun.Initialize(myType);
        }
        return true;
    }
}
