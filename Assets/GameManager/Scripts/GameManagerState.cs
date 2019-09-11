using System.Collections;
using System.Collections.Generic;
using StateKraft;
using UnityEngine;

public class GameManagerState : State
{
    protected GameManager manager;
    protected RoundController roundController => manager.RoundController;
    
    public override void Initialize(object owner)
    {
        manager = (GameManager)owner;
    }
}
