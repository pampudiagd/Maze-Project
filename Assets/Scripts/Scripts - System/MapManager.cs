using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Tilemaps;

public class MapManager : MonoBehaviour
{
    public static readonly int sectorLength = 19;
    public static readonly int sectorHalf = (sectorLength - 1) / 2; 

    public static int sectorGeneration = 0;

    public GameObject startingSector;
    public static GameObject currentSector;
    public static Grid currentGrid;
    public static Tilemap currentTilemap;
    public GameObject player;
    public GameObject myCamera;
    private static Player playerScript;

    private int pursuingCount;
    private int pursuingFinished;

    private bool startMode = true;

    [SerializeField] private int sectorCount;
    [SerializeField] private int sectorsComplete = 0;

    public int camMoveSpeed = 2;
    public static Vector3Int PlayerGridPos => currentGrid.WorldToCell(playerScript.transform.position);

    private void OnEnable()
    {
        EventManager.OnZoneEnter += MoveSector;
        EventManager.PursueLogicStart += CountPursuing;
        EventManager.PursueLogicEnd += CheckPursuingDone;
        EventManager.SectorCompleted += CheckWinConditions;
    }

    private void OnDisable()
    {
        EventManager.OnZoneEnter -= MoveSector;
        EventManager.PursueLogicStart -= CountPursuing;
        EventManager.PursueLogicEnd -= CheckPursuingDone;
        EventManager.SectorCompleted -= CheckWinConditions;
    }

    private void Awake()
    {
        playerScript = player.GetComponent<Player>();
        MoveSector(startingSector);
    }

    private void Start()
    {
        CountSectors();
    }

    private void CountSectors()
    {
        foreach (Sector item in GetComponentsInChildren<Sector>(true))
            sectorCount++;
    }

    private void CheckWinConditions()
    {
        sectorsComplete++;
        if (sectorsComplete >= sectorCount)
            print("-----------WIN------------");
    }

    private void MoveSector(GameObject Sector)
    {
        sectorGeneration++;
        currentSector = Sector;
        print(currentSector.name);
        Transform gridChild = null;

        foreach (Transform child in Sector.transform) // Finds the collision grid in the new Sector
        {
            if (child.CompareTag("CollisionGrid"))
            {
                gridChild = child;
                break;
            }
        }
        currentGrid = gridChild.GetComponent<Grid>();
        currentTilemap = gridChild.GetComponentInChildren<Tilemap>();
        playerScript.UpdateMapInfo(gridChild);
        EnemyManager.SetNewList();
        EnemyManager.FillSpawnersList();

        if (EventManager.OnPursuingNewSector != null)
            EventManager.OnPursuingNewSector.Invoke(Sector);
        else
            StartCoroutine(EnemyManager.TriggerSpawners(startTimer: 1.5f));

        if (startMode)
        {
            startMode = false;
            return;
        }

        StartCoroutine(CamMove());
    }

    // Freeze game
    private IEnumerator CamMove()
    {
        Time.timeScale = 0f;

        Vector3 targetPos = currentSector.transform.position;
        targetPos.x += 0.5f;
        targetPos.z = myCamera.transform.position.z;

        while ((myCamera.transform.position - targetPos).sqrMagnitude > 0.001f)
        {
            myCamera.transform.position = Vector3.MoveTowards(myCamera.transform.position, targetPos, camMoveSpeed * Time.unscaledDeltaTime);
            yield return null;
        }
        Time.timeScale = 1f;
    }

    // No game freeze
    //private IEnumerator CamMove()
    //{
    //    print("Cam position: " + myCamera.transform.position + "\nNew Current Sector pos: " + currentSector.transform.position);

    //    while ((myCamera.transform.position - currentSector.transform.position).sqrMagnitude > 0.001f)
    //    {
    //        Vector3 targetPos = currentSector.transform.position;
    //        targetPos.x += 0.5f;
    //        targetPos.z = myCamera.transform.position.z;

    //        Vector2 newPos = Vector2.MoveTowards(myCamera.transform.position, targetPos, speed * Time.fixedDeltaTime);
    //        myCamera.GetComponent<Rigidbody2D>().MovePosition(newPos);
    //        yield return new WaitForFixedUpdate();
    //    }
    //}

    private void CountPursuing() => pursuingCount++;

    private void CheckPursuingDone()
    {
        pursuingFinished++;
        if (pursuingFinished == pursuingCount)
            StartCoroutine(EnemyManager.TriggerSpawners());
    }
}
