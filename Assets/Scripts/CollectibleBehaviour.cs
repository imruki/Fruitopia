using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollectibleBehaviour : MonoBehaviour
{
    public GameObject CollectedAnimation;
  
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            Instantiate(CollectedAnimation, transform.position, Quaternion.identity);
            Destroy(gameObject);
        }
    }
}
