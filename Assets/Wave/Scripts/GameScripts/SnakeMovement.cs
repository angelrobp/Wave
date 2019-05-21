using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SnakeMovement : MonoBehaviour
{

    private enum Direction { Left, Right, Up, Down, Nothing }
    private int vida = 0;
    public float vel = 100.0f;

    private Direction direction;

    // Start is called before the first frame update
    void Start()
    {
        direction = Direction.Nothing;
    }

    // Update is called once per frame
    void Update()
    {
        
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            direction = Direction.Left;
        }
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            direction = Direction.Right;
        }
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            direction = Direction.Up;
        }
        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            direction = Direction.Down;
        }


       
        if (direction == Direction.Left)
        {
            Vector3 position = this.transform.position;
            position.x -= vel * Time.deltaTime;
            this.transform.position = position;
        }
        if (direction == Direction.Right)
        {
            Vector3 position = this.transform.position;
            position.x += vel * Time.deltaTime;
            this.transform.position = position;
        }
        if (direction == Direction.Up)
        {
            Vector3 position = this.transform.position;
            position.y += vel * Time.deltaTime;
            this.transform.position = position;
        }
        if (direction == Direction.Down)
        {
            Vector3 position = this.transform.position;
            position.y -= vel * Time.deltaTime;
            this.transform.position = position;
        }

        if(vida > 60)
        {
            Vector3 aux = transform.localScale;

            aux.y += 50.0f;
            transform.localScale = aux;

            vida = 0;
        }

    }



    public int GetVida() => vida;

    private void OnTriggerEnter(Collider other)
    {
        
        Destroy(other.gameObject);
        Game.GenerateUp();
        vida += 20;

    }
}
