using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Pathfinding : MonoBehaviour
{
    public static readonly Vector3Int[] directions =
{
        Vector3Int.up,
        Vector3Int.down,
        Vector3Int.left,
        Vector3Int.right
    };

    public static List<Vector3Int> FindPath(Vector3Int start, Vector3Int goal, Vector3Int prior)
    {
        Queue<Vector3Int> queue = new();
        Dictionary<Vector3Int, Vector3Int> cameFrom = new();
        if (start == goal)
        {
            foreach (var dir in directions)
            {
                Vector3Int neighbor = goal + dir;
                if (!IsWalkable(neighbor))
                    continue;
                if (neighbor == start)
                    continue;
                goal = neighbor;
            }
        }

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

                //if (neighbor == prior && prior != goal) // Skips if neighbor tile being checked is the last tile the actor was in, and that tile isn't the goal
                //    continue;

                //if (neighbor == prior && prior == goal && current == start) // Skips if neighbor tile being checked is the last tile the actor was in, that last tile IS the goal, AND the current tile is the starting tile. (tile the actor currently occupies) So if the player is one tile behind the actor, the actor shouldn't be able to immediately turn around
                //    continue;

                queue.Enqueue(neighbor);
                cameFrom[neighbor] = current; // Stores neighbor tile and the tile that led to it
            }
        }

        return null;
    }

    protected static List<Vector3Int> ReconstructPath(Dictionary<Vector3Int, Vector3Int> cameFrom, Vector3Int start, Vector3Int goal)
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
    public static bool IsWalkable(Vector3Int pos)
    {
        return MapManager.currentTilemap.GetColliderType(pos) == Tile.ColliderType.None;
    }

    // Returns true if the given Vector3Int doesn't have a solid collider AND isn't a blank space
    public static bool IsWalkableStrict(Vector3Int pos)
    {
        // Tile must exist
        if (!MapManager.currentTilemap.HasTile(pos))
            return false;

        // Tile must not be blocked
        return MapManager.currentTilemap.GetColliderType(pos) == Tile.ColliderType.None;
    }

    // Checks if the given position, pos, equals the player's position and if it's a floor tile
    public static bool IsValidTarget(Vector3Int pos, Vector3Int playerPos)
    {
        if (pos == playerPos)
            return false;

        return IsWalkableStrict(pos);
    }

    public static Vector3Int FlankTile(Vector3Int initGoal, int flankMod)
    {
        // Find desired target position in front of player
        Vector3Int desiredGoal = initGoal + (Player.direction * flankMod);

        // If desired target is invalid, walk backward toward player
        // until a valid floor tile is found.
        while (!IsValidTarget(desiredGoal, initGoal))
        {
            desiredGoal -= Player.direction;

            // Prevent targeting player's own tile
            if (desiredGoal == initGoal)
                break;
        }

        // If still invalid, search nearby tiles around player
        if (!IsValidTarget(desiredGoal, initGoal))
        {
            foreach (Vector3Int dir in directions)
            {
                Vector3Int fallback = initGoal + dir;

                if (IsValidTarget(fallback, initGoal))
                {
                    desiredGoal = fallback;
                    break;
                }
            }
        }
        return desiredGoal;
    }

    /// <summary>
    /// Returns a random open tile within the given reach of a position.
    /// </summary>
    /// <returns></returns>
    public static Vector3Int RandomTile(Vector3Int pos, int randomTileReach)
    {
        Vector3Int target = new();

        do
        {
            target.x = UnityEngine.Random.Range(pos.x - randomTileReach, pos.x + randomTileReach);
            target.y = UnityEngine.Random.Range(pos.y - randomTileReach, pos.y + randomTileReach);
        }
        while (!IsWalkableStrict(target));// && i < (randomTileReach * randomTileReach));

        return target;
    }

    /// <summary>
    /// Returns a random open tile in a Sector.
    /// </summary>
    /// <returns></returns>
    public static Vector3Int RandomTile()
    {
        Vector3Int target = new();
        int range = MapManager.sectorHalf;

        do
        {
            target.x = UnityEngine.Random.Range(-range, range);
            target.y = UnityEngine.Random.Range(-range, range);
        }
        while (!IsWalkableStrict(target));

        return target;
    }

    public static List<Vector3Int> GetRandomReachableTiles(Vector3Int start)
    {
        Queue<Vector3Int> queue = new();
        HashSet<Vector3Int> visited = new();

        queue.Enqueue(start);
        visited.Add(start);

        while (queue.Count > 0)
        {
            Vector3Int current = queue.Dequeue();

            foreach (Vector3Int dir in directions)
            {
                Vector3Int neighbor = current + dir;

                if (visited.Contains(neighbor))
                    continue;

                if (!IsWalkableStrict(neighbor))
                    continue;

                visited.Add(neighbor);
                queue.Enqueue(neighbor);
            }
        }

        List<Vector3Int> reachable = new(visited);

        reachable.Remove(start);

        List<Vector3Int> result = new();

        int count = Mathf.Min(4, reachable.Count);

        for (int i = 0; i < count; i++)
        {
            int index = Random.Range(0, reachable.Count);

            result.Add(reachable[index]);
            reachable.RemoveAt(index);
        }

        return result;
    }
}
