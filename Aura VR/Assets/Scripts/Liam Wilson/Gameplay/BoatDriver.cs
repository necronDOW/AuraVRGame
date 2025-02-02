﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRTK.Controllables.ArtificialBased;
using VRTK.Controllables.PhysicsBased;

public class BoatDriver : MonoBehaviour
{
    [SerializeField] private VRTK_PhysicsRotator throttleControl;
    [SerializeField] private VRTK_ArtificialRotator directionControl;
    [SerializeField] private Transform target;
    [SerializeField] private float enginePower = 1.0f;
    [SerializeField] private float steeringPower = 1.0f;
    [SerializeField] private bool allowReverse = true;
    [SerializeField] private AudioSource engineAudio;
    [SerializeField] private PowerConsumer powerConsumer;

    public float realThrottleZero { get; private set; } = 0.0f;
    public float realThrottleMax { get; private set; } = float.MaxValue;

    public float Throttle
    {
        get
        {
            if (throttleControl == null) return 0.0f;
            return (throttleControl.GetStepValue(throttleControl.GetValue()) - realThrottleZero) / realThrottleMax;
        }
    }

    public float Direction
    {
        get
        {
            if (directionControl == null) return 0.0f;
            return directionControl.GetValue() / directionControl.angleLimits.maximum;
        }
    }

    void Start()
    {
        if (throttleControl != null)
        {
            realThrottleZero = (allowReverse ? (throttleControl.stepValueRange.maximum / 2.0f) : 0.0f);
            realThrottleMax = (allowReverse ? realThrottleZero : throttleControl.stepValueRange.maximum);

            throttleControl.SetAngleTargetWithStepValue(realThrottleZero);
        }
    }

    void Update()
    {
        float throttle = Throttle;

        Transform applyTo = (target == null) ? transform : target;
        applyTo.position += applyTo.forward * throttle * enginePower;
        applyTo.Rotate(transform.up, Direction * Throttle * steeringPower);

        if (engineAudio != null)
        {
            engineAudio.volume = Mathf.Abs(throttle);
        }

        if (powerConsumer != null)
        {
            powerConsumer.consumptionFactor = Mathf.Abs(throttle);
        }
    }

    public void ForceStopThrottle()
    {
        throttleControl.SetValue(throttleControl.GetAngleFromStepValue(realThrottleZero));
    }
}
