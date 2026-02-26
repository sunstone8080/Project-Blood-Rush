using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputHandler : MonoBehaviour
{
    public float lookSensitivity = 1f;

    PlayerMovement m_PlayerMovement;
    GameManager GameManager;
    PlayerMovement PlayerMovement;
 
    private void Start()
    {
        GameManager = FindObjectOfType<GameManager>();
        m_PlayerMovement = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMovement>();
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    //add "canprocessinput" 
    //add bool function that checks what game state the player is in, at bar, in dialogue, free roam

    public Vector3 GetMoveInput()
    {
        //if input for wasd can be processed run this
        Vector3 move = new Vector3(Input.GetAxisRaw("Horizontal"), 0f, Input.GetAxisRaw("Vertical"));
        move = Vector3.ClampMagnitude(move, 1);


        return move;
        // else return vector3 zero
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
