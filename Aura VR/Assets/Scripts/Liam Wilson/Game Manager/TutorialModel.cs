﻿using System;
using System.Collections;
using System.Collections.Generic;
using AuraHull.AuraVRGame;
using Photon.Pun;
using UnityEngine;

public class TutorialModel : MonoBehaviour, IPunObservable
{
    private Speaker _speaker;
    private PenguinAnimationControl _animator;

    private int clientsReady = 0;

    void Start()
    {
        _speaker = GetComponent<Speaker>();
        _animator = GetComponent<PenguinAnimationControl>();

        Initialize();
    }

    public void Initialize()
    {
        _speaker.OnDialogueFinish += NetworkController.Instance.NotifyClientProgress;
        NetworkController.OnTutorialClientProgress += TutorialClientProgress;

        NetworkController.Instance.NotifyClientProgress();
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(clientsReady);
        }
        else
        {
            clientsReady = (int)stream.ReceiveNext();
        }
    }

    public void TutorialClientProgress()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            clientsReady++;
        }

        if (clientsReady >= 2)
        {
            _speaker.Speak();
            clientsReady = 0;
        }
    }

    private void Finish()
    {
        AuraGameManager.Instance.StartGameplay();

        if (PhotonNetwork.IsMasterClient)
        {
            PhotonNetwork.Destroy(gameObject);
        }
    }

    public void CheckNextTutorialCondition()
    {
        //for (int i = 0; i < specialConditions.Count; i++)
        //{
        //    if (specialConditions[i].tutorialIndex == _penguinSpeaker.currentDialogue)
        //    {
        //        if (specialConditions[i].wasTriggeredEarly)
        //        {
        //            return;
        //        }

        //        specialConditions[i].SetLive();
        //        return;
        //    }
        //}

        // Default conditions

    }
}
