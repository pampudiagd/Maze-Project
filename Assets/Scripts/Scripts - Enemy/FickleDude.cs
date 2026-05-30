using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class FickleDude : BadDude
{
    // Moves one tile at a time
    protected override IEnumerator Move()
    {
        yield return null;
        Vector3Int lastPlayerPos = MapManager.PlayerGridPos;
        List<Vector3Int> path = myPathfindingType.FindPath(MyGridPos, MapManager.PlayerGridPos, myPriorPos);
        myPriorPos = MyGridPos;
        int pathIndex = 0;

        while (true)
        {
            if (path == null || pathIndex >= path.Count || lastPlayerPos != MapManager.PlayerGridPos)
            {
                lastPlayerPos = MapManager.PlayerGridPos;
                path = myPathfindingType.FindPath(MyGridPos, lastPlayerPos, myPriorPos);
                myPriorPos = MyGridPos;
                pathIndex = 0;
            }

            while (MyGridPos != path.Last())
            {
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
    }
}