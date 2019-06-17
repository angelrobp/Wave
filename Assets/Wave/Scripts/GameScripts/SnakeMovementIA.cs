using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Clase que lleva el control de las serpientes controladas por la IA
public class SnakeMovementIA : MonoBehaviour
{

    public List<Transform> BodyParts = new List<Transform>();

    public float mindistance = 50000.0f;
    public int vida = 100;

    private int tipoPoder;
    private int trecarga;
    private int tduracion;
    private bool poderActivo = false;
    private bool enRecarga = false;
    private bool cuentoSegundos = true;

    public bool repelente = false;
    public bool invisible = false;
    private float ataqueInvisible;
    private bool generateRandom = false;

    private bool dcontraria = false;

    private double lastTime;
    private double lastTime2;

    public static int contador;
    public int ID;

    public int beginsize;

    public float speed = 350;
    public float angle_increment = 0.03f;
    public float dir_angle = 0;
    //public float rotationspeed = 50;

    public GameObject bodyprefab;

    private float dis;
    private Transform curBodyPart;
    private Transform PrevBodyPart;

    public float umbralDist = 100.0f;
    private GameObject god;

    private Color originalColor;
    private int danio = 150;

    // Start is called before the first frame update
    void Start()
    {

        ID = SnakeMovementIA.GetID();
        for (int i = 0; i < beginsize - 1; i++)
        {
            AddBodyPart();
        }
    }

    // Update is called once per frame
    void Update()
    {

        updateCollider();

        updateNube();

        updatePoder();

        updateRecarga();

        int estado = nuevoEstado();

        god = selDest(estado);

        Move(god);

    }

    //comprueba si estamos en la nube
    private void updateNube()
    {
        if (estoyCirculo())
        {
            int seconds = cuantosSegundos();

            if (seconds > 1)
            {
                this.restarVida((int)(this.danio / (Game.getTimer_Static() / 60f)));
                cuentoSegundos = true;
            }
        }
        else cuentoSegundos = true;
    }

    private int cuantosSegundos()
    {

        int seconds = 0;
        if (cuentoSegundos)
        {
            lastTime2 = Time.realtimeSinceStartup;
            cuentoSegundos = false;
        }
        else
        {
            seconds = (int)(Time.realtimeSinceStartup - lastTime2);
        }

        return seconds;
    }

    //comprueba si estamos en la nube
    private bool estoyCirculo()
    {
        GameObject nube = GameObject.FindGameObjectWithTag("circle");

        return DamageCircle.IsOutsideCircle_Static(this.BodyParts[0].position);
    }

    //Comprueba si la cabeza del emigo esta cerca de tu propia cola
    private bool enemigoCercano(float umbral)
    {
        float dist = 0.0f;
        float min = Mathf.Infinity;
        int pos = 0;
        List<bool> res = new List<bool>();

        GameObject[] objs1 = GameObject.FindGameObjectsWithTag("snakes");
        GameObject objs2 = GameObject.FindGameObjectWithTag("snakep");

        //Recorre todas las serpientes controladas por la IA en el mapa
        for (int i = 0; i < objs1.Length; i++)
        {

            if (!objs1[i].GetComponent<SnakeMovementIA>().invisible)
            {
                List<Transform> aux1 = objs1[i].GetComponent<SnakeMovementIA>().BodyParts;

                //Comprobamos la distancia entre si cabeza y nuestra cola
                dist = Vector3.Distance(BodyParts[BodyParts.Count - 1].position, aux1[0].position);

                if (dist < min && ID != objs1[i].GetComponent<SnakeMovementIA>().ID)
                {
                    min = dist;
                    pos = i;
                }
            }
        }

        //Comprobamos lo mismo con el jugador
        if (objs2 != null && !objs2.GetComponent<SnakeMovementReal>().invisible)
        {
            List<Transform> aux2 = objs2.GetComponent<SnakeMovementReal>().BodyParts;

            dist = Vector3.Distance(BodyParts[BodyParts.Count-1].position, aux2[0].position);

            if (dist < min && ID != objs2.GetComponent<SnakeMovementReal>().ID)
            {
                min = dist;
            }
        }

        if (min >= umbral)
        {
            return false;
        }
        return true;
    }

