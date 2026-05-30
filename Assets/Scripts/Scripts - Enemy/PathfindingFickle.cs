using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class PathfindingFickle : Pathfinding
{
    [SerializeField] private int randomTileReach = 4;

    public override List<Vector3Int> FindPath(Vector3Int start, Vector3Int goal, Vector3Int prior)
    {
        Vector3Int desiredGoal = RandomTile(goal);
        int i = 0;
        while (!IsValidTarget(desiredGoal, goal, start) && i < (randomTileReach^2))
        {
            desiredGoal = RandomTile(goal);
            i++;
        }

        Queue<Vector3Int> queue = new();
        Dictionary<Vector3Int, Vector3Int> cameFrom = new();

        queue.Enqueue(start);
        cameFrom[start] = start;

        while (queue.Count > 0)
        {
            Vector3Int current = queue.Dequeue();

            if (current == desiredGoal)
                return ReconstructPath(cameFrom, start, desiredGoal);

            foreach (Vector3Int dir in directions)
            {
                Vector3Int neighbor = current + dir;

                if (cameFrom.ContainsKey(neighbor))
                    continue;

                if (!IsWalkableStrict(neighbor))
                    continue;

                if (neighbor == prior)
                    continue;

                // Never move onto player's tile
                //if (neighbor == goal)
                //    continue;

                queue.Enqueue(neighbor);
                cameFrom[neighbor] = current;
            }
        }

        return base.FindPath(start, goal, prior);
    }

    private Vector3Int RandomTile(Vector3Int playerPos)
    {
        Vector3Int target = new();

        target.x = UnityEngine.Random.Range(playerPos.x - randomTileReach, playerPos.x + randomTileReach);
        target.y = UnityEngine.Random.Range(playerPos.y - randomTileReach, playerPos.y + randomTileReach);

        return target;
    }
}
