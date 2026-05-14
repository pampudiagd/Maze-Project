using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public static class EnemyManager
{
    public static GameObject currentEnemyList = null;
    public static List<Spawner> spawners = new();

    // Disables the stored currentEnemyList, assigns it to the currentSector's list, and enables that
    public static void SetNewList()
    {
        if (currentEnemyList != null)
            currentEnemyList.SetActive(false);

        foreach (Transform item in MapManager.currentSector.transform)
        {
            if (item.CompareTag("EnemyList"))
            {
                currentEnemyList = item.gameObject;
                break;
            }
        }

        if (currentEnemyList != null)
            currentEnemyList.SetActive(true);
    }

    // Finds all spawners in the current sector's tilemap
    public static void FillSpawnersList()
    {
        spawners.Clear();
        foreach (Transform child in MapManager.currentTilemap.transform)
        {
            if (child.gameObject.CompareTag("Spawner"))
                spawners.Add(child.gameObject.GetComponent<Spawner>());
        }
        spawners.ForEach(item => { MapManager.print(item); });
    }

    // Tells every spawner in the spawner list to run their SpawnEnemy function
    public static IEnumerator TriggerSpawners()
    {
        yield return null;
        spawners.ForEach(item => { item.SpawnEnemy(); });
    }

    // Find an open spawner to assign to enemy's mySpawner
    // Set spawner's myEnemy to enemy and tells the spawner what prefab to load
    public static Spawner AssignSpawner(BadDude enemy, int prefabIndex)
    {
        Spawner foundSpawner = null;

        if (spawners.Count > 0)
        {
            foreach (var item in spawners)
            {
                if (item.enemyPrefab == null)
                {
                    item.myEnemy = enemy;
                    item.SetEnemyType(prefabIndex);
                    foundSpawner = item;

                    enemy.mySpawner.EmptySpawner();
                    break;
                }
            }
        }
        return foundSpawner;
    }



}
