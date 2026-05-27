using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEditor.Progress;

public class EnemyManager: MonoBehaviour
{
    public const float spawnerInterval = 1f;
    public const float initialDelay = 1f;

    public static GameObject currentEnemyList = null;
    public static List<Spawner> spawners = new();

    public static EnemyManager Instance { get; private set; }

    public enum EnemyType // Change to associate each with a relevant enemy prefab, so that each enemy doesn't need to hold their whole prefab
    {
        Chaser,
        Flanker,
        Test3,
        Test4,
        None
    }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

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
        //spawners.ForEach(item => { MapManager.print(item); });
    }

    // Tells every spawner in the spawner list to run their SpawnEnemy function and staggers spawns by specified spawnTimer amount of time, or the default set by spawnerInterval
    public static IEnumerator TriggerSpawners(float spawnTimer = spawnerInterval, float startTimer = initialDelay)
    {
        print("Attempting multiple spawns");
        int thisGeneration = MapManager.sectorGeneration;
        yield return new WaitForSeconds(startTimer);

        foreach (Spawner spawner in spawners)
        {
            if (spawner.myEnemy != null)
                continue;

            yield return new WaitForSeconds(spawnTimer);

            if (thisGeneration != MapManager.sectorGeneration)
                break;

            spawner.SpawnEnemy();
        }
    }

    public static IEnumerator TriggerSingleSpawner(Spawner spawner, float startTimer = initialDelay)
    {
        print("Attempting single spawn");
        int thisGeneration = MapManager.sectorGeneration;
        yield return new WaitForSeconds(startTimer);

        if (spawner.myEnemy != null || thisGeneration != MapManager.sectorGeneration)
            yield break;

        spawner.SpawnEnemy();
    }

    // Find an open spawner to assign to enemy's mySpawner
    // Set spawner's myEnemy to enemy and tells the spawner what prefab to load
    public static Spawner AssignSpawner(BadDude enemy, EnemyType prefabIndex)
    {
        Spawner foundSpawner = null;

        if (spawners.Count > 0)
        {
            foreach (var item in spawners)
            {
                if (item.enemyPrefab == null)
                {
                    item.FillSpawner(prefabIndex, enemy);
                    foundSpawner = item;

                    enemy.mySpawner.EmptySpawner();
                    break;
                }
            }
        }
        return foundSpawner;
    }
}
