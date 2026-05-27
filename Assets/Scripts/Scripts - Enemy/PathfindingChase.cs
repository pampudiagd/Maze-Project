using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class PathfindingChase: Pathfinding
{
    public override List<Vector3Int> FindPath(Vector3Int start, Vector3Int goal, Vector3Int prior)
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

            foreach (var dir in directions) // Checks a cardinal direction from directions
            {
                Vector3Int neighbor = current + dir;

                if (cameFrom.ContainsKey(neighbor)) // Skips if neighbor tile is already in cameFrom
                    continue;

                if (!IsWalkable(neighbor)) // Skips if neighbor tile has a collider
                    continue;

                if (neighbor == prior) 
                    continue;

                queue.Enqueue(neighbor);
                cameFrom[neighbor] = current; // Stores neighbor tile and the tile that led to it
            }
        }

        return null;
    }
}