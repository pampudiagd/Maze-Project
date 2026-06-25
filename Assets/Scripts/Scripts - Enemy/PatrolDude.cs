using System.Collections;
using System.Collections.Generic;
using System.Threading;
using TMPro;
using UnityEditor.SearchService;
using UnityEngine;

public class PatrolDude : BadDude
{
    private Vector3Int goalTile;

    private List<Vector3Int> targetList = new();
    private int currentIndex = 0;

    public static Dictionary<Vector3Int, int> distanceMap = new();

    //private TextMeshPro debugMarker;
    //public GameObject debugPrefab;
    //[SerializeField] private GameObject holder;

    protected override void Activate()
    {
        //holder = GameObject.FindGameObjectWithTag("DebugHolder");
        targetList = Pathfinding.GetRandomReachableTiles(MyGridPos);
        PopDistMap();
        StartCoroutine(Move());
    }

    protected override IEnumerator Move()
    {
        targetTile = MapManager.currentGrid.GetCellCenterWorld(MyGridPos);

        while (true)
        {
            if (!Pathfinding.IsWalkableStrict(MyGridPos))
                yield return MoveIntoBounds();

            ChooseDirection(distanceMap);

            targetTile = MapManager.currentGrid.GetCellCenterWorld(MyGridPos + Vector3Int.FloorToInt(myDirection));

            while (transform.position != targetTile)
            {
                transform.position = Vector3.MoveTowards(transform.position, targetTile, speed * Time.deltaTime);
                yield return null;
            }
            transform.position = targetTile;

            if (goalTile == MyGridPos)
                PopDistMap();
        }
    }

    private Vector3Int CycleNextTarget()
    {
        Vector3Int target = targetList[currentIndex];

        if (currentIndex == targetList.Count - 1)
            currentIndex = 0;
        else
            currentIndex++;

        return target;
    }

    // pick 4 random tiles from reachable tiles
    // Patrol the path between the 4 tiles
    private void PopDistMap()
    {
        goalTile = CycleNextTarget();

        Queue<Vector3Int> queue = new();

        distanceMap.Clear();

        //foreach (Transform item in holder.transform)
        //{
        //    Destroy(item.gameObject);
        //}

        queue.Enqueue(goalTile);
        distanceMap[goalTile] = 0;

        while (queue.Count > 0)
        {
            Vector3Int current = queue.Dequeue();

            foreach (var dir in Pathfinding.directions)
            {
                Vector3Int neighbor = current + dir;
                if (distanceMap.ContainsKey(neighbor))
                    continue;
                if (!Pathfinding.IsWalkableStrict(neighbor))
                    continue;

                distanceMap[neighbor] = distanceMap[current] + 1;

                //GameObject debug = Instantiate(debugPrefab, MapManager.currentGrid.GetCellCenterWorld(neighbor), holder.transform.rotation, holder.transform);
                //debugMarker = debug.GetComponent<TextMeshPro>();
                //debugMarker.text = "" + distanceMap[neighbor];

                queue.Enqueue(neighbor);
            }
        }
    }
}
