using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SnakeMovementReal : MonoBehaviour
{
    public List<Transform> BodyParts = new List<Transform>();

    public float mindistance = 50000.0f;
    public int vida = 100;

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

        Move();
    }

    public void updateCollider()
    {
        BodyParts[BodyParts.Count - 1].gameObject.GetComponent<SphereCollider>().enabled = true;

        if (BodyParts.Count > 2)
        {
            BodyParts[BodyParts.Count - 2].gameObject.GetComponent<SphereCollider>().enabled = false;
        }
    }

    public void Move()
    {
        float curspeed = speed;

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
}