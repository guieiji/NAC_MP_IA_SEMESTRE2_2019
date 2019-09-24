using ArdJoystick;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.Characters.FirstPerson;

public class MoverPlayerScript : MonoBehaviour
{
    // Start is called before the first frame update
    public ArdController ardControl;
    public FirstPersonController fstPerson;
    public CrouchScript crouch;

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

        if (ardControl.GetKey(ArdKeyCode.BUTTON_RIGHT))
        {
            MouseLook.cameraDir = 1;
            fstPerson.move_H = -1;
            
        }

        if (ardControl.GetKeyUp(ArdKeyCode.BUTTON_RIGHT))
        {
            MouseLook.cameraDir = 0;
            fstPerson.move_H = 0;
        }
        if (ardControl.GetKey(ArdKeyCode.BUTTON_LEFT))
        {
            MouseLook.cameraDir = -1;
            fstPerson.move_H = 1;
        }

        if (ardControl.GetKeyUp(ArdKeyCode.BUTTON_LEFT))
        {
            MouseLook.cameraDir = 0;
            fstPerson.move_H = 0;
        }

        if (ardControl.GetKey(ArdKeyCode.BUTTON_UP))
        {
            fstPerson.move_V = 1;
        }

        if (ardControl.GetKeyUp(ArdKeyCode.BUTTON_UP))
        {
            fstPerson.move_V = 0;
        }
        if (ardControl.GetKey(ArdKeyCode.BUTTON_DOWN))
        {
            fstPerson.move_V = -1;
        }

        if (ardControl.GetKeyUp(ArdKeyCode.BUTTON_DOWN))
        {
            fstPerson.move_V = 0;
        }

        fstPerson.jumpKey = ardControl.GetKeyDown(ArdKeyCode.BUTTON_B);

        fstPerson.run = ardControl.GetKey(ArdKeyCode.BUTTON_Y);

        fstPerson.slide = ardControl.GetKeyDown(ArdKeyCode.BUTTON_X);

        crouch.crouchBtn = ardControl.GetKeyDown(ArdKeyCode.BUTTON_A);


    }


}
