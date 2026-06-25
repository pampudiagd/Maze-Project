using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlankDude : BadDude
{
    protected override IEnumerator Move()
    {
        while (true)
        {
            if (!Pathfinding.IsWalkableStrict(MyGridPos))
                yield return MoveIntoBounds();
            if (!EnemyManager.distanceMapFlank.ContainsKey(MyGridPos))
                yield return base.Move();

            ChooseDirection(EnemyManager.distanceMapFlank);

            targetTile = MapManager.currentGrid.GetCellCenterWorld(MyGridPos + Vector3Int.FloorToInt(myDirection));

            while (transform.position != targetTile)
            {
                transform.position = Vector3.MoveTowards(transform.position, targetTile, speed * Time.deltaTime);
                yield return null;
            }
            transform.position = targetTile;
        }
    }
}
