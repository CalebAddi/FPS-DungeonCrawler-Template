using System.Collections;
using UnityEngine;
using DungeonCrawl.WeaponController;

public class PlayerController_fps : MonoBehaviour
{
    #region - Player Settings -
    public bool CanMove { get; private set; } = true;
    private bool IsSprinting => canSprint && Input.GetKey(sprintKey);
    private bool StartCrouching => Input.GetKeyDown(crouchKey) && !duringCrouchAnim && controller.isGrounded;
    private bool ShouldJump => Input.GetKeyDown(jumpKey) && controller.isGrounded;
    private bool IsSliding { get {
        if (controller.isGrounded && Physics.Raycast(transform.position, Vector3.down, out RaycastHit slopeHit, 2f)) {
            hitPointNorm = slopeHit.normal;
            return Vector3.Angle(hitPointNorm, Vector3.up) > controller.slopeLimit;
        }
        else return false;
    } }
//================================================================//
    private bool isCrouching;
    private bool canSprint = true;
    private bool canCrouch = true;
    private bool canJump = true;
    private bool canHeadBob = true;
    private bool canSlideSlope = true;
    private bool canInteract = true;
    private bool canFootsteps = true;
//================================================================//
    [SerializeField] private Transform cameraHolder;
    private Camera playerCam;
    private CharacterController controller;
    private Vector3 move;
    private Vector2 currentInput;
    private Vector3 hitPointNorm;
//================================================================//
    [Header("Movement Settings")]
    [SerializeField] private float walkSpeed;
    [SerializeField] private float sprintSpeed;
    [SerializeField] private float crouchSpeed;
    [SerializeField] private float slopeSpeed = 7;
    private KeyCode sprintKey = KeyCode.LeftShift;
//================================================================//
    [Header("Crouch Params")]
    [SerializeField] private float crouchHeight;
    [SerializeField] private float standHeight;
    [SerializeField] private float timeToCrouch;
    private Vector3 crouchingCenter = new Vector3(0, 0.5f, 0);
    private Vector3 standCenter = new Vector3(0, 0, 0);
    private KeyCode crouchKey = KeyCode.LeftControl;
    private bool duringCrouchAnim;
//================================================================//
    [Header("Jump Settings")]
    [SerializeField] private float jumpHeight;
    [SerializeField] private float gravity;
    private KeyCode jumpKey = KeyCode.Space;
//================================================================//
    [Header("Mouse Settings")]
    [SerializeField, Range(1, 10)] private float xSensitivity;
    [SerializeField, Range(1, 10)] private float ySensitivity;
    [Range(1, 90)] private float upperLookLimit = 70f;
    [Range(1, 90)] private float lowerLookLimit = 60f;
    private float xRotation;
//================================================================//
    [Header("Interaction Settings")]
    [SerializeField] private Vector3 interactRayPoint = default;
    [SerializeField] private float interactDistance = default;
    [SerializeField] private LayerMask interactLayer = default;
    private KeyCode interactKey = KeyCode.E;
    private Interaction currentInteraction;
//================================================================//
    [Header("HeadBob Settings")]
    [SerializeField] private float walkBobbing;
    [SerializeField] private float walkBobAmount;
    [SerializeField] private float sprintBobbing;
    [SerializeField] private float sprintBobAmount;
    [SerializeField] private float crouchBobbing;
    [SerializeField] private float crouchBobAmount;
    private float defaultYPos = 0;
    private float timer;
//================================================================//
    [Header("Footstep Params")]
    [SerializeField] private float baseStepSpeed;
    [SerializeField] private float sprintStepMultiplier;
    [SerializeField] private float crouchStepMultiplier;
    [SerializeField] private AudioSource footStepAudio = default;
    [SerializeField] private AudioClip[] woodClips = default;
    [SerializeField] private AudioClip[] stoneClips = default;
    [SerializeField] private AudioClip[] metalClips = default;
    [SerializeField] private AudioClip[] groundClips = default;
    [SerializeField] private AudioClip[] tileClips = default;
    [SerializeField] private AudioClip[] gravelClips = default;
    private float stepTimer = 0;
    private float GetCurrentOffset => isCrouching ? baseStepSpeed * crouchStepMultiplier : IsSprinting ? baseStepSpeed * sprintStepMultiplier : baseStepSpeed;
//================================================================//
    [HideInInspector] public StaminaMechanic staminaController;
    [Header("Weapon")]
    public WeaponController currentWeapon;
//================================================================//
    #endregion

