using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Clase que lleva la lógica de la serpiente en sí (controlada por el jugador)
public class SnakeMovementReal : MonoBehaviour
{
    public List<Transform> BodyParts = new List<Transform>();

    public float mindistance = 50000.0f;
    public int vida = 100;
    Color orange = new Color(1.0f,0.5251f,0.0f);

    private int tipoPoder;
    private int trecarga;
    private int tduracion;
    private bool poderActivo = false;
    private bool enRecarga = false;

    public bool repelente = false;
    public bool invisible = false;

    private double lastTime;

    public int beginsize;

    public int ID = 0;

    public float speed = 350;
    public float angle_increment = 0.03f;
    public float dir_angle = 0;
    //public float rotationspeed = 50;

    public GameObject bodyprefab;

    private float dis;
    private Transform curBodyPart;
    private Transform PrevBodyPart;

    // Start is called before the first frame update
    void Start()
    {
        for(int i = 0; i < beginsize-1; i++)
        {
            AddBodyPart();
        }
    }

    // Update is called once per frame
    void Update()
    {
        updateCollider();

        updatePoder();

        updateRecarga();

        Move();
    }

    //Lleva el control de los collider, activando y desactivando para que únicamente esté activa la última vida.
    public void updateCollider()
    {
        if(BodyParts.Count > 0)
        {
            BodyParts[BodyParts.Count - 1].gameObject.GetComponent<SphereCollider>().enabled = true;

            if (BodyParts.Count > 2)
            {
                BodyParts[BodyParts.Count - 2].gameObject.GetComponent<SphereCollider>().enabled = false;
            }
        }
    }

    //Mueve la serpiente, el movimiento es definido por le jugador utilizando las flechas.
    public void Move()
    {

        //Comprueba si hemos activado el poder
        activatePower();

        float curspeed;

        // CONTROL DE PODERES
        if (tipoPoder == 0 && poderActivo) curspeed = speed * 3;
        else curspeed = speed;

        if (tipoPoder == 2 && poderActivo) poderRepelente();

        if (tipoPoder == 1 && poderActivo)
        {
            curspeed = speed * 1.8f;
            poderInvisible();
        }
        //FIN PODERES

        //Comprobamos si estamos cerca de un repelente
        alejarJugador();

        //Guardamos los valores del movimiento pulsando las flechas.
        if (Input.GetKey(KeyCode.LeftArrow))
        {
            dir_angle += angle_increment;
        }
        if (Input.GetKey(KeyCode.RightArrow))

        {
            dir_angle -= angle_increment;
        }

        //Movemos la pieza clave del jugador (cabeza)
        Vector3 position = BodyParts[0].position;
        position.x += Mathf.Cos(dir_angle) * Time.deltaTime * curspeed;
        position.y += Mathf.Sin(dir_angle) * Time.deltaTime * curspeed;
        BodyParts[0].position = position;

        //El resto del cuerpo se mueve con él
        for (int i = 1; i < BodyParts.Count; i++)
        {
            curBodyPart = BodyParts[i];
            PrevBodyPart = BodyParts[i - 1];

            dis = Vector3.Distance(PrevBodyPart.position, curBodyPart.position);

            Vector3 newpos = PrevBodyPart.position;

            newpos.z = BodyParts[0].position.z;

            float T = Time.deltaTime * dis / mindistance * curspeed;

            if (T > 0.5f)
                T = 0.5f;

            curBodyPart.position = Vector3.Slerp(curBodyPart.position, newpos, T);
        }

    }

    //Función que añade una nueva vida a la serpiente
    public void AddBodyPart()
    {
        Transform newpart = (Instantiate(bodyprefab, BodyParts[BodyParts.Count-1].position, BodyParts[BodyParts.Count - 1].rotation) as GameObject).transform;

        newpart.SetParent(transform);

        BodyParts.Add(newpart);


        BodyParts[BodyParts.Count - 1].gameObject.GetComponent<Renderer>().material.color = orange;
    }

    //Funcion que elimina la última vida de la serpiente.
    public void RemoveBodyPart()
    {
        BodyParts.RemoveAt(BodyParts.Count - 1);
        if(BodyParts.Count == 1)
        {
            Destroy(this.gameObject);
        }
    }

    //Funcion para sumar vida a una serpiente
    public void sumarVida(int n)
    {
        vida += n;
    }

    //Funcion para restar vida a una serpiente
    public void restarVida(int n)
    {
        vida -= n;
    }

    //Funcion que se utiliza al comienzo del juego para indicar el poder de la serpiente jugador
    public void setPoder(int value)
    {

        tipoPoder = value;

        switch (value)
        {
            case 0: //velocidad
                tduracion = 3;
                trecarga = 10;
                break;
            case 1: //invisibilidad
                tduracion = 7;
                trecarga = 15;
                break;
            case 2: //repelente
                tduracion = 7;
                trecarga = 20;
                break;
        }
    }

    //Devuelve los segundos que llevamos con cierta actividad (poder o recarga)
    private float getSeconds()
    {

        int seconds = 0;

        if (!poderActivo && !enRecarga) lastTime = Time.realtimeSinceStartup;
        else
        {
            seconds = (int)(Time.realtimeSinceStartup - lastTime);
        }

        return seconds;
    }

