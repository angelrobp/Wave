using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainCam : MonoBehaviour
{
    //Mueve la camara. Es llamado desde el jugador pasandole su mismo movimiento para que la camara lo siga
    public void move(Vector3 movement)
    {
        transform.Translate(movement.x, movement.z, 0f);
    }
}
