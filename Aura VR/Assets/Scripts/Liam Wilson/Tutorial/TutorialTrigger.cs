﻿using System;
using System.Collections;
using System.Collections.Generic;
using AuraHull.AuraVRGame;
using Photon.Pun;
using UnityEngine;

public class TutorialTrigger : MonoBehaviour
{
    public int tutorialIndex;
    public bool canTriggerEarly = false;
    public Action OnConditionMet;

    protected bool live = false;

    protected virtual void Start()
    {
        TutorialManager.Instance.AddTriggerCondition(this);
    }

    public void SetLive(bool value = true)
    {
        live = value;
    }

    protected void Trigger()
    {
        if (!PhotonNetwork.IsMasterClient) return;

        if (canTriggerEarly)
        {
            OnConditionMet?.Invoke();
        }
        else
        {
            TriggerIfLive();
        }
    }

    private void TriggerIfLive()
    {
        if (live)
        {
            OnConditionMet?.Invoke();
            live = false;
        }
    }
}