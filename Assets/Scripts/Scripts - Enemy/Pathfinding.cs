using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public abstract class Pathfinding : MonoBehaviour
{
    protected static readonly Vector3Int[] directions =
{
        Vector3Int.up,
        Vector3Int.down,
        Vector3Int.left,
        Vector3Int.right
    };

    public virtual List<Vector3Int> FindPath(Vector3Int start, Vector3Int goal, Vector3Int prior)
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

    protected virtual List<Vector3Int> ReconstructPath(Dictionary<Vector3Int, Vector3Int> cameFrom, Vector3Int start, Vector3Int goal)
    {
        List<Vector3Int> path = new();
        Vector3Int current = goal;

        while (current != start) // Goes through cameFrom, adding the tiles that connect goal to start into the List, path.
        {
            path.Add(current);
            current = cameFrom[current];
        }

        //path.Add(start);
        path.Reverse();
        return path;
    }
    
    // Returns true if the given Vector3Int doesn't have a solid collider
    protected virtual bool IsWalkable(Vector3Int pos)
    {
        return MapManager.currentTilemap.GetColliderType(pos) == Tile.ColliderType.None;
    }

    // Returns true if the given Vector3Int doesn't have a solid collider AND isn't a blank space
    protected virtual bool IsWalkableStrict(Vector3Int pos)
    {
        // Tile must exist
        if (!MapManager.currentTilemap.HasTile(pos))
            return false;

        // Tile must not be blocked
        return MapManager.currentTilemap.GetColliderType(pos) == Tile.ColliderType.None;
    }

    // Checks if the given position, pos, equals the player's position and if it's a floor tile
    protected bool IsValidTarget(Vector3Int pos, Vector3Int playerPos)
    {
        if (pos == playerPos)
            return false;

        return IsWalkableStrict(pos);
    }

    // Checks if the given position, pos, equals the player's position and if it's a floor tile
    protected bool IsValidTarget(Vector3Int pos, Vector3Int playerPos, Vector3Int start)
    {
        if (pos == playerPos || pos == start)
            return false;

        return IsWalkableStrict(pos);
    }
}
