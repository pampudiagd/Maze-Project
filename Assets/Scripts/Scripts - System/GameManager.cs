using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public Grid sectorGrid;
    public int gridLength;

    private GameObject[] sectorPrefabs;
    [SerializeField] private GlobalSettings settings;

    private void Awake()
    {
        GlobalVar.spawnEnemies = settings.allowEnemySpawns;
        GlobalVar.spawnCoins = settings.allowCoins;

        sectorGrid = GetComponent<Grid>();
        LoadSectors();
    }

    private void LoadSectors()
    {
        gridLength = 3;
        sectorPrefabs = Resources.LoadAll<GameObject>("Sectors/Easy");

        if (GlobalVar.difficulty == 1)
        {
            gridLength = 5;
            sectorPrefabs.AddRange(Resources.LoadAll<GameObject>("Sectors/Medium"));
        }
        else if (GlobalVar.difficulty == 2)
        {
            sectorPrefabs.AddRange(Resources.LoadAll<GameObject>("Sectors/Hard"));
            gridLength = 7;
        }

        // Sorts the prefabs by the two character id ([Letter][Number]) at the end of their name
        Array.Sort(sectorPrefabs, (a, b) =>
        {
            Vector2Int posA = ReadSectorID(a.name.Substring(a.name.LastIndexOf(' ') + 1));
            Vector2Int posB = ReadSectorID(b.name.Substring(b.name.LastIndexOf(' ') + 1));

            int rowCompare = posA.y.CompareTo(posB.y);
            if (rowCompare != 0)
                return rowCompare;

            return posA.x.CompareTo(posB.x);
        });

        //for (int i = 0; i < sectorPrefabs.Length; i++)
        //{
        //    print(sectorPrefabs[i].name + " " + ReadSectorID(sectorPrefabs[i].name.Substring(sectorPrefabs[i].name.LastIndexOf(' ') + 1)));
        //}

        int listIndex = 0;
        for (int j = 0; j < gridLength; j++)
        {
            for (int i = 0; i < gridLength; i++)
            {
                GameObject instance = Instantiate(sectorPrefabs[listIndex], sectorGrid.CellToWorld(new Vector3Int(i,-j)), transform.rotation, gameObject.transform);
                listIndex++;
                if (listIndex >= sectorPrefabs.Length)
                    break;
            }
        }

        //foreach (GameObject p in sectorPrefabs)
        //{
        //    Vector2Int idPos = ReadSectorID(p.GetComponent<Sector>().id);
        //    Vector3 sectorWorldPos = sectorGrid.CellToWorld(new Vector3Int(idPos.x, idPos.y));


        //    GameObject instance = Instantiate(p, sectorWorldPos, transform.rotation);
        //    print(sectorGrid.WorldToCell(instance.transform.position));
        //}
    }

    // Converts a string indicating a Sector's id ([Letter][Number]) into a Vector2int for comparison
    private static Vector2Int ReadSectorID(string id)
    {
        int row = char.ToUpper(id[0]) - 'A';
        int column = int.Parse(id.Substring(1));

        return new Vector2Int(column, row);
    }

}
