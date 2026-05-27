using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public static class EventManager
{
    public static Func<bool> OnCollectCoin; // Returns a bool
    public static Action<GameObject> OnZoneEnter; // Takes a parameter
    public static Action<GameObject> OnPursuingNewSector;
    public static Action OnCollectPowerup;
    public static Action PursueLogicStart;
    public static Action PursueLogicEnd;
    public static Action SectorCompleted;
    public static Action OnBankFilled;

}