    //Comprueba si la cola del emigo esta cerca de tu propia cabeza
    private bool enemigoCercanoAt(float umbral)
    {
        float dist = 0.0f;
        float min = Mathf.Infinity;
        int pos = 0;
        List<bool> res = new List<bool>();

        GameObject[] objs1 = GameObject.FindGameObjectsWithTag("snakes");
        GameObject objs2 = GameObject.FindGameObjectWithTag("snakep");

        for (int i = 0; i < objs1.Length; i++)
        {

            if (objs1[i] != null && !objs1[i].GetComponent<SnakeMovementIA>().invisible)
            {
                List<Transform> aux1 = objs1[i].GetComponent<SnakeMovementIA>().BodyParts;

                if (aux1 != null && BodyParts.Count > 0)
                {
                    dist = Vector3.Distance(BodyParts[0].position, aux1[aux1.Count - 1].position);

                    if (dist < min && ID != objs1[i].GetComponent<SnakeMovementIA>().ID)
                    {
                        min = dist;
                        pos = i;
                    }
                }
            }
        }

        if (objs2 != null && !objs2.GetComponent<SnakeMovementReal>().invisible)
        {
            List<Transform> aux2 = objs2.GetComponent<SnakeMovementReal>().BodyParts;

            dist = Vector3.Distance(BodyParts[0].position, aux2[aux2.Count-1].position);

            if (dist < min && ID != objs2.GetComponent<SnakeMovementReal>().ID)
            {
                min = dist;
            }
        }

        if (min >= umbral)
        {
            return false;
        }
        return true;
    }

    //Comprueba si debe activarse el poder que la serpiente tenga asignado
    public void activatePower()
    {
        if(!enRecarga && !poderActivo)
        {
            switch (tipoPoder)
            {
                case 0: //velocidad
                    if (enemigoCercano(400)) poderActivo = true;
                    else if (enemigoCercanoAt(400)) poderActivo = true;
                    break;
                case 1: //invisibilidad
                    if (enemigoCercano(300)) poderActivo = true;
                    break;
                case 2: //repelente
                    if (enemigoCercano(200)) poderActivo = true;
                    break;
            }
        }
    }

    //Permite asignar un poder a la serpiente.
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

    //Devuelve los segundos que lleva una tarea determinada (poder o recarga)
    private float getSeconds()
    {

        int seconds = 0;

        if (!poderActivo && !enRecarga) lastTime = Time.realtimeSinceStartup;
        else
        {
            seconds = (int)(Time.realtimeSinceStartup-lastTime);
        }

        return seconds;
    }

