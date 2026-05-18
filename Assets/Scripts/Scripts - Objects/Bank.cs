using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bank : MonoBehaviour
{
    public Sector mySector;
    public int maxCapacity = 20;
    public int currentCoins = 0;

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
        mySector.CalcCompState();
    }
}
