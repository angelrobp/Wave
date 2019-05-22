using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Clase para cada una de las vidas de una serpiente (IA).
public class SnakeIA : MonoBehaviour
{
    public int ID;

    // Start is called before the first frame update
    void Start()
    {
        ID = gameObject.GetComponentInParent<SnakeMovementIA>().ID;
    }

    // Update is called once per frame
    void Update()
    {

        if (nuevaVida())
        {
            SnakeMovementIA sm = gameObject.GetComponentInParent<SnakeMovementIA>();

            sm.AddBodyPart();
        }

    }

    //Comprueba si tenemos que crecer dependiendo de la vida que tenemos.
    public bool nuevaVida()
    {
        if (Mathf.Floor(GetComponentInParent<SnakeMovementIA>().vida / 100) >= gameObject.GetComponentInParent<SnakeMovementIA>().BodyParts.Count) return true;
        return false;
    }

    public int GetVida() => GetComponentInParent<SnakeMovementIA>().vida;

    //Comprueba las colisiones de cada una de las bolas.
    private void OnTriggerEnter(Collider other)
    {
        //Si colisionamos con una bola (nuestra cabeza)
        if (other.gameObject.tag == "balls")
        {
            //Se destruye la bola y se crea una nueva en el mapa para seguir con la proporción de bola indicadas al inicio.
            Destroy(other.gameObject);
            GameObject[] objs = GameObject.FindGameObjectsWithTag("balls");
            if (objs.Length < GameObject.FindGameObjectWithTag("game").GetComponent<Game>().GetUpsNumber())
                Game.GenerateUp();
            //Se suma la vida correspondiente a la serpiente.
            GetComponentInParent<SnakeMovementIA>().sumarVida(20);
        }
        //Si colisionamos con otra serpiente. (IA)
        if (other.gameObject.tag == "bodypart" && other.GetComponentInParent<SnakeMovementIA>() != null && !gameObject.GetComponentInParent<SnakeMovementIA>().repelente)
        {
            if(other.GetComponentInParent<SnakeMovementIA>().ID != ID)
            {
                //Restamos la vida a la otra serpiente, destruimos la ultima vida y nos sumamos vida.
                other.GetComponentInParent<SnakeMovementIA>().restarVida(100);
                other.GetComponentInParent<SnakeMovementIA>().RemoveBodyPart();
                Destroy(other.gameObject);
                GetComponentInParent<SnakeMovementIA>().sumarVida(50);
            }
        }
        //Si colisionamos con otra serpiente. (Jugador)
        if (other.gameObject.tag == "bodypart" && other.GetComponentInParent<SnakeMovementReal>() != null && !gameObject.GetComponentInParent<SnakeMovementIA>().repelente)
        {
            if(other.GetComponentInParent<SnakeMovementReal>().ID != ID)
            {
                //Restamos la vida a la otra serpiente, destruimos la ultima vida y nos sumamos vida.
                other.GetComponentInParent<SnakeMovementReal>().restarVida(100);
                other.GetComponentInParent<SnakeMovementReal>().RemoveBodyPart();
                Destroy(other.gameObject);
                GetComponentInParent<SnakeMovementIA>().sumarVida(50);
            }
        }
    }
}
