using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowCamera : MonoBehaviour
{
    [SerializeField]
    private GameObject camera = null;

    // Start is called before the first frame update
    void Start()
    {
        camera = GameObject.FindGameObjectWithTag("MainCamera");
    }

    // Update is called once per frame
    void Update()
    {
        camera.transform.position = new Vector3(gameObject.transform.position.x, gameObject.transform.position.y, camera.transform.position.z);

        zoom();
        
    }

    public void zoom()
    {

        int vida = this.GetComponent<SnakePlayer>().GetVida();

        if(camera.GetComponent<Camera>().orthographicSize <= vida+1300)
            camera.GetComponent<Camera>().orthographicSize *= 1.00025f;
    }
}
