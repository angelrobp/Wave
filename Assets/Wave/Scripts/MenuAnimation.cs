using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuAnimation : MonoBehaviour
{
    public float moveSpeed = 10f;
    public float positionX, positionZ = 0f;

    // Update is called once per frame
    void Update()
    {
        //Movimiento del menu
        positionX = 0f;
        positionZ = moveSpeed;
        transform.Translate(0f, 0f, moveSpeed * Time.deltaTime);
    }
}
