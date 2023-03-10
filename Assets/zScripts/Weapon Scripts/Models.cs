using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Models
{
    #region - Weapons -

    [Serializable]

    public class WeaponSettingsModel
    {
        [Header("Weapon Sway")]
        public float SwayAmount;
        public bool SwayYInverted;
        public bool SwayXInverted;
        public float SwaySmoothing;
        public float SwayResetSmoothing;
        public float SwayClampX;
        public float SwayClampY;

        [Header("Weapon Movement Sway")]
        public float MovementSwayX;
        public float MovementSwayY;
        public bool MovementSwayXInverted;
        public bool MovementSwayYInverted;
        public float MovementSwaySmoothing;
    }

    #endregion

}