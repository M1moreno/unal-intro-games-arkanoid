using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Slow : MonoBehaviour
{
    private float _speed = 3;

    void Update()
    {
        transform.Translate(new Vector2(0f, -1f) * Time.deltaTime * _speed);
        if(transform.position.y < -6)
        {
            Destroy(gameObject);
        }
    }
}
