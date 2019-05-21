using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Game : MonoBehaviour
{

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


    // Start is called before the first frame update
    void Start()
    {
        staticUp = Up;

        GameObject g;
        for(int i=0; i<UpsNumber; i++)
        {
            g = Instantiate(Up);
            g.transform.position = new Vector3(Random.Range(-7000, 7000), Random.Range(-7000, 7000), 0);
        }

        GameObject s;
        for (int i = 0; i < SnakeNumber; i++)
        {
            s = Instantiate(snake);
            s.transform.position = new Vector3(Random.Range(-7000, 7000), Random.Range(-7000, 7000), 0);
        }

        GameObject p;

        p = Instantiate(snakep);
        p.transform.position = new Vector3(Random.Range(-7000, 7000), Random.Range(-7000, 7000), 0);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public int GetUpsNumber() => UpsNumber;

    public static void GenerateUp()
    {
        GameObject g = Instantiate(staticUp);
        g.transform.position = new Vector3(Random.Range(-7000, 7000), Random.Range(-7000, 7000), 0);
    }




}
