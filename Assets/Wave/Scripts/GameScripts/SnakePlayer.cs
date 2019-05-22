using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SnakePlayer : MonoBehaviour{

    
    public int ID;
    public bool crecer = false;

    // Start is called before the first frame update
    void Start()
    {
        ID = gameObject.GetComponentInParent<SnakeMovementReal>().ID;
    }

    // Update is called once per frame
    void Update()
    {

        crecer = nuevaVida();

        if(crecer)
        { 
            SnakeMovementReal sm = gameObject.GetComponentInParent<SnakeMovementReal>();

            sm.AddBodyPart();
        }
    }

    public bool nuevaVida()
    {
        if (Mathf.Floor(GetComponentInParent<SnakeMovementReal>().vida / 100) >= gameObject.GetComponentInParent<SnakeMovementReal>().BodyParts.Count) return true;
        return false;
    }

    public int GetVida() => GetComponentInParent<SnakeMovementReal>().vida;

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "balls")
        {
            Destroy(other.gameObject);
            GameObject[] objs = GameObject.FindGameObjectsWithTag("balls");
            if (objs.Length < GameObject.FindGameObjectWithTag("game").GetComponent<Game>().GetUpsNumber())
                Game.GenerateUp();
            GetComponentInParent<SnakeMovementReal>().sumarVida(20);
        }
        if (other.gameObject.tag == "bodypart" && other.GetComponentInParent<SnakeMovementIA>() != null && !gameObject.GetComponentInParent<SnakeMovementReal>().repelente)
        {
            if(other.GetComponentInParent<SnakeMovementIA>().ID != ID)
            {
                other.GetComponentInParent<SnakeMovementIA>().restarVida(100);
                other.GetComponentInParent<SnakeMovementIA>().RemoveBodyPart();
                Destroy(other.gameObject);
                GetComponentInParent<SnakeMovementReal>().sumarVida(50);
            }
        }
    }
}
