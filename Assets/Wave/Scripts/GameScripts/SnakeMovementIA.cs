using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    public bool repelente = false;
    public bool invisible = false;

    private bool dcontraria = false;

    private double lastTime;

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

        updatePoder();

        updateRecarga();

        int estado = nuevoEstado();

        god = selDest(estado);

        Move(god);

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

        for (int i = 0; i < objs1.Length; i++)
        {

            List<Transform> aux1 = objs1[i].GetComponent<SnakeMovementIA>().BodyParts;

            dist = Vector3.Distance(BodyParts[BodyParts.Count-1].position, aux1[0].position);

            if (dist < min && ID != objs1[i].GetComponent<SnakeMovementIA>().ID)
            {
                min = dist;
                pos = i;
            }
        }

        if (objs2 != null)
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

            List<Transform> aux1 = objs1[i].GetComponent<SnakeMovementIA>().BodyParts;

            if (aux1 != null)
            {
                dist = Vector3.Distance(BodyParts[0].position, aux1[aux1.Count - 1].position);

                if (dist < min && ID != objs1[i].GetComponent<SnakeMovementIA>().ID)
                {
                    min = dist;
                    pos = i;
                }
            }
        }

        if (objs2 != null)
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

    private void updateRecarga()
    {
        float seconds = getSeconds();

        if (seconds >= trecarga && enRecarga)
        {
            enRecarga = false;
            lastTime = Time.realtimeSinceStartup;
        }
    }

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

        for (int i = 0; i < objs1.Length; i++)
        {

            if (!objs1[i].gameObject.GetComponent<SnakeMovementIA>().invisible)
            {
                List<Transform> lt = objs1[i].GetComponent<SnakeMovementIA>().BodyParts;

                for (int j = 0; j < lt.Count; j++)
                {

                    if (lt[j] != null)
                    {
                        Vector3 aux1 = lt[j].position;

                        dist = Vector3.Distance(BodyParts[0].position, aux1);

                        if (dist < min && ID != lt[j].gameObject.GetComponentInParent<SnakeMovementIA>().ID)
                        {
                            min = dist;
                            pos = i;
                        }

                        if (dist < umbralDist && !repel && objs1[i].gameObject.GetComponent<SnakeMovementIA>().repelente && ID != objs1[i].GetComponent<SnakeMovementIA>().ID) repel = true;
                    }
                }
            }
        }

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

            if (repel)
            {
                //Alejarse
                estado = 2;
            }
            else if (invisible)
            {
                if (Random.value < 0.3f) estado = 1;
                else estado = 2;
            }
            else if((Vector3.Distance(BodyParts[0].position, other[0].position) > Vector3.Distance(BodyParts[0].position, other[n_elem - 1].position)) && !(tipoPoder == 2 && poderActivo))
            {
                //Ataque
                estado = 1;
            }
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

    public GameObject selDest(int estado)
    {

        float dist = 0.0f;
        int pos = 0;
        float min = Mathf.Infinity;
        GameObject[] objs;
        GameObject objs2;
        GameObject res = null;
        bool player = false;
        int tam = 0;


        switch (estado)
        {
            case 0:
                objs = GameObject.FindGameObjectsWithTag("balls");

                for (int i = 0; i < objs.Length; i++)
                {
                    dist = Vector3.Distance(BodyParts[0].position, objs[i].transform.position);

                    if (dist < min)
                    {
                        min = dist;
                        pos = i;
                    }
                }

                res = objs[pos];
                break;
            case 1:
                objs = GameObject.FindGameObjectsWithTag("snakes");
                objs2 = GameObject.FindGameObjectWithTag("snakep");
                player = false;

                for (int i = 0; i < objs.Length; i++)
                {
                    dist = Vector3.Distance(BodyParts[0].position, objs[i].GetComponent<SnakeMovementIA>().BodyParts[0].position);

                    if (dist < min && ID != objs[i].GetComponent<SnakeMovementIA>().ID && !objs[i].gameObject.GetComponent<SnakeMovementIA>().invisible)
                    {
                        min = dist;
                        pos = i;
                    }
                }

                if(objs2 != null)
                {
                    dist = Vector3.Distance(BodyParts[0].position, objs2.GetComponent<SnakeMovementReal>().BodyParts[0].position);

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
            case 2:
                dcontraria = true;

                objs = GameObject.FindGameObjectsWithTag("snakes");
                objs2 = GameObject.FindGameObjectWithTag("snakep");
                player = false;

                for (int i = 0; i < objs.Length; i++)
                {
                    dist = Vector3.Distance(BodyParts[0].position, objs[i].GetComponent<SnakeMovementIA>().BodyParts[0].position);

                    if (dist < min && ID != objs[i].GetComponent<SnakeMovementIA>().ID && !objs[i].gameObject.GetComponent<SnakeMovementIA>().invisible)
                    {
                        min = dist;
                        pos = i;
                    }

                    if (dist < umbralDist && objs[i].gameObject.GetComponent<SnakeMovementIA>().repelente && ID != objs[i].GetComponent<SnakeMovementIA>().ID)
                    {
                        pos = i;
                        break;
                    }
                }

                if (objs2 != null)
                {
                    dist = Vector3.Distance(BodyParts[0].position, objs2.GetComponent<SnakeMovementReal>().BodyParts[0].position);

                    if ((dist < min && !objs2.gameObject.GetComponent<SnakeMovementReal>().invisible) || (dist < umbralDist && objs2.gameObject.GetComponent<SnakeMovementReal>().repelente && ID != objs2.GetComponent<SnakeMovementReal>().ID))
                    {
                        min = dist;
                        player = true;
                    }
                }

                tam = 0;
                if (player)
                {
                    res = objs2.GetComponent<SnakeMovementReal>().BodyParts[0].gameObject;
                }
                else
                {
                    res = objs[pos].GetComponent<SnakeMovementIA>().BodyParts[0].gameObject;
                }
                break;
            case 3:
                objs = GameObject.FindGameObjectsWithTag("balls");

                for (int i = 0; i < objs.Length; i++)
                {
                    dist = Vector3.Distance(BodyParts[0].position, objs[i].transform.position);

                    if (dist < min)
                    {
                        min = dist;
                        pos = i;
                    }
                }

                res = objs[pos];
                break;
        }

        return res;
    }

    public void AddBodyPart()
    {
        Transform newpart = (Instantiate(bodyprefab, BodyParts[BodyParts.Count - 1].position, BodyParts[BodyParts.Count - 1].rotation) as GameObject).transform;

        newpart.SetParent(transform);

        BodyParts.Add(newpart);
    }

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

        if (dcontraria)
        {
            BodyParts[0].position = Vector3.MoveTowards(BodyParts[0].position, dest.transform.position, -1* curspeed * Time.deltaTime);
            if (Vector3.Distance(BodyParts[0].position, dest.transform.position) > 1500) dcontraria = false;
        }
        else
        {
            BodyParts[0].position = Vector3.MoveTowards(BodyParts[0].position, dest.transform.position, curspeed * Time.deltaTime);
        }

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

    public void poderRepelente()
    {
        repelente = true;

        if (BodyParts.Count > 0)
        {
            BodyParts[BodyParts.Count - 1].gameObject.GetComponent<SphereCollider>().enabled = false;
        }

        for(int i = 0; i < BodyParts.Count; i++)
        {
            BodyParts[i].gameObject.GetComponent<Renderer>().material.color = Color.blue;
        }
    }

    public void resetRepelente()
    {

        repelente = false;

        if (BodyParts.Count > 0)
        {
            for (int i = 0; i < BodyParts.Count; i++)
            {
                BodyParts[i].gameObject.GetComponent<Renderer>().material.color = Color.white;
            }
            BodyParts[BodyParts.Count - 1].gameObject.GetComponent<SphereCollider>().enabled = true;
        }
    }

    public void poderInvisible()
    {
        invisible = true;

        for (int i = 0; i < BodyParts.Count; i++)
        {
            BodyParts[i].gameObject.GetComponent<SphereCollider>().enabled = false;
            BodyParts[i].gameObject.GetComponent<Renderer>().enabled = false;
        }
    }

    public void resetInvisible()
    {

        invisible = false;

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

    public static int GetID()
    {
        contador++;
        return contador;
    }

    public void RemoveBodyPart()
    {
        if(BodyParts.Count > 0) BodyParts.RemoveAt(BodyParts.Count - 1);
        if (BodyParts.Count == 1)
        {
            Destroy(this.gameObject);
        }
    }

    public void sumarVida(int n)
    {
        vida += n;
    }

    public void restarVida(int n)
    {
        vida -= n;
    }
}