    //Comprueba si hemos terminado de usar el poder (tiempo).
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
                generateRandom = false;
            }
        }
    }

    //Comprueba si hemos terminado la recarga (tiempo).
    private void updateRecarga()
    {
        float seconds = getSeconds();

        if (seconds >= trecarga && enRecarga)
        {
            enRecarga = false;
            lastTime = Time.realtimeSinceStartup;
        }
    }

    //Va actualizando para que la única pieza colisionable sea la última.
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

    //Selecciona el siguiente estado al que la serpiente pasa.
    public int nuevoEstado()
    {
        float dist = 0.0f;
        float min = Mathf.Infinity;
        int pos = 0;
        int estado = -1;
        bool player = false;
        bool repel = false;

        GameObject[] objs1 = GameObject.FindGameObjectsWithTag("snakes");
        GameObject objs2 = GameObject.FindGameObjectWithTag("snakep");

        //Se recorren todas las serpientes IA del mapa
        for (int i = 0; i < objs1.Length; i++)
        {

            if (!objs1[i].gameObject.GetComponent<SnakeMovementIA>().invisible)
            {
                List<Transform> lt = objs1[i].GetComponent<SnakeMovementIA>().BodyParts;

                //Se recorren cada una de las vidas de la serpiente
                for (int j = 0; j < lt.Count; j++)
                {

                    if (lt[j] != null)
                    {
                        Vector3 aux1 = lt[j].position;

                        //Se comprueba la distancia entre nuestra cabeza y una vida determinada de la serpiente enemiga
                        dist = Vector3.Distance(BodyParts[0].position, aux1);

                        if (dist < min && ID != lt[j].gameObject.GetComponentInParent<SnakeMovementIA>().ID)
                        {
                            min = dist;
                            pos = i;
                        }

                        //Comprobamos si la serpiente que estamos viendo es repelente.
                        if (dist < umbralDist && !repel && objs1[i].gameObject.GetComponent<SnakeMovementIA>().repelente && ID != objs1[i].GetComponent<SnakeMovementIA>().ID) repel = true;
                    }
                }
            }
        }

        //Mismo proceso anterior pero comparando con la serpiente jugador.
        if(objs2 != null)
        {
            if (!objs2.gameObject.GetComponent<SnakeMovementReal>().invisible)
            {
                List<Transform> lt2 = objs2.GetComponent<SnakeMovementReal>().BodyParts;

                for (int j = 0; j < lt2.Count; j++)
                {

                    Vector3 aux2 = lt2[j].position;

                    dist = Vector3.Distance(BodyParts[0].position, aux2);

                    if (dist < min && ID != lt2[j].GetComponentInParent<SnakeMovementReal>().ID)
                    {
                        min = dist;
                        player = true;
                    }

                    if (dist < umbralDist && !repel && objs2.gameObject.GetComponent<SnakeMovementReal>().repelente && ID != objs2.GetComponent<SnakeMovementReal>().ID) repel = true;
                }
            }
        }

        //DEFINICION DE ESTADOS:

        //Si no tenemos a nadie cerca, seguimos es búsqueda.
        if (min >= umbralDist)
        {
            //Búsqueda
            estado = 0;
        }
        else
        {
            List<Transform> other;

            if (!player)
            {
                other = objs1[pos].GetComponent<SnakeMovementIA>().BodyParts;
            }
            else
            {
                other = objs2.GetComponent<SnakeMovementReal>().BodyParts;
            }

            int n_elem = other.Count;

            //Si tenemos un repelente cerca, nos alejamos de él.
            if (repel)
            {
                //Alejarse
                estado = 2;
            }
            //Si nosotros somos invisbles, dependiendo de una probabilidad, atacaremos o huiremos para defendernos.
            else if (invisible)
            {
                if (ataqueInvisible < 0.3f) estado = 1;
                else estado = 2;
            }
            //Si tenemos la cola del enemigo mas cerca que su cabeza, atacamos.
            else if((Vector3.Distance(BodyParts[0].position, other[0].position) > Vector3.Distance(BodyParts[0].position, other[n_elem - 1].position)) && !(tipoPoder == 2 && poderActivo))
            {
                //Ataque
                estado = 1;
            }
            //Si nuestra cabeza esta muy cerca de otra, nos alejamos de ésta
            else if(Vector3.Distance(BodyParts[0].position, other[0].position) < 200 || dcontraria)
            {
                //Alejarse
                estado = 2;
            }
            else
            {
                //Defensa
                estado = 3;
            }

        }

        return estado;
    }

    //Funcion para defenir el destino al que nos movemos.
    public GameObject selDest(int estado)
    {

        float dist = 0.0f;
        int pos = 0;
        int posj = 0;
        float min = Mathf.Infinity;
        GameObject[] objs;
        GameObject objs2;
        GameObject res = null;
        bool player = false;
        int tam = 0;
        int tam_enemy = 0;
        bool hayBolas;

        //Dependiendo del estado calculado en la funcion anterior, haremos un acción determinada.
        switch (estado)
        {
            case 0: //Búsqueda
                objs = GameObject.FindGameObjectsWithTag("balls");
                hayBolas = false;

                //Buscamos las bolas en el mapa y vamos a por la más cercana
                for (int i = 0; i < objs.Length; i++)
                {
                    dist = Vector3.Distance(BodyParts[0].position, objs[i].transform.position);

                    if (dist < min && !DamageCircle.IsOutsideCircle_Static(objs[i].transform.position))
                    {
                        hayBolas = true;
                        min = dist;
                        pos = i;
                    }
                }

                if(!hayBolas)
                {
                    //Ataque
                    objs = GameObject.FindGameObjectsWithTag("snakes");
                    objs2 = GameObject.FindGameObjectWithTag("snakep");
                    player = false;
                    tam_enemy = -1;

                    //Buscamos de todas las serpientes, la mas cercana para atacarle.
                    for (int i = 0; i < objs.Length; i++)
                    {
                        tam_enemy = objs[i].GetComponent<SnakeMovementIA>().BodyParts.Count;
                        dist = Vector3.Distance(BodyParts[0].position, objs[i].GetComponent<SnakeMovementIA>().BodyParts[tam_enemy - 1].position);

                        if (dist < min && ID != objs[i].GetComponent<SnakeMovementIA>().ID && !objs[i].gameObject.GetComponent<SnakeMovementIA>().invisible)
                        {
                            min = dist;
                            pos = i;
                        }
                    }

                    if (objs2 != null)
                    {
                        tam_enemy = objs2.GetComponent<SnakeMovementReal>().BodyParts.Count;
                        dist = Vector3.Distance(BodyParts[0].position, objs2.GetComponent<SnakeMovementReal>().BodyParts[tam_enemy - 1].position);

                        if (dist < min && !objs2.GetComponent<SnakeMovementReal>().invisible)
                        {
                            min = dist;
                            player = true;
                        }
                    }

                    tam = 0;
                    if (player)
                    {
                        tam = objs2.GetComponent<SnakeMovementReal>().BodyParts.Count;
                        res = objs2.GetComponent<SnakeMovementReal>().BodyParts[tam - 1].gameObject;
                    }
                    else
                    {
                        tam = objs[pos].GetComponent<SnakeMovementIA>().BodyParts.Count;
                        res = objs[pos].GetComponent<SnakeMovementIA>().BodyParts[tam - 1].gameObject;
                    }
                }
                else
                {
                    res = objs[pos];
                }
                break;
            case 1: //Ataque
                objs = GameObject.FindGameObjectsWithTag("snakes");
                objs2 = GameObject.FindGameObjectWithTag("snakep");
                player = false;
                tam_enemy = -1;

                //Buscamos de todas las serpientes, la mas cercana para atacarle.
                for (int i = 0; i < objs.Length; i++)
                {
                    tam_enemy = objs[i].GetComponent<SnakeMovementIA>().BodyParts.Count;
                    dist = Vector3.Distance(BodyParts[0].position, objs[i].GetComponent<SnakeMovementIA>().BodyParts[tam_enemy-1].position);

                    if (dist < min && ID != objs[i].GetComponent<SnakeMovementIA>().ID && !objs[i].gameObject.GetComponent<SnakeMovementIA>().invisible)
                    {
                        min = dist;
                        pos = i;
                    }
                }

                if(objs2 != null)
                {
                    tam_enemy = objs2.GetComponent<SnakeMovementReal>().BodyParts.Count;
                    dist = Vector3.Distance(BodyParts[0].position, objs2.GetComponent<SnakeMovementReal>().BodyParts[tam_enemy-1].position);

                    if (dist < min && !objs2.GetComponent<SnakeMovementReal>().invisible)
                    {
                        min = dist;
                        player = true;
                    }
                }

                tam = 0;
                if (player)
                {
                    tam = objs2.GetComponent<SnakeMovementReal>().BodyParts.Count;
                    res = objs2.GetComponent<SnakeMovementReal>().BodyParts[tam-1].gameObject;
                }
                else
                {
                    tam = objs[pos].GetComponent<SnakeMovementIA>().BodyParts.Count;
                    res = objs[pos].GetComponent<SnakeMovementIA>().BodyParts[tam-1].gameObject;
                }

                break;
            case 2: //Alejarse
                dcontraria = true;

                objs = GameObject.FindGameObjectsWithTag("snakes");
                objs2 = GameObject.FindGameObjectWithTag("snakep");
                player = false;

                //Buscamos la serpiente cercana o repelente para alejarnos.
                for (int i = 0; i < objs.Length; i++)
                {
                    List<Transform> lt = objs[i].GetComponent<SnakeMovementIA>().BodyParts;

                    //Se recorren cada una de las vidas de la serpiente
                    for (int j = 0; j < lt.Count; j++)
                    {

                        if (lt[j] != null)
                        {
                            Vector3 aux1 = lt[j].position;

                            //Se comprueba la distancia entre nuestra cabeza y una vida determinada de la serpiente enemiga
                            dist = Vector3.Distance(BodyParts[0].position, aux1);

                            if (dist < min && ID != lt[j].gameObject.GetComponentInParent<SnakeMovementIA>().ID && !objs[i].gameObject.GetComponent<SnakeMovementIA>().invisible)
                            {
                                min = dist;
                                pos = i;
                                posj = j;
                            }

                            //Comprobamos si la serpiente que estamos viendo es repelente.
                            if (dist < umbralDist && objs[i].gameObject.GetComponent<SnakeMovementIA>().repelente && ID != objs[i].GetComponent<SnakeMovementIA>().ID)
                            {
                                pos = i;

                                dist = Vector3.Distance(BodyParts[0].position, lt[0].position);
                                min = dist;
                                posj = 0;

                                for (int t = 0; t < lt.Count; t++)
                                {
                                    dist = Vector3.Distance(BodyParts[0].position, lt[t].position);

                                    if (dist < min)
                                    {
                                        posj = t;
                                        min = dist;
                                    }
                                }
                                break;
                            }
                        }
                    }
                }

                //Comprobamos el jugador
                if (objs2 != null)
                {
                    List<Transform> lt2 = objs2.GetComponent<SnakeMovementReal>().BodyParts;

                    for (int j = 0; j < lt2.Count; j++)
                    {
                        Vector3 aux2 = lt2[j].position;

                        dist = Vector3.Distance(BodyParts[0].position, aux2);

                        if ((dist < min && !objs2.gameObject.GetComponent<SnakeMovementReal>().invisible) || (dist < umbralDist && objs2.gameObject.GetComponent<SnakeMovementReal>().repelente && ID != objs2.GetComponent<SnakeMovementReal>().ID))
                        {
                            min = dist;
                            player = true;
                            posj = j;
                        }
                    }
                }

                tam = 0;
                if (player)
                {
                    res = objs2.GetComponent<SnakeMovementReal>().BodyParts[posj].gameObject;
                }
                else
                {
                    res = objs[pos].GetComponent<SnakeMovementIA>().BodyParts[posj].gameObject;
                }
                break;
            case 3: //Defensa
                hayBolas = false;
                objs = GameObject.FindGameObjectsWithTag("balls");

                //Buscamos las bolas y vamos a por la más cercana
                for (int i = 0; i < objs.Length; i++)
                {
                    dist = Vector3.Distance(BodyParts[0].position, objs[i].transform.position);

                    if (dist < min && !DamageCircle.IsOutsideCircle_Static(objs[i].transform.position))
                    {
                        hayBolas = true;
                        min = dist;
                        pos = i;
                    }
                }

                if (!hayBolas)
                {
                    //Ataque
                    objs = GameObject.FindGameObjectsWithTag("snakes");
                    objs2 = GameObject.FindGameObjectWithTag("snakep");
                    player = false;
                    tam_enemy = -1;
                    min = Mathf.Infinity;

                    //Buscamos de todas las serpientes, la mas cercana para atacarle.
                    for (int i = 0; i < objs.Length; i++)
                    {
                        tam_enemy = objs[i].GetComponent<SnakeMovementIA>().BodyParts.Count;
                        dist = Vector3.Distance(BodyParts[0].position, objs[i].GetComponent<SnakeMovementIA>().BodyParts[tam_enemy - 1].position);

                        if (dist < min && ID != objs[i].GetComponent<SnakeMovementIA>().ID && !objs[i].gameObject.GetComponent<SnakeMovementIA>().invisible)
                        {
                            min = dist;
                            pos = i;
                        }
                    }

                    if (objs2 != null)
                    {
                        tam_enemy = objs2.GetComponent<SnakeMovementReal>().BodyParts.Count;
                        dist = Vector3.Distance(BodyParts[0].position, objs2.GetComponent<SnakeMovementReal>().BodyParts[tam_enemy - 1].position);

                        if (dist < min && !objs2.GetComponent<SnakeMovementReal>().invisible)
                        {
                            min = dist;
                            player = true;
                        }
                    }

                    tam = 0;
                    if (player)
                    {
                        tam = objs2.GetComponent<SnakeMovementReal>().BodyParts.Count;
                        res = objs2.GetComponent<SnakeMovementReal>().BodyParts[tam - 1].gameObject;
                    }
                    else
                    {
                        tam = objs[pos].GetComponent<SnakeMovementIA>().BodyParts.Count;
                        res = objs[pos].GetComponent<SnakeMovementIA>().BodyParts[tam - 1].gameObject;
                    }
                }
                else
                {
                    res = objs[pos];
                }
                break;
        }

        return res;
    }

    //Añade una nueva vida a la serpiente.
    public void AddBodyPart()
    {
        Transform newpart = (Instantiate(bodyprefab, BodyParts[BodyParts.Count - 1].position, BodyParts[BodyParts.Count - 1].rotation) as GameObject).transform;

        newpart.SetParent(transform);

        BodyParts.Add(newpart);
    }

    //Mueve la serpiente hacia un objetivo seleccionado en la funcion selDest.
    public void Move(GameObject dest)
    {
        //Comprueba si activamos el poder.
        activatePower();

        float curspeed;

        // GESTION PODERES
        if (tipoPoder == 0 && poderActivo) curspeed = speed * 3f;
        else curspeed = speed;

        if (tipoPoder == 2 && poderActivo) poderRepelente();

        if (tipoPoder == 1 && poderActivo)
        {
            curspeed = speed * 1.8f;
            poderInvisible();
        }
        //FIN PODERES

        //Comprobamos si el movimiento es alejarnos o ir hacia el objetivo.
        if (dcontraria)
        {
            if (dest.gameObject.GetComponentInParent<SnakeMovementIA>() != null && dest.gameObject.GetComponentInParent<SnakeMovementIA>().repelente) curspeed = speed * 1.5f;
            if (dest.gameObject.GetComponentInParent<SnakeMovementReal>() != null && dest.gameObject.GetComponentInParent<SnakeMovementReal>().repelente) curspeed = speed * 1.5f;

            BodyParts[0].position = Vector3.MoveTowards(BodyParts[0].position, dest.transform.position, -1* curspeed * Time.deltaTime);
            if (invisible && Vector3.Distance(BodyParts[0].position, dest.transform.position) > 2000) dcontraria = false;
            else if (Vector3.Distance(BodyParts[0].position, dest.transform.position) > 1500) dcontraria = false;
        }
        else
        {
            BodyParts[0].position = Vector3.MoveTowards(BodyParts[0].position, dest.transform.position, curspeed * Time.deltaTime);
        }

        //Se mueven el resto de partes con la principal.
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
                BodyParts[i].gameObject.GetComponent<Renderer>().material.color = Color.white;
            }
            BodyParts[BodyParts.Count - 1].gameObject.GetComponent<SphereCollider>().enabled = true;
        }
    }

    //Cuando se activa el poder invisible, se llama a esta funcion para cambiar la configuración necesaria
    public void poderInvisible()
    {
        invisible = true;

        //Se genera el aleatrio que va a determinar si atacamos o no.
        if (!generateRandom)
        {
            ataqueInvisible = Random.value;
            generateRandom = true;
        }

        // Se pone la serpiente entera como no colisionable y se hace invisible
        for (int i = 0; i < BodyParts.Count; i++)
        {
            BodyParts[i].gameObject.GetComponent<SphereCollider>().enabled = false;
            BodyParts[i].gameObject.GetComponent<Renderer>().enabled = false;
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
                BodyParts[i].gameObject.GetComponent<Renderer>().enabled = true;
            }
            BodyParts[BodyParts.Count - 1].gameObject.GetComponent<SphereCollider>().enabled = true;
            BodyParts[0].gameObject.GetComponent<SphereCollider>().enabled = true;
        }
    }

    //Funcion para asignar ID a cada serpiente y poder identificarlas.
    public static int GetID()
    {
        contador++;
        return contador;
    }

    //Elimina la última vida de la serpiente.
    public void RemoveBodyPart()
    {
        if(BodyParts.Count > 0) BodyParts.RemoveAt(BodyParts.Count - 1);
        if (BodyParts.Count == 1)
        {
            Destroy(this.gameObject);
        }
    }

    //Suma vida a la serpiente.
    public void sumarVida(int n)
    {
        vida += n;
    }

    //Resta vida a la serpiente.
    public void restarVida(int n)
    {
        vida -= n;
    }
}
