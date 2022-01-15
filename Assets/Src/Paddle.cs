using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Paddle : MonoBehaviour
{
    [SerializeField] private float speed = 5;
    [SerializeField] private float movementLimit = 7;
    private Vector3 targetPosition;
    private Camera cam;
    public ArkanoidController _controller;

    private Camera Camera
    {
        get
        {
            if(cam==null)
            {
                cam = Camera.main;
            }
            return cam;
        }
    }

    void Update()
    {
        targetPosition.x = Camera.ScreenToWorldPoint(Input.mousePosition).x;
        targetPosition.x = Mathf.Clamp(targetPosition.x, -movementLimit, movementLimit);
        targetPosition.y = this.transform.position.y;

        transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime*speed);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if(other.GetComponent<MultiBall>())
        {
            _controller.powerup("multiball");
            GameObject pu = other.gameObject;
            Destroy(pu);
        }

        else if(other.GetComponent<Fast>())
        {
            _controller.powerup("fast");
            GameObject pu = other.gameObject;
            Destroy(pu);
        }

        else if(other.GetComponent<Slow>())
        {
            _controller.powerup("slow");
            GameObject pu = other.gameObject;
            Destroy(pu);
        }

        else if(other.GetComponent<ExtraPoints>())
        {
            _controller.powerup("extrapoints");
            GameObject pu = other.gameObject;
            Destroy(pu);
        }
    }
}
