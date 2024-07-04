using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundBehaviour : MonoBehaviour
{
    [SerializeField] private Vector2 parallaxEffectMultiplier;
    private Vector3 firstPosition;
    private float textureUnitSizeX;
    private float textureUnitSizeY;

    private void Start()
    {
        firstPosition = transform.position;
        Sprite sprite = GetComponent<SpriteRenderer>().sprite;
        Texture2D texture = sprite.texture;
        textureUnitSizeX = texture.width / sprite.pixelsPerUnit;
        textureUnitSizeY = texture.height / sprite.pixelsPerUnit;
        Debug.Log(transform.position);
        
        
    }

    private void FixedUpdate()
    {
        transform.position = new Vector3(transform.position.x + parallaxEffectMultiplier.x, transform.position.y + parallaxEffectMultiplier.y);
        Debug.Log("new"+ transform.position);
        Debug.Log("difference" + (firstPosition.x - transform.position.x));
        
        if (Mathf.Abs(firstPosition.x - transform.position.x) >= textureUnitSizeX*3)
        {
            float offsetPositionX = (firstPosition.x - transform.position.x) % textureUnitSizeX;
            transform.position = new Vector3(firstPosition.x + offsetPositionX, transform.position.y);
            Debug.Log(transform.position);
        }

        if (Mathf.Abs(firstPosition.y - transform.position.y) >= textureUnitSizeY*3)
        {
            float offsetPositionY = (firstPosition.y - transform.position.y) % textureUnitSizeY;
            transform.position = new Vector3(transform.position.x, firstPosition.y + offsetPositionY);
        }
    }
}