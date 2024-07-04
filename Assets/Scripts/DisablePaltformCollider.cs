using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisablePaltformCollider : MonoBehaviour
{
    public PlatformEffector2D PF;
    [SerializeField]
    private float WaitTime;
    private float Compteur;
    private bool pressed=false;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.DownArrow) || Input.GetKey(KeyCode.S))
        {
            PF.rotationalOffset = 180f;
            Compteur = WaitTime;
            pressed = true;
        }
        if (pressed)
        {
            if (Compteur <= 0)
            {
                PF.rotationalOffset = 0f;
                pressed = false;
            }
            else
            {
                Compteur -= Time.deltaTime;
            }
        }

    }
}
