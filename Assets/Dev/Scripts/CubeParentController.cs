using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubeParentController : MonoBehaviour
{
    [SerializeField] float speed = 80;


    private void LateUpdate()
    {
        if (Input.GetKey(KeyCode.A))
        {
            transform.Rotate(Vector3.up, -speed * Time.deltaTime);
        }
        else if(Input.GetKey(KeyCode.D))
        {
            transform.Rotate(Vector3.up, speed * Time.deltaTime);
        }
    }
}
