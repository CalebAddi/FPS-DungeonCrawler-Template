using System.Xml.Schema;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BreathingSway : MonoBehaviour
{
    [Header("Weapon Breathing")]
    [SerializeField] public Transform breathingSwayObject;
    [SerializeField] private float swayAmountA;
    [SerializeField] private float swayAmountB;
    [SerializeField] private float swayScale;
    [SerializeField] private float swayLerpSpeed = 10;
    private float swayTime;
    public Vector3 swayPosition;
//==========================================================//

    void Update()
    {
        CalcWeaponBreathing();
    }

    private void CalcWeaponBreathing()
    {
        var targetPos = LissajousCurve(swayTime, swayAmountA, swayAmountB) / swayScale;

        swayPosition = Vector3.Lerp(swayPosition, targetPos, Time.smoothDeltaTime * swayLerpSpeed);
        swayTime += Time.deltaTime;

        if (swayTime > 6.3f) {
            swayTime = 0;
        }

        breathingSwayObject.localPosition = swayPosition;
    }

    private Vector3 LissajousCurve(float Time, float A, float B) 
    {
        return new Vector3(Mathf.Sin(Time), A * Mathf.Sin(B * Time + Mathf.PI));
    }
}
