using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AmmoUI : MonoBehaviour
{
    [SerializeField] private Image ammoIconPrefab;
    [SerializeField] private int maxAmmo = 10;
    [SerializeField] private float drawDelay = 0.05f;

    private List<Image> ammoIcons = new();

    private void Awake()
    {
        for (int i=0; i < maxAmmo; i++)
        {
            Image icon = Instantiate(ammoIconPrefab, transform);
            ammoIcons.Add(icon);
        }
    }

    public void SetAmmo(int currentAmmo)
    {
        StopAllCoroutines();
        StartCoroutine(AnimateAmmo(currentAmmo));
    }

    //AnimateAmmo makes it so the ammo icons spawn with a slight delay.
    //This way they appear one at a time, not all at once.
    //It uses a coroutine. Check this if there are frame drops.
    //However, this coroutine should be simple enough to not cause performance issues.
    private IEnumerator AnimateAmmo(int currentAmmo)
    {
        for (int i = 0; i < ammoIcons.Count; i++)
        {
            ammoIcons[i].gameObject.SetActive(false);
        }

        for (int i = 0; i < currentAmmo; i++)
        {
            ammoIcons[i].gameObject.SetActive(true);
            yield return new WaitForSeconds(drawDelay);
        }
    }
}