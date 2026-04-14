using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class CoinMap : MonoBehaviour
{
    Tilemap map;

    // Start is called before the first frame update
    void Start()
    {
        map = gameObject.GetComponent<Tilemap>();
        map.color = new Color(1, 1, 1, 0);
    }
}
