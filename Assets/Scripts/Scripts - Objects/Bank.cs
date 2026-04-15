using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bank : MonoBehaviour
{
    public int maxCapacity = 20;
    public int currentCoins = 0;

    public int DepositCoins(int amount)
    {
        int spaceLeft = maxCapacity - currentCoins;
        int accepted = Mathf.Min(amount, spaceLeft);

        currentCoins += accepted;
        return accepted;
    }

    public bool IsFull()
    {
        return currentCoins >= maxCapacity;
    }
}
