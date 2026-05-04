using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapManager : MonoBehaviour
{
    public static GameObject currentSector;
    public GameObject player;
    public GameObject myCamera;
    private Player playerScript;

    public int speed = 2;

    private void OnEnable()
    {
        EventManager.OnZoneEnter += MoveSector;
    }

    private void OnDisable()
    {
        EventManager.OnZoneEnter -= MoveSector;
    }

    // Start is called before the first frame update
    void Start()
    {
        playerScript = player.GetComponent<Player>();
    }

    private void MoveSector(GameObject Sector)
    {
        currentSector = Sector;
        Transform gridChild = null;

        foreach (Transform child in Sector.transform)
        {
            if (child.CompareTag("CollisionGrid"))
            {
                gridChild = child;
                break;
            }
        }
        playerScript.UpdateMapInfo(gridChild);
        StartCoroutine(CamMove());
    }

    // Freeze game
    private IEnumerator CamMove()
    {
        Time.timeScale = 0f;

        Vector3 targetPos = currentSector.transform.position;
        targetPos.x += 0.5f;
        targetPos.y += 0.5f;
        targetPos.z = myCamera.transform.position.z;

        while ((myCamera.transform.position - targetPos).sqrMagnitude > 0.001f)
        {
            myCamera.transform.position = Vector3.MoveTowards(myCamera.transform.position, targetPos, speed * Time.unscaledDeltaTime);
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
}
