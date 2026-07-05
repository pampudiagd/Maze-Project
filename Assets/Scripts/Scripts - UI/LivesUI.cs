using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LivesUI : MonoBehaviour
{
    [SerializeField] private Image lifeIconPrefab;
    [SerializeField] private int maxLives = 10;

    private List<Image> lifeIcons = new();

    private void Awake()
    {
        for (int i=0; i < maxLives; i++)
        {
            Image icon = Instantiate(lifeIconPrefab, transform);
            lifeIcons.Add(icon);
        }
    }

    public void SetLives(int currentLives)
    {
        for (int i = 0; i < lifeIcons.Count; i++)
        {
            lifeIcons[i].gameObject.SetActive(i < currentLives);
        }
    }
}