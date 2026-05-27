using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class PathfindingFlank : Pathfinding
{
    [SerializeField][Tooltip("Number of spaces in front of the player the target tile should be.")] private int goalMod = 1;

    public override List<Vector3Int> FindPath(Vector3Int start, Vector3Int goal, Vector3Int prior)
    {
        // Find desired target position in front of player
        Vector3Int desiredGoal = goal + (Player.direction * goalMod);

        // If desired target is invalid, walk backward toward player
        // until a valid floor tile is found.
        while (!IsValidTarget(desiredGoal, goal))
        {
            desiredGoal -= Player.direction;

            // Prevent targeting player's own tile
            if (desiredGoal == goal)
                break;
        }

        // If still invalid, search nearby tiles around player
        if (!IsValidTarget(desiredGoal, goal))
        {
            foreach (Vector3Int dir in directions)
            {
                Vector3Int fallback = goal + dir;

                if (IsValidTarget(fallback, goal))
                {
                    desiredGoal = fallback;
                    break;
                }
            }
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

                if (!IsWalkable(neighbor))
                    continue;

                if (neighbor == prior)
                    continue;

                // Never move onto player's tile
                if (neighbor == goal)
                    continue;

                queue.Enqueue(neighbor);
                cameFrom[neighbor] = current;
            }
        }
        return base.FindPath(start, goal, prior);
    }

    // Specifically marked new so can use parent's logic when outside current tilemap
    protected new bool IsWalkable(Vector3Int pos)
    {
        // Tile must exist
        if (!MapManager.currentTilemap.HasTile(pos))
            return false;

        // Tile must not be blocked
        return MapManager.currentTilemap.GetColliderType(pos) == Tile.ColliderType.None;
    }

    bool IsValidTarget(Vector3Int pos, Vector3Int playerPos)
    {
        if (pos == playerPos)
            return false;

        return IsWalkable(pos);
    }
}