    #region - Awake / FixedUpdate -
    private void Awake() 
    {
        staminaController = GetComponent<StaminaMechanic>();
        playerCam = GetComponentInChildren<Camera>();
        controller = GetComponent<CharacterController>();
        defaultYPos = playerCam.transform.localPosition.y;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void FixedUpdate() 
    {
        if (CanMove) {
            MoveInput();
            MouseLook();
            Gravity();

            if (canJump) {
                Jump();
            }

            if (canCrouch) {
                Crouching();
            }

            if (canHeadBob) {
                HeadBob();
            }

            if (canFootsteps) {
                Footsteps();
            }

            if (canInteract) {
                InteractCheck();
                InteractInput();
            }

            ApplyFinalMove();
        }

    }

    #endregion

    #region - Move / Mouse Inputs / Footsteps / ApplyFinalMove -

    private void MoveInput()
    {
        currentInput = new Vector2((isCrouching ? crouchSpeed : IsSprinting ? sprintSpeed : walkSpeed) * Input.GetAxis("Vertical"), walkSpeed * Input.GetAxis("Horizontal"));
        float moveY = move.y;
        move = (transform.TransformDirection(Vector3.forward) * currentInput.x) + (transform.TransformDirection(Vector3.right) * currentInput.y);
        move.y = moveY;
    }

    private void MouseLook()
    {
        xRotation -= Input.GetAxisRaw("Mouse Y") * ySensitivity;
        xRotation = Mathf.Clamp(xRotation, -upperLookLimit, lowerLookLimit);
        cameraHolder.transform.localRotation = Quaternion.Euler(xRotation, 0, 0);
        transform.rotation *= Quaternion.Euler(0, Input.GetAxisRaw("Mouse X") * xSensitivity, 0);
    }

    private void Footsteps()
    {
        if (!controller.isGrounded) return;
        if (currentInput == Vector2.zero) return;

        stepTimer -= Time.deltaTime;

        if (stepTimer <= 0) {

            if (Physics.Raycast(playerCam.transform.position, Vector3.down, out RaycastHit hit, 10)) {
                switch (hit.collider.tag) {
                    case "Footsteps/Wood": 
                        footStepAudio.PlayOneShot(woodClips[Random.Range(0, woodClips.Length - 1)]);
                    break;

                    case "Footsteps/Stone": 
                        footStepAudio.PlayOneShot(stoneClips[Random.Range(0, stoneClips.Length - 1)]);
                    break;

                    case "Footsteps/Metal":
                        footStepAudio.PlayOneShot(metalClips[Random.Range(0, metalClips.Length - 1)]);
                    break;

                    case "Footsteps/Tile":
                        footStepAudio.PlayOneShot(tileClips[Random.Range(0, tileClips.Length - 1)]);
                        break;

                    case "Footsteps/Gravel":
                        footStepAudio.PlayOneShot(gravelClips[Random.Range(0, gravelClips.Length - 1)]);
                        break;

                    case "Footsteps/Ground":
                        footStepAudio.PlayOneShot(groundClips[Random.Range(0, groundClips.Length - 1)]);
                    break;

                    default:
                        footStepAudio.PlayOneShot(groundClips[Random.Range(0, groundClips.Length - 1)]);
                    break;
                }
            }

            stepTimer = GetCurrentOffset;
        }
    }

    private void ApplyFinalMove()
    {
        if (canSlideSlope && IsSliding) {
            move += new Vector3(hitPointNorm.x, -hitPointNorm.y, hitPointNorm.z) * slopeSpeed;
        }

        controller.Move(move * Time.deltaTime);
    }

    #endregion

    #region - Jump / Gravity -

    private void Jump()
    {
        if (ShouldJump) {
            move.y = jumpHeight;
        }
    }

    private void Gravity()
    {
        if (!controller.isGrounded) {
            move.y -= gravity * Time.deltaTime;
        }
    }

    #endregion

    #region - Crouching / HeadBob -
    private void Crouching()
    {
        if (StartCrouching)
        {
            StartCoroutine(CrouchStand());
        }
    }

    private IEnumerator CrouchStand()
    {
        if (isCrouching && Physics.Raycast(playerCam.transform.position, Vector3.up, 1f))
        {
            yield break;
        }

        duringCrouchAnim = true;

        float timeElapsed = 0;
        float targetHeight = isCrouching ? standHeight : crouchHeight;
        float currentHeight = controller.height;
        Vector3 targetCenter = isCrouching ? standCenter : crouchingCenter;
        Vector3 currentCenter = controller.center;

        while (timeElapsed < timeToCrouch)
        {
            controller.height = Mathf.Lerp(currentHeight, targetHeight, timeElapsed / timeToCrouch);
            controller.center = Vector3.Lerp(currentCenter, targetCenter, timeElapsed / timeToCrouch);
            timeElapsed += Time.deltaTime;
            yield return null;
        }

        controller.height = targetHeight;
        controller.center = targetCenter;

        isCrouching = !isCrouching;

        duringCrouchAnim = false;
    }

    private void HeadBob()
    {
        if (!controller.isGrounded) return;

        if (Mathf.Abs(move.x) > 0.1f || Mathf.Abs(move.z) > 0.1f) {

            timer += Time.deltaTime * (isCrouching ? crouchBobbing : IsSprinting ? sprintBobbing : walkBobbing);

            playerCam.transform.localPosition = new Vector3(
                playerCam.transform.localPosition.x, 
                defaultYPos + Mathf.Sin(timer) * (isCrouching ? crouchBobAmount : IsSprinting ? sprintBobAmount : walkBobAmount),
                playerCam.transform.localPosition.z
            );
        }
    }

    #endregion

    #region - InteractCheck / InteractInput -
    private void InteractCheck()
    {
        if (Physics.Raycast(playerCam.ViewportPointToRay(interactRayPoint), out RaycastHit hit, interactDistance)) {

            if (hit.collider.gameObject.layer == 7 && (currentInteraction == null || hit.collider.gameObject.GetInstanceID() != currentInteraction.GetInstanceID())) {

                hit.collider.TryGetComponent(out currentInteraction);

                if (currentInteraction) currentInteraction.OnFocus();
            }

        } else if (currentInteraction) {

            currentInteraction.OnLoseFocus();
            currentInteraction = null;

        }
    }

    private void InteractInput()
    {
        if (Input.GetKeyDown(interactKey) && currentInteraction != null 
        && Physics.Raycast(playerCam.ViewportPointToRay(interactRayPoint), 
        out RaycastHit hit, interactDistance, interactLayer)) {

            currentInteraction.OnInteract();

        }
    }

    #endregion
}