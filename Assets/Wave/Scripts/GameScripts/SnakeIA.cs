using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SnakeIA : MonoBehaviour
{
    public int ID;
    private bool crecer = false;

    // Start is called before the first frame update
    void Start()
    {
        ID = gameObject.GetComponentInParent<SnakeMovementIA>().ID;
    }

    // Update is called once per frame
    void Update()
    {

        crecer = nuevaVida();

        if (crecer)
        {
            SnakeMovementIA sm = gameObject.GetComponentInParent<SnakeMovementIA>();

            sm.AddBodyPart();
        }

    }

    public bool nuevaVida()
    {
        if (Mathf.Floor(GetComponentInParent<SnakeMovementIA>().vida / 100) >= gameObject.GetComponentInParent<SnakeMovementIA>().BodyParts.Count) return true;
        return false;
    }

    public int GetVida() => GetComponentInParent<SnakeMovementIA>().vida;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "balls")
        {
            Destroy(other.gameObject);
            GameObject[] objs = GameObject.FindGameObjectsWithTag("balls");
            if (objs.Length < GameObject.FindGameObjectWithTag("game").GetComponent<Game>().GetUpsNumber())
                Game.GenerateUp();
            GetComponentInParent<SnakeMovementIA>().sumarVida(20);
        }
        if(other.gameObject.tag == "bodypart" && other.GetComponentInParent<SnakeMovementIA>() != null && !gameObject.GetComponentInParent<SnakeMovementIA>().repelente)
        {
            if(other.GetComponentInParent<SnakeMovementIA>().ID != ID)
            {
                other.GetComponentInParent<SnakeMovementIA>().restarVida(100);
                other.GetComponentInParent<SnakeMovementIA>().RemoveBodyPart();
                Destroy(other.gameObject);
                GetComponentInParent<SnakeMovementIA>().sumarVida(50);
            }
        }
        if (other.gameObject.tag == "bodypart" && other.GetComponentInParent<SnakeMovementReal>() != null && !gameObject.GetComponentInParent<SnakeMovementIA>().repelente)
        {
            if(other.GetComponentInParent<SnakeMovementReal>().ID != ID)
            {
                other.GetComponentInParent<SnakeMovementReal>().restarVida(100);
                other.GetComponentInParent<SnakeMovementReal>().RemoveBodyPart();
                Destroy(other.gameObject);
                GetComponentInParent<SnakeMovementIA>().sumarVida(50);
            }
        }
    }
}
