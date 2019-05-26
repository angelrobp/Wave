﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Game : MonoBehaviour
{

    private GameObject objectEstadoJuego, texto, personaje;
    private EstadoJuego estadoJuego;

    [SerializeField]
    private GameObject Up = null;

    [SerializeField]
    private GameObject snake = null;

    [SerializeField]
    private GameObject snakep = null;

    private static GameObject staticUp = null;

    [SerializeField]
    public int UpsNumber = 200;
    [SerializeField]
    private int SnakeNumber = 50;

    [SerializeField]
    private int velocidad= 0;
    [SerializeField]
    private int invisible = 0;

    //Temporizador
    [SerializeField]
    private float mainTimer;
    private int secondsTimer;
    private int minutesTimer;

    // Start is called before the first frame update
    void Start()
    {
        //incialización de tiempo de partida
        mainTimer = 15f*60f;

        staticUp = Up;
        int liminv = velocidad + invisible;

        personaje = Instantiate(snakep);

        // Se inicializan las bolas
        GameObject g;
        for(int i=0; i<UpsNumber; i++)
        {
            g = Instantiate(Up);
            g.transform.position = new Vector3(Random.Range(-7000, 7000), Random.Range(-7000, 7000), 0);
        }

        //Se inicializan las serpientes controladas por la IA
        GameObject s;
        for (int i = 0; i < SnakeNumber; i++)
        {
            s = Instantiate(snake);
            s.transform.position = new Vector3(Random.Range(-7000, 7000), Random.Range(-7000, 7000), 0);

            if (i < velocidad) s.GetComponent<SnakeMovementIA>().setPoder(0);
            else if(i < liminv) s.GetComponent<SnakeMovementIA>().setPoder(1);
            else s.GetComponent<SnakeMovementIA>().setPoder(2);
        }
        

        
        personaje.transform.position = new Vector3(Random.Range(-7000, 7000), Random.Range(-7000, 7000), 0);

        //UNICAMENTE MODIFICAR ESTA VARIABLE PARA INDICAR EL PODER DEL JUGADOR (TIPO)
        objectEstadoJuego = GameObject.FindGameObjectWithTag("EstadoJuego");
        estadoJuego = objectEstadoJuego.GetComponent<EstadoJuego>();

        personaje.GetComponent<SnakeMovementReal>().setPoder(estadoJuego.personajeSeleccionado.power);
        //p.GetComponent<SnakeMovementReal>().setMaterial(estadoJuego.personajeSeleccionado.characterColor);

        //Modifico información en pantalla
        GameObject.FindGameObjectWithTag("TextPower").GetComponent<Text>().text = estadoJuego.personajeSeleccionado.characterNamePower;
        GameObject.FindGameObjectWithTag("TextPlayers").GetComponent<Text>().text = (SnakeNumber+1) + "/" + (SnakeNumber+1);
        GameObject.FindGameObjectWithTag("TextTime").GetComponent<Text>().text = "15'00\"";
        GameObject.FindGameObjectWithTag("TextReload1").GetComponent<Text>().text = "Duracion Poder: ";


    }

    // Update is called once per frame
    void Update()
    {
        updateText();
    }

    //Actualización del tiempo de la partida
    public void updateTime()
    {
        mainTimer -= Time.deltaTime;
        minutesTimer = Mathf.RoundToInt(mainTimer) / 60;
        secondsTimer = Mathf.RoundToInt(mainTimer) % 60;
            
        GameObject.FindGameObjectWithTag("TextTime").GetComponent<Text>().text = minutesTimer + "'" + secondsTimer + "\"";

    }

    //Actualización de los textos de la partida
    public void updateText()
    {
        
        //Contador jugadores
        GameObject[] snakes = GameObject.FindGameObjectsWithTag("snakes");
        GameObject.FindGameObjectWithTag("TextPlayers").GetComponent<Text>().text = (snakes.Length + 1) + "/" + (SnakeNumber + 1);
        
        //Contadores de tiempos de partida, recarga y duración de poder
        if (personaje.GetComponent<SnakeMovementReal>().isEnRecarga())
        {
            GameObject.FindGameObjectWithTag("TextReload1").GetComponent<Text>().text = "Recargando: ";
            float timeToAction = personaje.GetComponent<SnakeMovementReal>().getTRecarga() - Mathf.RoundToInt((Time.realtimeSinceStartup - personaje.GetComponent<SnakeMovementReal>().getLastTime()));
            GameObject.FindGameObjectWithTag("TextReload2").GetComponent<Text>().text = timeToAction + "\"";
        }
        else if (personaje.GetComponent<SnakeMovementReal>().isPoderActivo())
        {
            GameObject.FindGameObjectWithTag("TextReload1").GetComponent<Text>().text = "Duracion Poder: ";
            float timeToAction = personaje.GetComponent<SnakeMovementReal>().getTDuracion() - Mathf.RoundToInt((Time.realtimeSinceStartup - personaje.GetComponent<SnakeMovementReal>().getLastTime()));
            GameObject.FindGameObjectWithTag("TextReload2").GetComponent<Text>().text = timeToAction + "\"";
        }
        else
        {
            GameObject.FindGameObjectWithTag("TextReload1").GetComponent<Text>().text = "Poder Preparado";
            GameObject.FindGameObjectWithTag("TextReload2").GetComponent<Text>().text = "";
        }
        updateTime();
        

    }
    public int GetUpsNumber() => UpsNumber;

    //Genera una nueva bola en el espacio
    public static void GenerateUp()
    {
        GameObject g = Instantiate(staticUp);
        g.transform.position = new Vector3(Random.Range(-7000, 7000), Random.Range(-7000, 7000), 0);
    }




}
