using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputHandler : MonoBehaviour
{
    public float lookSensitivity = 1f;

    PlayerMovement m_PlayerMovement;
    GameManager GameManager;
    PlayerMovement PlayerMovement;
    private bool upPressedLastFrame = false;
    private bool downPressedLastFrame = false;
    private bool leftPressedLastFrame = false;
    private bool rightPressedLastFrame = false;
    private void Start()
    {
        GameManager = FindObjectOfType<GameManager>();
        m_PlayerMovement = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMovement>();
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }


    public bool CanProcessInput()
    {
        if (GameManager.Instance.CurrentGameState == GameManager.GameState.Collection)
        {
            return true;
        }
        else
        {
            return false;
        }

    }
   

    public bool GetLeftMouseDown()
    {
        
      
             return Input.GetKeyDown(KeyCode.Mouse0);
       
    }

    public bool GetUpInput()
{
    if (!CanProcessInput()) return false;

    bool isPressed = Input.GetAxisRaw("Vertical") > 0f;
    bool justPressed = isPressed && !upPressedLastFrame;
    upPressedLastFrame = isPressed;
    return justPressed;
}

public bool GetDownInput()
{
    if (!CanProcessInput()) return false;

    bool isPressed = Input.GetAxisRaw("Vertical") < 0f;
    bool justPressed = isPressed && !downPressedLastFrame;
    downPressedLastFrame = isPressed;
    return justPressed;
}

public bool getLeftInput()
{
    if (!CanProcessInput()) return false;

    bool isPressed = Input.GetAxisRaw("Horizontal") < 0f;
    bool justPressed = isPressed && !leftPressedLastFrame;
    leftPressedLastFrame = isPressed;
    return justPressed;
}

public bool getRightInput()
{
    if (!CanProcessInput()) return false;

    bool isPressed = Input.GetAxisRaw("Horizontal") > 0f;
    bool justPressed = isPressed && !rightPressedLastFrame;
    rightPressedLastFrame = isPressed;
    return justPressed;
}



    public Vector3 GetMoveInput()
    {
     if (GameManager.Instance.CurrentGameState == GameManager.GameState.Collection)
    {
        return Vector3.zero; // No movement allowed
    }

    // Otherwise, normal movement
    Vector3 move = new Vector3(Input.GetAxisRaw("Horizontal"), 0f, Input.GetAxisRaw("Vertical"));
    move = Vector3.ClampMagnitude(move, 1);
    return move;
    }

    public float GetLookInputsHorizontal()
    {
        return GetMouseOrStickLookAxis("Mouse X", "Look X");
    }

    public float GetLookInputsVertical()
    {
        return GetMouseOrStickLookAxis("Mouse Y", "Look Y");

    }

    float GetMouseOrStickLookAxis(string mouseInputName, string stickInputName)
    {
        //if input for camera is avaliable run this
        bool isGamepad = Input.GetAxisRaw(stickInputName) != 0f;
        float i = isGamepad ? Input.GetAxisRaw(stickInputName) : Input.GetAxisRaw(mouseInputName);
       
        i *= lookSensitivity;
        if (isGamepad)
        {
            i *= Time.deltaTime;
        }else
        {
            i *= 0.01f;
#if UNITY_WEBGL
i*= WebglLookSensitivityMultiplier;
#endif
        }
        return i;
        
        //else return 0
        
    }
    void Update()
    {
    
    }
}
