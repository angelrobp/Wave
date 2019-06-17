using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Game : MonoBehaviour
{

    private GameObject objectEstadoJuego, texto, personaje;
    private EstadoJuego estadoJuego;
    private DamageCircle damageCircle;
    private static Game instance;

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
    private int velocidad= 0;
    [SerializeField]
    private int invisible = 0;
    [SerializeField]
    private int repulsion = 0;

    //Temporizador
    [SerializeField]
    private float mainTimer;
    private int secondsTimer;
    private int minutesTimer;

    //Tamaño y Reducción del area
    [SerializeField]
    private float diametroArea = 14000;
    private float decreaseArea;
    private float lastTimeDecrease;

    private int SnakeNumber;

    private static bool endGame, winGame;

    // Control del menu del jugador
    private GameObject pausa;
    private bool pausaActiva;

    // Start is called before the first frame update
    void Start()
    {
        instance = this;
        endGame = false;
        winGame = false;

        //Incialización de tiempo de partida
        mainTimer = 15f*60f;

        //Inicializo diametro del area
        damageCircle = GameObject.Find("DamageCircle").GetComponent<DamageCircle>();
        damageCircle.SetCircleSize(new Vector3(0, 0), new Vector3(diametroArea, diametroArea));
        
        lastTimeDecrease = Time.realtimeSinceStartup;

        //Calculo reducción de area en función del tiempo de duración de la partida
        decreaseArea = diametroArea/mainTimer;


        staticUp = Up;
        SnakeNumber = velocidad + invisible + repulsion;
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
        personaje.GetComponent<SnakeMovementReal>().setMaterial(estadoJuego.personajeSeleccionado.characterColor);
        //Modifico información en pantalla
        GameObject.FindGameObjectWithTag("TextPower").GetComponent<Text>().text = "Poder: " + estadoJuego.personajeSeleccionado.characterNamePower;
        GameObject.FindGameObjectWithTag("TextPlayers").GetComponent<Text>().text = (SnakeNumber+1) + "/" + (SnakeNumber+1);
        GameObject.FindGameObjectWithTag("TextTime").GetComponent<Text>().text = "15'00\"";
        GameObject.FindGameObjectWithTag("TextReload1").GetComponent<Text>().text = "Poder Listo";

        //Creación menu pausa
        pausa = GameObject.Find("Menu Pausa");
        pausaActiva = false;
        pausa.SetActive(pausaActiva);

    }

    public float getDiametroArea()
    {
        return diametroArea;
    }

    public static float getDiametroArea_Static()
    {
        return instance.getDiametroArea();
    }

    public float getTimer()
    {
        return mainTimer;
    }

    public static float getTimer_Static()
    {
        return instance.getTimer();
    }

    public void setActiveMenu ()
    {
        pausaActiva = !pausaActiva;
        pausa.SetActive(pausaActiva);
    }
    public void setEndGame (bool newEndGame)
    {
        endGame = newEndGame;
    }

    public bool isEndGame()
    {
        return endGame;
    }

    public void setWinGame(bool newWinGame)
    {
        winGame = newWinGame;
    }

    public bool isWinGame()
    {
        return winGame;
    }

    // Update is called once per frame
    void Update()
    {
        updateMenu();
        updateText();
        updateArea();
        updateFinalDePartida();
    }

    //Muestra u oculta el menu
    public void updateMenu()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (pausaActiva)
            {
                pausaActiva = false;

            }
            else
            {
                pausaActiva = true;
            }
            pausa.SetActive(pausaActiva);
        }
    }

    //Recuento de jugadores con vida y final de tiempo
    public void updateFinalDePartida()
    {
        
        GameObject[] snakes = GameObject.FindGameObjectsWithTag("snakes");
        if (((snakes.Length + 1) <= 1) || (mainTimer <= 0.0f))
        {
            setEndGame(true);
            setWinGame(true);
        }
    }
    //Actualización de tamaño del area en función del tiempo transcurrido
    public void updateArea()
    {
        float newTime = Time.realtimeSinceStartup - lastTimeDecrease;
        if (newTime >= 1)
        {
            diametroArea -= decreaseArea;
            damageCircle.SetCircleSize(new Vector3(0, 0), new Vector3(diametroArea, diametroArea));
            lastTimeDecrease = Time.realtimeSinceStartup;
        }
        
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
        if (personaje != null && personaje.GetComponent<SnakeMovementReal>().isEnRecarga())
        {
            float timeToAction = personaje.GetComponent<SnakeMovementReal>().getTRecarga() - Mathf.RoundToInt((Time.realtimeSinceStartup - personaje.GetComponent<SnakeMovementReal>().getLastTime()));
            GameObject.FindGameObjectWithTag("TextReload1").GetComponent<Text>().text = "Recargando: " + timeToAction + "\"";
        }
        else if (personaje != null && personaje.GetComponent<SnakeMovementReal>().isPoderActivo())
        {
            float timeToAction = personaje.GetComponent<SnakeMovementReal>().getTDuracion() - Mathf.RoundToInt((Time.realtimeSinceStartup - personaje.GetComponent<SnakeMovementReal>().getLastTime()));
            GameObject.FindGameObjectWithTag("TextReload1").GetComponent<Text>().text = "Duración Poder: " + timeToAction + "\"";
        }
        else
        {
            GameObject.FindGameObjectWithTag("TextReload1").GetComponent<Text>().text = "Poder Listo";
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
