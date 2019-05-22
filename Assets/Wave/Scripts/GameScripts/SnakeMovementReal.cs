using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    public void Move()
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

        alejarJugador();
        if (Input.GetKey(KeyCode.LeftArrow))
        {
            dir_angle += angle_increment;
        }
        if (Input.GetKey(KeyCode.RightArrow))

        {
            dir_angle -= angle_increment;
        }
        Vector3 position = BodyParts[0].position;
        position.x += Mathf.Cos(dir_angle) * Time.deltaTime * curspeed;
        position.y += Mathf.Sin(dir_angle) * Time.deltaTime * curspeed;
        BodyParts[0].position = position;

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

    public void AddBodyPart()
    {
        Transform newpart = (Instantiate(bodyprefab, BodyParts[BodyParts.Count-1].position, BodyParts[BodyParts.Count - 1].rotation) as GameObject).transform;

        newpart.SetParent(transform);

        BodyParts.Add(newpart);


        BodyParts[BodyParts.Count - 1].gameObject.GetComponent<Renderer>().material.color = orange;
    }

    public void RemoveBodyPart()
    {
        BodyParts.RemoveAt(BodyParts.Count - 1);
        if(BodyParts.Count == 1)
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
            seconds = (int)(Time.realtimeSinceStartup - lastTime);
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

    public void poderRepelente()
    {
        repelente = true;

        if (BodyParts.Count > 0)
        {
            BodyParts[BodyParts.Count - 1].gameObject.GetComponent<SphereCollider>().enabled = false;
        }

        for (int i = 0; i < BodyParts.Count; i++)
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
                BodyParts[i].gameObject.GetComponent<Renderer>().material.color = orange;
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
            Color cn = orange;
            cn.a = 0.3f;
            BodyParts[i].gameObject.GetComponent<Renderer>().material.color = cn;
        }
    }

    public void resetInvisible()
    {

        invisible = false;

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

    public void alejarJugador()
    {
        float dist = 0.0f;
        int pos = -1;

        GameObject[] objs = GameObject.FindGameObjectsWithTag("snakes");

        for (int i = 0; i < objs.Length; i++)
        {
            dist = Vector3.Distance(BodyParts[0].position, objs[i].GetComponent<SnakeMovementIA>().BodyParts[0].position);

            if (dist < objs[i].GetComponent<SnakeMovementIA>().umbralDist && objs[i].gameObject.GetComponent<SnakeMovementIA>().repelente)
            {
                pos = i;
                break;
            }
        }

        if(pos >= 0)
        {
            GameObject res = objs[pos].GetComponent<SnakeMovementIA>().BodyParts[0].gameObject;

            Move(res);
        }
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