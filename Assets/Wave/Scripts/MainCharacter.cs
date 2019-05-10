using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainCharacter : MonoBehaviour
{
    private Vector2 saved_Offset;
    private MainCam mainCam = null;
    private BG_Scroll background = null;

    private GameObject pausa;
    
    public float positionX, positionZ, positionXAntiguo, positionZAntiguo;

    public float moveSpeed = 10f;

    private float xAxis, zAxis;
    private Vector3 mousePosition;

    private bool pausaActiva;
    private void Start()
    {
        // Eliminar gravedad
        Physics2D.gravity = Vector2.zero;
        // Guardar camara y fondo en variables
        GameObject objeto = GameObject.Find("Background");
        background = objeto.GetComponent<BG_Scroll>();
        objeto = GameObject.Find("Main Camera");
        mainCam = objeto.GetComponent<MainCam>();

        positionX = positionZ = positionXAntiguo = positionZAntiguo = 0f;
        mousePosition = new Vector3(0f,0f,0f);

        //Creación menu pausa
        pausa = GameObject.Find("Menu Pausa");
        pausaActiva = false;
        pausa.SetActive(pausaActiva);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (pausaActiva)
            {
                pausaActiva = false;

            }
            else
            {
                pausaActiva = true;
            }
            pausa.SetActive(pausaActiva);
        }

        mousePosition = Input.mousePosition;
        print("Mouse: " + mousePosition);
        //print("Personaje: "+ transform.position);
        //Movimiento personaje
        xAxis = Input.GetAxis("Horizontal");
        zAxis = Input.GetAxis("Vertical");

        positionX = xAxis * moveSpeed;
        positionZ = zAxis * moveSpeed;

        
        Vector3 movement;

        if ((positionX < moveSpeed && positionX > -moveSpeed) && (positionZ < moveSpeed && positionZ > -moveSpeed))
        {
            positionX = positionXAntiguo;
            positionZ = positionZAntiguo ;
            movement = new Vector3(positionX, 0, positionZ);
        }
        else
        {
            movement = new Vector3(positionX, 0, positionZ);
            positionXAntiguo = positionX;
            positionZAntiguo = positionZ;
        }
        
        

        transform.Translate(movement * Time.deltaTime);

        //Movimiento camara para seguir al jugador
        mainCam.move(movement * Time.deltaTime);

        
    }
}