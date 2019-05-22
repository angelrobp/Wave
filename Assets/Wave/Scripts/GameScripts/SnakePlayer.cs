using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Clase para cada una de las vidas de una serpiente (jugador).
public class SnakePlayer : MonoBehaviour{

    
    public int ID;

    // Start is called before the first frame update
    void Start()
    {
        ID = gameObject.GetComponentInParent<SnakeMovementReal>().ID;
    }

    // Update is called once per frame
    void Update()
    {

        if(nuevaVida())
        { 
            SnakeMovementReal sm = gameObject.GetComponentInParent<SnakeMovementReal>();

            sm.AddBodyPart();
        }
    }

    //Comprueba si tenemos que crecer dependiendo de la vida que tenemos.
    public bool nuevaVida()
    {
        if (Mathf.Floor(GetComponentInParent<SnakeMovementReal>().vida / 100) >= gameObject.GetComponentInParent<SnakeMovementReal>().BodyParts.Count) return true;
        return false;
    }

    public int GetVida() => GetComponentInParent<SnakeMovementReal>().vida;

    //Comprueba las colisiones de cada una de las bolas.
    private void OnTriggerEnter(Collider other)
    {
        //Si colisionamos con una bola (nuestra cabeza)
        if(other.gameObject.tag == "balls")
        {
            //Se destruye la bola y se crea una nueva en el mapa para seguir con la proporción de bola indicadas al inicio.
            Destroy(other.gameObject);
            GameObject[] objs = GameObject.FindGameObjectsWithTag("balls");
            if (objs.Length < GameObject.FindGameObjectWithTag("game").GetComponent<Game>().GetUpsNumber())
                Game.GenerateUp();
            //Se suma la vida correspondiente a la serpiente.
            GetComponentInParent<SnakeMovementReal>().sumarVida(20);
        }
        //Si colisionamos con otra serpiente.
        if (other.gameObject.tag == "bodypart" && other.GetComponentInParent<SnakeMovementIA>() != null && !gameObject.GetComponentInParent<SnakeMovementReal>().repelente)
        {
            if(other.GetComponentInParent<SnakeMovementIA>().ID != ID)
            {
                //Restamos la vida a la otra serpiente, destruimos la ultima vida y nos sumamos vida.
                other.GetComponentInParent<SnakeMovementIA>().restarVida(100);
                other.GetComponentInParent<SnakeMovementIA>().RemoveBodyPart();
                Destroy(other.gameObject);
                GetComponentInParent<SnakeMovementReal>().sumarVida(50);
            }
        }
    }
}
