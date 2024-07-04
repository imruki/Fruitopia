using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingObstacle : MonoBehaviour
{
    public enum Direction{Horizontal,Vertical};
    public Direction ChosenDirection;
    public float MoveSpeed;
    public float ExtremePosition;
    public float BackwardExtremePosition;
    private bool MoveRight = true;
    private bool MoveUp = true;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (ChosenDirection == Direction.Horizontal )
        {
            if (transform.position.x > ExtremePosition) MoveRight = false;
            if (transform.position.x < BackwardExtremePosition) MoveRight = true;

            if (MoveRight) transform.position = new Vector2(transform.position.x + MoveSpeed * Time.deltaTime, transform.position.y);
            else transform.position = new Vector2(transform.position.x - MoveSpeed * Time.deltaTime, transform.position.y);
        }

        else
        {
            if (transform.position.y > ExtremePosition) MoveUp = false;
            if (transform.position.y < BackwardExtremePosition) MoveUp = true;

            if (MoveUp) transform.position = new Vector2(transform.position.x , transform.position.y + MoveSpeed * Time.deltaTime);
            else transform.position = new Vector2(transform.position.x , transform.position.y - MoveSpeed * Time.deltaTime);
        }


    }
}
