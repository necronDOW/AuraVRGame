﻿using System.Collections;
using System.Collections.Generic;
using AuraHull.AuraVRGame;
using Photon.Pun;
using UnityEngine;

public class BuildingSite : MonoBehaviour
{
    [SerializeField] private BuildPartMatcher[] partMatchers;
    [SerializeField] GameObject _objectToBecome;
    [SerializeField] Material _holoMaterial;
    [SerializeField] Material _filledMaterial;

    private int _partsBuiltCount = 0;

    void Awake()
    {
        NetworkController.OnTurbinePartBuilt += ConstructPartNetworked;
        
        for (int i = 0; i < partMatchers.Length; i++)
        {
            if (partMatchers[i] != null)
            {
                Renderer partRend = partMatchers[i].GetComponent<Renderer>();
                if (partRend != null)
                {
                    partRend.material = _holoMaterial;
                }
            }
        }
    }

    void Start()
    {
        transform.forward = PowerManager.Instance.activeWindManager.OptimalRotation(transform.position);
    }

    public void BuildPart(BuildPartMatcher matcher)
    {
        if (!ConstructPart(matcher.Index)) return;

        NetworkController.Instance.NotifyTurbinePartBuilt(gameObject.GetPhotonView().ViewID, matcher.Index);
    }

    private void ConstructPartNetworked(int actorNumber, int turbinePhotonId, int matcherIndex)
    {
        if (actorNumber == PhotonNetwork.LocalPlayer.ActorNumber) return;
        if (turbinePhotonId != gameObject.GetPhotonView().ViewID) return;
        ConstructPart(matcherIndex);
    }

    private bool ConstructPart(int matcherIndex)
    {
        BuildPartMatcher target = null;
        foreach (BuildPartMatcher m in partMatchers)
        {
            if (m.Index == matcherIndex)
            {
                target = m;
                break;
            }
        }

        if (target == null) return false;

        Renderer partRenderer = target.GetComponent<Renderer>();
        if (partRenderer == null) return false;

        target.enabled = false;
        partRenderer.material = _filledMaterial;
        
        if (++_partsBuiltCount >= partMatchers.Length)
        {
            NetworkController.Instance.NotifyTurbineBuilt(gameObject.GetPhotonView().ViewID, _objectToBecome.name);
        }

        return true;
    }
}
