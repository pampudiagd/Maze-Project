using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public abstract class BadDude : MonoBehaviour
{
    public EnemyManager.EnemyType myType = EnemyManager.EnemyType.None;

    public Spawner mySpawner;
    public int speed = 5;

    public bool isPursuing = false;

    protected Vector3 targetTile;

    public Vector3Int MyGridPos => MapManager.currentGrid.WorldToCell(transform.position);
    public Vector3Int myPriorPos;

    public Vector3Int myDirection = Vector3Int.up;

    protected Dictionary<Vector3Int, int> fallbackDistMap = new();

    protected virtual void OnEnable()
    {
        Activate();
    }

    protected virtual void OnDisable()
    {
        if (!isPursuing)
        {
            EventManager.OnPursuingNewSector -= PursueSector;
            Destroy(gameObject);
        }
    }

    public virtual void Initialize(Spawner newSpawner)
    {
        mySpawner = newSpawner;
    }

    protected virtual void Activate()
    {
        StartCoroutine(Move());
    }

    protected virtual IEnumerator Move()
    {
        while (true)
        {
            if (!Pathfinding.IsWalkableStrict(MyGridPos))
            {
                yield return MoveIntoBounds();
                yield break;
            }

            do
                PopDistMap();
            while (!fallbackDistMap.ContainsKey(MyGridPos));

            ChooseDirection(fallbackDistMap);

            targetTile = MapManager.currentGrid.GetCellCenterWorld(MyGridPos + Vector3Int.FloorToInt(myDirection));

            while (transform.position != targetTile)
            {
                transform.position = Vector3.MoveTowards(transform.position, targetTile, speed * Time.deltaTime);
                yield return null;
            }
            transform.position = targetTile;
        }
    }

    protected void ChooseDirection(Dictionary<Vector3Int, int> distanceMap)
    {
        Vector3Int reverse = -myDirection;

        List<Vector3Int> bestDirections = new();
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
            if (!distanceMap.ContainsKey(neighbor))
                continue;

            int distance = distanceMap[neighbor];

            if (distance < bestDistance)
            {
                bestDistance = distance;
                bestDirections.Clear();
                bestDirections.Add(dir);
            }
            else if (distance == bestDistance)
                bestDirections.Add(dir);
        }

        bestDirection = bestDirections[Random.Range(0, bestDirections.Count)];

        // Dead-end handling.
        if (bestDirection == Vector3Int.zero)
        {
            bestDirection = reverse;
        }

        myDirection = bestDirection;
        transform.up = bestDirection;
    }

    // Moves one tile at a time
    protected IEnumerator MoveIntoBounds()
    {
        yield return null;
        List<Vector3Int> path = Pathfinding.FindPath(MyGridPos, MapManager.PlayerGridPos, myPriorPos);
        int pathIndex = 0;

        while (pathIndex < path.Count - 1)
        {
            myPriorPos = MyGridPos;
            if (path.Count == 0)
                print("Path is empty");
            Vector3Int targetTile = path[pathIndex];
            transform.up = RotateToVector3(targetTile);

            Vector3 targetWorld = MapManager.currentGrid.GetCellCenterWorld(targetTile);

            while ((transform.position - targetWorld).sqrMagnitude > 0.001f)
            {
                transform.position = Vector3.MoveTowards(transform.position, targetWorld, speed * Time.deltaTime);
                yield return null;
            }

            pathIndex++;
        }
    }

    void PopDistMap()
    {
        Vector3Int goalTile = Pathfinding.RandomTile();

        Queue<Vector3Int> queue = new();

        fallbackDistMap.Clear();

        queue.Enqueue(goalTile);
        fallbackDistMap[goalTile] = 0;

        while (queue.Count > 0)
        {
            Vector3Int current = queue.Dequeue();

            foreach (var dir in Pathfinding.directions)
            {
                Vector3Int neighbor = current + dir;
                if (fallbackDistMap.ContainsKey(neighbor))
                    continue;
                if (!Pathfinding.IsWalkableStrict(neighbor))
                    continue;

                fallbackDistMap[neighbor] = fallbackDistMap[current] + 1;

                queue.Enqueue(neighbor);
            }
        }
    }

    protected Vector3 RotateToVector3(Vector3Int targetTile)
    {
        Vector3Int difference = targetTile - MyGridPos;
        if (difference.x > 0)
            return Vector3.right;
        else if (difference.x < 0)
            return Vector3.left;
        else if (difference.y > 0)
            return Vector3.up;
        else if (difference.y < 0)
            return Vector3.down;
        else
            return new Vector3(1, 1, 0); // This should NEVER get here.
    }

    protected virtual void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Bullet") || collision.CompareTag("BankAttack"))
            Death();
        else if (collision.gameObject.layer == LayerMask.NameToLayer("Pursuit"))
        {
            print("ENTERED PURSUIT RANGE");
            isPursuing = true;
            EventManager.OnPursuingNewSector += PursueSector;
        }
    }

    private void Death()
    {
        EnemyManager.Instance.StartCoroutine(EnemyManager.TriggerSingleSpawner(mySpawner, startTimer: 4.0f)); // Calls for mySpawner to spawn
        Destroy(gameObject);
    }

    protected virtual void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Pursuit"))
        {
            print("LEFT THE PURSUIT RANGE");
            isPursuing = false;
            EventManager.OnPursuingNewSector -= PursueSector;
        }
    }

    // Sets parent to the EnemyList of the newSector and assigns to an empty spawner. If none are empty, this object destroys itself
    protected virtual void PursueSector(GameObject newSector)
    {
        EventManager.PursueLogicStart.Invoke();
        Transform newParentList = EnemyManager.currentEnemyList.transform;

        mySpawner = EnemyManager.AssignSpawner(this, myType);

        if (mySpawner != null)
        {
            transform.SetParent(newParentList);
            EventManager.PursueLogicEnd.Invoke();
        }
        else
        {
            EventManager.PursueLogicEnd.Invoke();
            Destroy(gameObject);
        }
    }
}
