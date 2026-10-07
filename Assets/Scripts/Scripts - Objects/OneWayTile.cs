using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Tilemaps;

[CreateAssetMenu(fileName = "OneWayTile", menuName = "One Way Tile")]
public class OneWayTile : SpecialTile
{
    public enum Direction
    {
        Up,
        Right,
        Down,
        Left
    }

    [SerializeField] private Direction direction;

    private float DirectionVector
    {
        get
        {
            return direction switch
            {
                Direction.Up => 0f,
                Direction.Right => 270f,
                Direction.Down => 180f,
                Direction.Left => 90f,
                _ => 0f
            };
        }
    }

    private void OnEnable()
    {
        EventManager.OnOneWayEnter += ToggleOneWay;
    }

    private void OnDisable()
    {
        EventManager.OnOneWayEnter -= ToggleOneWay;
    }

    public override bool StartUp(Vector3Int position, ITilemap tilemap, GameObject go)
    {
        if (go != null)
        {
            go.transform.rotation = Quaternion.Euler(0f, 0f, DirectionVector);
        }

        return true;
    }

    private void ToggleOneWay(bool playerInZone, Vector3Int gridPos)
    {
        if (playerInZone)
        {
            colliderType = ColliderType.None;
        }
        else 
            colliderType = ColliderType.Sprite;

        MapManager.currentTilemap.RefreshTile(gridPos);
    }

}
