using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpikeChainRotate : MonoBehaviour
{
    public enum RotationTypeEnum { RotateAllAround, RotateWithExtremes}
    public RotationTypeEnum RotationType;
    public float RotationSpeed;
    public float RotationExtreme;
    private bool RotatingRight = true;
    void Update()
    {
        if (RotationType == RotationTypeEnum.RotateAllAround)
        {
            transform.Rotate(0f, 0f, RotationSpeed);
        }
        else
        {
            float angle = transform.localEulerAngles.z;
            angle = (angle > 180) ? angle - 360 : angle;

            if (angle > RotationExtreme && RotatingRight)
            { 
                RotatingRight = false;
            }
            if (angle < -RotationExtreme && !RotatingRight)
            {
                RotatingRight = true;
            }
            if (RotatingRight)
            {
                transform.Rotate(0f, 0f, RotationSpeed);
                
            }
            else
            {
                transform.Rotate(0f, 0f, -RotationSpeed);
            }

        }
    }
}
