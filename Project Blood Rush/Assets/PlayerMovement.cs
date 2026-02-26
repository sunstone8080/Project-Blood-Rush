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

    // Start is called before the first frame update
    void Start()
    {
        m_Controller = GetComponent<CharacterController>();
        GameManager = FindObjectOfType<GameManager>();
        m_InputHandler = GameManager.GetComponent<InputHandler>();
    }

    // Update is called once per frame
    void Update()
    {
        HandleCharacterMovement();
    }


    void HandleCharacterMovement()
    {

        transform.Rotate( new Vector3(0f, m_InputHandler.GetLookInputsHorizontal() * RotationSpeed, 0f), Space.Self);
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
