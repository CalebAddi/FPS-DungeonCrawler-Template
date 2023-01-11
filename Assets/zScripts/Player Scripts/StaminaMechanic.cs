using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StaminaMechanic : MonoBehaviour
{
    private PlayerController controller;
//===========================================================//
    [Header("Stamina Controller Settings")]
    public float playerStamina = 100f;
    [SerializeField] private float maxStamina = 100f;
    [SerializeField] private float jumpCost;
    [HideInInspector] public bool hasRegenerated = true;
    [HideInInspector] public bool areSprinting = false;
//============================================================//
    [Header("Stamina Regeneration Settings")]
    [Range(0, 50)] [SerializeField] private float staminaDrain;
    [Range(0, 50)] [SerializeField] private float staminaRegen;
//============================================================//
    [Header("Stamina Speed Parameters")]
    [SerializeField] private int slowedRunSpeed;
    [SerializeField] private int normalRunSpeed;
//============================================================//
    [Header("Stamina UI Elements")]
    [SerializeField] private Image staminaBar = null;
    [SerializeField] private CanvasGroup sliderCanvas = null;
//============================================================//

    private void Awake() 
    {
        controller = GetComponent<PlayerController>();
    }

    private void Update() 
    {
        StaminaChecks();
    }

    private void StaminaChecks()
    {
        if (!areSprinting)
        {

            if (playerStamina <= maxStamina - 0.01)
            {
                playerStamina += staminaRegen * Time.deltaTime;
                UpdateStamina(1);

                if (playerStamina >= maxStamina)
                {
                    //Set to normal speed
                    sliderCanvas.alpha = 0;
                    hasRegenerated = true;
                }
            }
        }
    }

    public void SprintingStamina()
    {
        if (hasRegenerated) {
            areSprinting = true;
            playerStamina -= staminaDrain * Time.deltaTime;
            UpdateStamina(1);

            if (playerStamina <= 0) {
                hasRegenerated = false;
                //Slow our player
                sliderCanvas.alpha = 0;
            }
        }
    }

    public void StaminaJump()
    {
        if (playerStamina >= (maxStamina * jumpCost / maxStamina)) {
            playerStamina -= jumpCost;
            
            // controller.Jump(); //allow the player to jump
            UpdateStamina(1);
        }
    }

    private void UpdateStamina(int value)
    {
        staminaBar.fillAmount = playerStamina / maxStamina;

        if (value == 0) sliderCanvas.alpha = 0;
        else sliderCanvas.alpha = 1;
    }
}
