using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlankDude : BadDude
{
    private Vector3 targetTile;

    protected override IEnumerator Move()
    {
        targetTile = MapManager.currentGrid.GetCellCenterWorld(MyGridPos);
        while (true)
        {
            if (!Pathfinding.IsWalkableStrict(MyGridPos))
                yield return MoveIntoBounds();

            ChooseDirection();

            targetTile = MapManager.currentGrid.GetCellCenterWorld(MyGridPos + Vector3Int.FloorToInt(myDirection));

            while (transform.position != targetTile)
            {
                transform.position = Vector3.MoveTowards(transform.position, targetTile, speed * Time.deltaTime);
                yield return null;
            }
            transform.position = targetTile;
        }
    }

    private void ChooseDirection()
    {
        Vector3Int reverse = -myDirection;

        Vector3Int bestDirection = Vector3Int.zero;
        int bestDistance = int.MaxValue;

        foreach (Vector3Int dir in Pathfinding.directions)
        {
            // Don't immediately reverse.
            if (dir == reverse)
                continue;

            Vector3Int neighbor = MyGridPos + dir;

            // Can't move into walls.
            if (!Pathfinding.IsWalkableStrict(neighbor))
                continue;

            // Skip unreachable tiles.
            if (!EnemyManager.distanceMapFlank.ContainsKey(neighbor))
                continue;

            int distance = EnemyManager.distanceMapFlank[neighbor];

            if (distance < bestDistance)
            {
                bestDistance = distance;
                bestDirection = dir;
            }
        }

        // Dead-end handling.
        if (bestDirection == Vector3Int.zero)
        {
            bestDirection = reverse;
        }

        myDirection = bestDirection;
        transform.up = bestDirection;
    }
}
