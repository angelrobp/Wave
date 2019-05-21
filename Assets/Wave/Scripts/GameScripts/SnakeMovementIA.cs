using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SnakeMovementIA : MonoBehaviour
{

    public List<Transform> BodyParts = new List<Transform>();

    public float mindistance = 50000.0f;
    public int vida = 100;

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

    private int estado = 0;
    public float umbralDist = 100.0f;
    private GameObject god;

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

        estado = nuevoEstado();

        god = selDest(estado);

        Move(god);

    }

    public void updateCollider()
    {
        BodyParts[BodyParts.Count - 1].gameObject.GetComponent<SphereCollider>().enabled = true;

        if (BodyParts.Count > 2)
        {
            BodyParts[BodyParts.Count - 2].gameObject.GetComponent<SphereCollider>().enabled = false;
        }
    }

    public int nuevoEstado()
    {
        List<float> dists = new List<float>();
        float min = Mathf.Infinity;
        int pos = 0;
        int estado = -1;
        bool player = false;

        GameObject[] objs1 = GameObject.FindGameObjectsWithTag("snakes");
        GameObject objs2 = GameObject.FindGameObjectWithTag("snakep");

        for (int i = 0; i < objs1.Length; i++)
        {

            Vector3 aux1 = objs1[i].GetComponent<SnakeMovementIA>().BodyParts[0].position;

            dists.Add(Vector3.Distance(BodyParts[0].position, aux1));

            if (dists[dists.Count-1] < min && dists[dists.Count - 1] != 0)
            {
                min = dists[dists.Count - 1];
                pos = i;
            }
        }

        if(objs2 != null)
        {
            Vector3 aux2 = objs2.GetComponent<SnakeMovementReal>().BodyParts[0].position;

            dists.Add(Vector3.Distance(BodyParts[0].position, aux2));

            if (dists[dists.Count - 1] < min && dists[dists.Count - 1] != 0)
            {
                min = dists[dists.Count - 1];
                player = true;
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
                other = objs1[pos].GetComponent<SnakeMovementIA>().BodyParts;
            else
                other = objs2.GetComponent<SnakeMovementReal>().BodyParts;

            int n_elem = other.Count;

            if(Vector3.Distance(BodyParts[0].position, other[0].position) > Vector3.Distance(BodyParts[0].position, other[n_elem - 1].position))
            {
                //Ataque
                estado = 1;
            }
            else
            {
                //Defensa
                estado = 2;
            }

        }

        return estado;
    }

    public GameObject selDest(int estado)
    {

        List<float> dists = new List<float>();
        int pos = 0;
        float min = Mathf.Infinity;
        GameObject[] objs;
        GameObject objs2;
        GameObject res = null;

        switch (estado)
        {
            case 0:
                objs = GameObject.FindGameObjectsWithTag("balls");

                for (int i = 0; i < objs.Length; i++)
                {
                    dists.Add(Vector3.Distance(BodyParts[0].position, objs[i].transform.position));

                    if (dists[dists.Count - 1] < min)
                    {
                        min = dists[dists.Count - 1];
                        pos = i;
                    }
                }

                res = objs[pos];
                break;
            case 1:
                objs = GameObject.FindGameObjectsWithTag("snakes");
                objs2 = GameObject.FindGameObjectWithTag("snakep");
                bool player = false;

                for (int i = 0; i < objs.Length; i++)
                {
                    dists.Add(Vector3.Distance(BodyParts[0].position, objs[i].GetComponent<SnakeMovementIA>().BodyParts[0].position));

                    if (dists[dists.Count - 1] < min && dists[dists.Count - 1] != 0)
                    {
                        min = dists[dists.Count - 1];
                        pos = i;
                    }
                }

                if(objs2 != null)
                {
                    dists.Add(Vector3.Distance(BodyParts[0].position, objs2.GetComponent<SnakeMovementReal>().BodyParts[0].position));

                    if (dists[dists.Count - 1] < min)
                    {
                        min = dists[dists.Count - 1];
                        player = true;
                    }
                }

                int tam = 0;
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
                objs = GameObject.FindGameObjectsWithTag("balls");

                for (int i = 0; i < objs.Length; i++)
                {
                    dists.Add(Vector3.Distance(BodyParts[0].position, objs[i].transform.position));

                    if (dists[dists.Count - 1] < min)
                    {
                        min = dists[dists.Count - 1];
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
        float curspeed = speed;
        /*
        if (Input.GetKey(KeyCode.LeftArrow))
        {
            dir_angle += angle_increment;
        }
        if (Input.GetKey(KeyCode.RightArrow))

        {
            dir_angle -= angle_increment;
        }
        Vector3 position = BodyParts[0].transform.position;
        position.x += Mathf.Cos(dir_angle) * Time.deltaTime * curspeed;
        position.y += Mathf.Sin(dir_angle) * Time.deltaTime * curspeed;
        BodyParts[0].transform.position = position;
        */

        BodyParts[0].position = Vector3.MoveTowards(BodyParts[0].position, dest.transform.position, curspeed * Time.deltaTime);

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

    public static int GetID()
    {
        contador++;
        return contador;
    }

    public void RemoveBodyPart()
    {
        BodyParts.RemoveAt(BodyParts.Count - 1);
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
