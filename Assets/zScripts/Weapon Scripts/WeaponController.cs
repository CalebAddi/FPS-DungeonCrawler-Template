using UnityEngine;

namespace DungeonCrawl.WeaponController
{    
    public class WeaponController : MonoBehaviour
    {
        #region - Weapon Settings -
//=========================================================//
        private  Quaternion origRotat;
//=========================================================//
        [Header("Current Weapon")]
        [SerializeField] public Transform currentWeapon;
//=========================================================//
        [Header("Weapon Sway")]
        [SerializeField] private float swayIntensity;
        [SerializeField] private float swaySmoothing;
//=========================================================//
        [Header("Aim Down Sights")]
        [SerializeField] private float aimSpeed;
//=========================================================//
        #endregion

        #region - Update / Start  -
        private void Start() 
        {
            origRotat = transform.localRotation;
        }
        private void FixedUpdate() 
        {
            SwayUpdate();

            if (Input.GetMouseButton(1)) {
                ADS();
            };
        }

        #endregion

        #region - SwayUpdate / ADS -
        private void SwayUpdate()
        {
            float mouseSwayX = Input.GetAxisRaw("Mouse X");
            float mouseSwayY = Input.GetAxisRaw("Mouse Y");

            //calc target rotation
            Quaternion targetAdjX = Quaternion.AngleAxis(-swayIntensity * mouseSwayX, Vector3.up);
            Quaternion targetAdjY = Quaternion.AngleAxis(swayIntensity * mouseSwayY, Vector3.right);
            Quaternion targetRotat = origRotat * targetAdjX * targetAdjY;

            //rotate towards the target rotation
            transform.localRotation = Quaternion.Lerp(transform.localRotation, targetRotat, Time.deltaTime * swaySmoothing);
        }

        private void ADS()
        {
            
        }

        #endregion
    }
}