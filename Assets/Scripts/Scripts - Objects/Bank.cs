using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bank : MonoBehaviour
{
    public Sector mySector;
    public int maxCapacity = 100;
    public int currentCoins = 0;
    private GameObject nukeBox;

    private void Start()
    {
        nukeBox = transform.GetChild(0).gameObject;
    }

    public int DepositCoins(int amount)
    {
        if (currentCoins >= maxCapacity)
            return 0;
        int spaceLeft = maxCapacity - currentCoins;
        int accepted = Mathf.Min(amount, spaceLeft);

        currentCoins += accepted;
        if (currentCoins >= maxCapacity)
            Filled();
        return accepted;
    }

    public void Filled()
    {
        mySector.bankFilled = true;
        StartCoroutine(BankNuke());
        EventManager.OnBankFilled?.Invoke();
        mySector.CalcCompState();
    }

    private IEnumerator BankNuke()
    {
        nukeBox.SetActive(true);
        for (int i = 0; i < 80; i++)
        {
            nukeBox.transform.localScale += new Vector3(1, 1, 0);
            yield return new WaitForSeconds(0.01f);
        }
        nukeBox.transform.localScale = Vector3.one;
        nukeBox.SetActive(false);
    }
}
