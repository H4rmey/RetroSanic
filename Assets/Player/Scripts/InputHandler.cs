using UnityEngine;
using System.Collections;

public struct Inputs
{
    public bool pUp, pDown;
    public bool pLeft, pRight;

    public bool pJump;
    public bool pInteract;


    public bool pUpPressed, pDownPressed;
    public bool pLeftPressed, pRightPressed;

    public bool pJumpPressed;
    public bool pInteractPressed;

    public int pHorizontal, pVertical;
}


public class InputHandler : MonoBehaviour
{
    public KeyCode upKey;
    public KeyCode downKey;
    public KeyCode leftKey;
    public KeyCode rightKey;
    public KeyCode jumpKey;
    public KeyCode interactKey;

    public Inputs input;

    // Update is called once per frame
    void Update()
    {
        input.pUp       = Input.GetKey(upKey);
        input.pDown     = Input.GetKey(downKey);
        input.pLeft     = Input.GetKey(leftKey);
        input.pRight    = Input.GetKey(rightKey);
        input.pJump     = Input.GetKey(jumpKey);
        input.pInteract = Input.GetKey(interactKey);
        
        input.pUpPressed        = Input.GetKeyDown(upKey);
        input.pDownPressed      = Input.GetKeyDown(downKey);
        input.pLeftPressed      = Input.GetKeyDown(leftKey);
        input.pRightPressed     = Input.GetKeyDown(rightKey);
        input.pJumpPressed      = Input.GetKeyDown(jumpKey);
        input.pInteractPressed  = Input.GetKeyDown(interactKey);

        if (Input.GetKey(upKey))            input.pVertical     = -1;
        else if (Input.GetKey(downKey))     input.pVertical     =  1;
        else                                input.pVertical     =  0;

        if (Input.GetKey(leftKey))          input.pHorizontal   = -1;
        else if (Input.GetKey(rightKey))    input.pHorizontal   =  1;
        else                                input.pHorizontal   =  0;

    }
}
