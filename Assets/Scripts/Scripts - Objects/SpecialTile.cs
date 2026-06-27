using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[CreateAssetMenu(fileName = "SpecialTile", menuName = "Special Tile")]
public class SpecialTile : Tile
{
    public bool blocksEnemies;
}
