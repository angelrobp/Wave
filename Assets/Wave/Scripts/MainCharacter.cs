using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainCharacter : MonoBehaviour
{
    private Vector2 saved_Offset;
    private MainCam mainCam = null;
    private BG_Scroll background = null;

    
    public float positionX = 0f;
    public float positionZ = 0f;

    public float moveSpeed = 10f;

    private float xAxis, zAxis;
   

    private void Start()
    {
        // Eliminar gravedad
        Physics2D.gravity = Vector2.zero;
        // Guardar camara y fondo en variables
        GameObject objeto = GameObject.Find("Background");
        background = objeto.GetComponent<BG_Scroll>();
        objeto = GameObject.Find("Main Camera");
        mainCam = objeto.GetComponent<MainCam>();
    }

    // Update is called once per frame
    void Update()
    {
        //Movimiento personaje
        xAxis = Input.GetAxis("Horizontal");
        zAxis = Input.GetAxis("Vertical");
        Vector3 movement = new Vector3(xAxis, 0, zAxis);
        positionX = xAxis*moveSpeed;
        positionZ = zAxis*moveSpeed;

        transform.Translate(movement * moveSpeed * Time.deltaTime);

        //Movimiento camara para seguir al jugador
        mainCam.move(movement * moveSpeed * Time.deltaTime);
    }
}