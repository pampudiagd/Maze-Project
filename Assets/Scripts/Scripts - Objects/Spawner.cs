using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    public GameObject enemyPrefab;
    public BadDude myEnemy;
    public List<GameObject> prefabList = new();

    private enum EnemyType // Change to associate each with a relevant enemy prefab, so that each enemy doesn't need to hold their whole prefab
    {
        Chaser,
        Test2,
        Test3,
        Test4,
        None
    }

    public void SetEnemyType(int num)
    {
        print("List length: " + prefabList.Count);
        enemyPrefab = prefabList[num];
    }

    public void EmptySpawner()
    {
        print($"Emptying {this.name}");
        enemyPrefab = null;
        myEnemy = null;
    }

    public void SpawnEnemy()
    {
        if (myEnemy == null && enemyPrefab != null)
        {
            myEnemy = Instantiate(enemyPrefab, transform.position, transform.rotation, EnemyManager.currentEnemyList.transform).GetComponent<BadDude>();
            myEnemy.Initialize(this);
        }
        else
            print("--------------Enemy not spawned----------------");
        if (myEnemy == null)
            print($"{this.name} FAILED BECAUSE NULL");
        else if (myEnemy.isPursuing)
            print($"{this.name} FAILED BECAUSE PURSUING");
    }
}
