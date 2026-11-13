using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sector : MonoBehaviour
{
    public int coinsTotal;
    public int coinsRemaining;

    public int halfAmount;
    public bool halfCoinsCollected = false;

    public bool bankFilled = false;
    public bool completionMark = false; // Prevents the Sector progressing the win condition again every time its completion is calculated

    public bool startSector = false;

    public enum SectorCompletionState
    {
        None,
        HalfCoins,
        AllCoins,
        BankFilled,
        Completed
    }

    private SectorCompletionState state;
    public SectorCompletionState State => state;

    private void Start()
    {
        FindBank();
        FindCoins();
    }

    private void FindBank()
    {
        GetComponentInChildren<Bank>(true).mySector = this;
    }

    private void FindCoins()
    {
        coinsTotal = GetComponentsInChildren<Coin>(true).Length;
        coinsRemaining = coinsTotal;
        halfAmount = coinsTotal / 2;
        print("Number of coins: " + coinsTotal);

        foreach (Coin item in GetComponentsInChildren<Coin>(true))
        {
            item.mySector = this;
            if (!GlobalVar.spawnCoins)
                item.gameObject.SetActive(false);
        }
    }

    // Determines what state of completion the Sector is in
    public void CalcCompState()
    {
        if (!halfCoinsCollected && coinsRemaining <= halfAmount)
        {
            halfCoinsCollected = true;
            state = SectorCompletionState.HalfCoins;
            EventManager.SectorHalfway.Invoke(gameObject);
        }

        switch (coinsRemaining == 0, bankFilled)
        {
            case (false, false):
                state = SectorCompletionState.None;
                break;
            case (true, false):
                state = SectorCompletionState.AllCoins;
                if (!completionMark)
                {
                    completionMark = true;
                    EventManager.SectorCompleted.Invoke();
                }
                break;
            case (false, true):
                state = SectorCompletionState.BankFilled;
                break;
            case (true, true):
                state = SectorCompletionState.Completed;
                break;
        }
    }

}
