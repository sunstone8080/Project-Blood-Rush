using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public Camera PlayerCamera;

    public float Speed = 5f;
    public float MovementSharpness = 10f;
    public float RotationSpeed = 150f;
    public float Gravity = 9.81f;

    public Vector3 CharacterVelocity;
    GameManager GameManager;
    InputHandler m_InputHandler;
    CharacterController m_Controller;

    Vector3 m_GroundNormal;
    private Vector3 M_GroundNormal = Vector3.up;
    float m_CameraVerticalAngle = 0f;

    // ===== BAR SYSTEM =====
    [SerializeField] private Transform barStandPoint;
    [SerializeField] private Transform barCameraPoint;

    private bool isTransitioning = false;
    private bool isExitingBar = false;

    private float transitionTime = 1.5f;
    private float transitionProgress = 0f;

    private Vector3 startPos;
    private Quaternion startRot;
    private Quaternion startCamRot;

    private Vector3 exitStartPos;
    private Quaternion exitStartRot;
    private Quaternion exitStartCamRot;

    private Vector3 freeRoamReturnPosition;
    private Quaternion freeRoamReturnRotation;
    private Quaternion freeRoamReturnCamRotation;

    void Start()
    {
        m_Controller = GetComponent<CharacterController>();
        GameManager = FindObjectOfType<GameManager>();
        m_InputHandler = GameManager.GetComponent<InputHandler>();
    }

    void Update()
    {
        // ENTER transition
        if (isTransitioning)
        {
            HandleBarTransition();
            return;
        }

        // EXIT transition
        if (isExitingBar)
        {
            HandleExitBarTransition();
            return;
        }

        // EXIT INPUT
        if (GameManager.Instance.CurrentGameState == GameManager.GameState.Bar)
        {
            if (m_InputHandler != null && m_InputHandler.GetInteractPressed())
            {
                if (DialogueUI.Instance.IsTyping())
                {
                    DialogueUI.Instance.Skip();
                }
                else
                {
                   // DialogueManager.Instance.Next();
                }
            }

            if (m_InputHandler != null && m_InputHandler.GetExitPressed())
            {
                DialogueUI.Instance.Hide();
                ExitBarState();
            }

            return;
        }

        // NORMAL MOVEMENT
        if (GameManager.Instance.CurrentGameState == GameManager.GameState.FreeRoam)
        {
            HandleCharacterMovement();
        }
    }

    // ===== ENTER BAR =====
    public void EnterBarState()
    {
        // Save return position
        freeRoamReturnPosition = transform.position;
        freeRoamReturnRotation = transform.rotation;
        freeRoamReturnCamRotation = PlayerCamera.transform.rotation;

        // Start transition
        startPos = transform.position;
        startRot = transform.rotation;
        startCamRot = PlayerCamera.transform.rotation;

        transitionProgress = 0f;
        isTransitioning = true;
    }

    void HandleBarTransition()
    {
        transitionProgress += Time.deltaTime / transitionTime;
        float t = Mathf.SmoothStep(0f, 1f, transitionProgress);

        transform.position = Vector3.Lerp(startPos, barStandPoint.position, t);
        transform.rotation = Quaternion.Lerp(startRot, barStandPoint.rotation, t);

        PlayerCamera.transform.rotation = Quaternion.Lerp(startCamRot, barCameraPoint.rotation, t);

        if (transitionProgress >= 1f)
        {
            isTransitioning = false;
            GameManager.Instance.CurrentGameState = GameManager.GameState.Bar;
        }
    }

    // ===== EXIT BAR =====
    public void ExitBarState()
    {
        exitStartPos = transform.position;
        exitStartRot = transform.rotation;
        exitStartCamRot = PlayerCamera.transform.rotation;

        transitionProgress = 0f;
        isExitingBar = true;
    }

    void HandleExitBarTransition()
    {
        transitionProgress += Time.deltaTime / transitionTime;
        float t = Mathf.SmoothStep(0f, 1f, transitionProgress);

        transform.position = Vector3.Lerp(exitStartPos, freeRoamReturnPosition, t);
        transform.rotation = Quaternion.Lerp(exitStartRot, freeRoamReturnRotation, t);

        PlayerCamera.transform.rotation = Quaternion.Lerp(exitStartCamRot, freeRoamReturnCamRotation, t);

        if (transitionProgress >= 1f)
        {
            isExitingBar = false; // mark exit transition done
            GameManager.Instance.CurrentGameState = GameManager.GameState.FreeRoam; //  restore movement

            //    DialogueManager.Instance.StartDialogue("WEREWOLF_START");
        }
    }

    // ===== MOVEMENT =====
    void HandleCharacterMovement()
    {
        transform.Rotate(new Vector3(0f, m_InputHandler.GetLookInputsHorizontal() * RotationSpeed, 0f), Space.Self);

        m_CameraVerticalAngle += m_InputHandler.GetLookInputsVertical() * RotationSpeed;
        m_CameraVerticalAngle = Mathf.Clamp(m_CameraVerticalAngle, -89f, 89f);
        PlayerCamera.transform.localEulerAngles = new Vector3(m_CameraVerticalAngle, 0, 0);

        Vector3 worldspaceMoveInput = transform.TransformVector(m_InputHandler.GetMoveInput());
        Vector3 targetVelocity = worldspaceMoveInput * Speed;

        targetVelocity = GetDirectionReorientedOnSlope(targetVelocity, M_GroundNormal) * targetVelocity.magnitude;

        CharacterVelocity = Vector3.Lerp(CharacterVelocity, targetVelocity, MovementSharpness * Time.deltaTime);
        CharacterVelocity += Vector3.down * Gravity * Time.deltaTime;

        m_Controller.Move(CharacterVelocity * Time.deltaTime);

        if (m_Controller.isGrounded)
        {
            RaycastHit hit;
            if (Physics.Raycast(transform.position, Vector3.down, out hit, 1.5f))
                m_GroundNormal = hit.normal;
            else
                m_GroundNormal = Vector3.up;
        }
    }

    public Vector3 GetDirectionReorientedOnSlope(Vector3 direction, Vector3 slopeNormal)
    {
        Vector3 directionRight = Vector3.Cross(direction, transform.up);
        return Vector3.Cross(slopeNormal, directionRight).normalized;
    }
}