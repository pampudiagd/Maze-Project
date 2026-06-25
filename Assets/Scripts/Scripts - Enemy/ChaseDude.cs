using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChaseDude : BadDude
{
    protected override IEnumerator Move()
    {
        while (true)
        {
            if (!Pathfinding.IsWalkableStrict(MyGridPos))
                yield return MoveIntoBounds();
            if (!EnemyManager.distanceMapChase.ContainsKey(MyGridPos))
                yield return base.Move();

            ChooseDirection(EnemyManager.distanceMapChase);

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