    //Comprueba si hemos pasado el limite del poder
    private void updatePoder()
    {
        float seconds = getSeconds();

        if (seconds >= tduracion && poderActivo)
        {
            poderActivo = false;
            enRecarga = true;
            lastTime = Time.realtimeSinceStartup;

            if (tipoPoder == 2)
            {
                resetRepelente();
            }
            else if (tipoPoder == 1)
            {
                resetInvisible();
            }
        }
    }

    //Comprueba si hemos pasado el limite de la recarga
    private void updateRecarga()
    {
        float seconds = getSeconds();

        if (seconds >= trecarga && enRecarga)
        {
            enRecarga = false;
            lastTime = Time.realtimeSinceStartup;
        }
    }

    //Cuando se activa el poder repelente, se llama a esta funcion para cambiar la configuración necesaria
    public void poderRepelente()
    {
        repelente = true;

        // Se pone el ultimo objeto como no colisionable para que no te puedan comer
        if (BodyParts.Count > 0)
        {
            BodyParts[BodyParts.Count - 1].gameObject.GetComponent<SphereCollider>().enabled = false;
        }

        //Se cambia el color de la serpiente para indicar el estado de repelente
        for (int i = 0; i < BodyParts.Count; i++)
        {
            BodyParts[i].gameObject.GetComponent<Renderer>().material.color = Color.blue;
        }
    }

    //Funcion que resetea a la serpiente despues de haber usado un poder repelente
    public void resetRepelente()
    {
        repelente = false;

        //Vuelve a dejar a la serpiente en estado normal
        if (BodyParts.Count > 0)
        {
            for (int i = 0; i < BodyParts.Count; i++)
            {
                BodyParts[i].gameObject.GetComponent<Renderer>().material.color = orange;
            }
            BodyParts[BodyParts.Count - 1].gameObject.GetComponent<SphereCollider>().enabled = true;
        }
    }

    //Cuando se activa el poder invisible, se llama a esta funcion para cambiar la configuración necesaria
    public void poderInvisible()
    {
        invisible = true;

        // Se pone la serpiente entera como no colisionable y se hace transparente para indicar el estado de invisible
        for (int i = 0; i < BodyParts.Count; i++)
        {
            BodyParts[i].gameObject.GetComponent<SphereCollider>().enabled = false;
            Color cn = orange;
            cn.a = 0.3f;
            BodyParts[i].gameObject.GetComponent<Renderer>().material.color = cn;
        }
    }

    //Funcion que resetea a la serpiente despues de haber usado un poder invisible
    public void resetInvisible()
    {
        invisible = false;

        //Vuelve a dejar a la serpiente en estado normal
        if (BodyParts.Count > 0)
        {
            for (int i = 0; i < BodyParts.Count; i++)
            {
                BodyParts[i].gameObject.GetComponent<Renderer>().material.color = orange;
            }
            BodyParts[BodyParts.Count - 1].gameObject.GetComponent<SphereCollider>().enabled = true;
            BodyParts[0].gameObject.GetComponent<SphereCollider>().enabled = true;
        }
    }

    //Comprueba si el usuario ha activado el poder pulsando el espacio.
    public void activatePower()
    {
        if (!enRecarga && !poderActivo)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                poderActivo = true;
            }
        }
    }

    //Funcion que aleja la serpiente de otra (repelente) automáticamente
    public void alejarJugador()
    {
        float dist = 0.0f;
        int pos = -1;

        //Se recorren todas las serpientes del mapa
        GameObject[] objs = GameObject.FindGameObjectsWithTag("snakes");

        for (int i = 0; i < objs.Length; i++)
        {
            //Se comprueba la distancia entre la serpiente jugador y el resto de serpientes.
            dist = Vector3.Distance(BodyParts[0].position, objs[i].GetComponent<SnakeMovementIA>().BodyParts[0].position);

            if (dist < objs[i].GetComponent<SnakeMovementIA>().umbralDist && objs[i].gameObject.GetComponent<SnakeMovementIA>().repelente)
            {
                pos = i;
                break;
            }
        }

        //Si efectivamente tenemos dentro de nuestro rango de vision una serpiente repelente, nos alejamos de ella automáticamente.
        if(pos >= 0)
        {
            GameObject res = objs[pos].GetComponent<SnakeMovementIA>().BodyParts[0].gameObject;

            Move(res);
        }
    }

    //Mueve a la serpiente jugador automáticamente para alejarse de una serpiente repelente.
    public void Move(GameObject dest)
    {

        activatePower();

        float curspeed;

        // PODERES
        if (tipoPoder == 0 && poderActivo) curspeed = speed * 3;
        else curspeed = speed;

        if (tipoPoder == 2 && poderActivo) poderRepelente();

        if (tipoPoder == 1 && poderActivo)
        {
            curspeed = speed * 1.8f;
            poderInvisible();
        }

        //FIN PODERES

        BodyParts[0].position = Vector3.MoveTowards(BodyParts[0].position, dest.transform.position, -1 * curspeed * Time.deltaTime);

        for (int i = 1; i < BodyParts.Count; i++)
        {
            curBodyPart = BodyParts[i];
            PrevBodyPart = BodyParts[i - 1];

            dis = Vector3.Distance(PrevBodyPart.position, curBodyPart.position);

            Vector3 newpos = PrevBodyPart.position;

            newpos.z = BodyParts[0].position.z;

            float T = Time.deltaTime * dis / mindistance * curspeed;

            if (T > 0.5f)
                T = 0.5f;

            curBodyPart.position = Vector3.Slerp(curBodyPart.position, newpos, T);
        }


    }
}