using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public static class Pathfinding
{
    static readonly Vector3Int[] directions =
    {
        Vector3Int.up,
        Vector3Int.down,
        Vector3Int.left,
        Vector3Int.right
    };

    public static List<Vector3Int> FindPath(Vector3Int start, Vector3Int goal)
    {
        Queue<Vector3Int> queue = new();
        Dictionary<Vector3Int, Vector3Int> cameFrom = new();

        queue.Enqueue(start);
        cameFrom[start] = start;

        while (queue.Count > 0)
        {
            Vector3Int current = queue.Dequeue();

            if (current == goal)
                return ReconstructPath(cameFrom, start, goal);

            foreach (var dir in directions)
            {
                Vector3Int neighbor = current + dir;

                if (cameFrom.ContainsKey(neighbor))
                    continue;

                if (!IsWalkable(neighbor))
                    continue;

                queue.Enqueue(neighbor);
                cameFrom[neighbor] = current;
            }
        }

        return null;
    }

    static bool IsWalkable(Vector3Int pos)
    {
        return MapManager.currentTilemap.GetColliderType(pos) == Tile.ColliderType.None;
    }

    static List<Vector3Int> ReconstructPath(Dictionary<Vector3Int, Vector3Int> cameFrom, Vector3Int start, Vector3Int goal)
    {
        List<Vector3Int> path = new();
        Vector3Int current = goal;

        while (current != start)
        {
            path.Add(current);
            current = cameFrom[current];
        }

        //path.Add(start);
        path.Reverse();
        return path;
    }
}