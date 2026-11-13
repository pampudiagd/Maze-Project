using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[CreateAssetMenu(fileName = "DoorTile", menuName = "Door Tile")]
public class DoorTile : SpecialTile
{
    public static SpecialTile borderTile;
    [SerializeField] private SpecialTile myBorderTile;

    public override bool StartUp(Vector3Int position, ITilemap tilemap, GameObject go)
    {
        borderTile = myBorderTile;
        return base.StartUp(position, tilemap, go);
    }

    public static void ReplaceMe(Vector3Int gridPos, Tilemap tilemap)
    {
        tilemap.SetTile(gridPos, borderTile);
        tilemap.RefreshTile(gridPos);
    }
}
