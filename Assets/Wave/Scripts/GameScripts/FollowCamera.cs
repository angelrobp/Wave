using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowCamera : MonoBehaviour
{
    [SerializeField]
    private GameObject camera = null;
    private GameObject background, stars, stars_I = null;

    // Start is called before the first frame update
    void Start()
    {
        camera = GameObject.FindGameObjectWithTag("MainCamera");
        background = GameObject.FindGameObjectWithTag("Background");
        stars = GameObject.FindGameObjectWithTag("BG_Stars");
        stars_I = GameObject.FindGameObjectWithTag("BG_Stars_Invert");
    }

    // Update is called once per frame
    void Update()
    {
        camera.transform.position = new Vector3(gameObject.transform.position.x, gameObject.transform.position.y, camera.transform.position.z);
        background.GetComponent<BG_Scroll>().move(new Vector3(gameObject.transform.position.x, gameObject.transform.position.y, 120f));
        stars.GetComponent<BG_Scroll>().move(new Vector3(gameObject.transform.position.x, gameObject.transform.position.y, 110f));
        stars_I.GetComponent<BG_Scroll>().move(new Vector3(gameObject.transform.position.x, gameObject.transform.position.y, 110f));
        //background.move(gameObject.transform.position.x, gameObject.transform.position.y, camera.transform.position.z);
        zoom();
        
    }

    public void zoom()
    {

        int vida = this.GetComponent<SnakePlayer>().GetVida();

        if(camera.GetComponent<Camera>().orthographicSize <= vida+1300)
        {
            camera.GetComponent<Camera>().orthographicSize *= 1.00025f;
            background.GetComponent<BG_Scroll>().transform.localScale *= 1.00025f;
            stars.GetComponent<BG_Scroll>().transform.localScale *= 1.00025f;
            stars_I.GetComponent<BG_Scroll>().transform.localScale *= 1.00025f;
        }
            
    }
}
