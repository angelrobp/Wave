using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BG_Scroll : MonoBehaviour
{
    // Retardo del movimiento
    public float parallax = 2f; 
    public float scroll_Speed = 10f;
    public bool invertMovement = false;
    public bool menu = false;

    private MeshRenderer mesh_Renderer;
    
    private void Start()
    {

    }

    //Seguimiento del jugador y parallax
    public void move(Vector3 position)
    {
        mesh_Renderer = GetComponent<MeshRenderer>();
        Material mat = mesh_Renderer.material;
        Vector2 offset = mat.mainTextureOffset;
        if (invertMovement)
        {
            this.transform.position = position;
            offset.x = -(position.x / transform.localScale.x / parallax);
            offset.y = -(position.y / transform.localScale.y / parallax);
        }
        else
        {
            this.transform.position = position;
            offset.x = position.x / transform.localScale.x / parallax;
            offset.y = position.y / transform.localScale.y / parallax;
        }

        mat.mainTextureOffset = offset;
    }
    // Update is called once per frame
    void Update()
    {
        
        if (menu) // Parallax del fondo en el menú
        {
            
            mesh_Renderer = GetComponent<MeshRenderer>();
            Material mat = mesh_Renderer.material;
            Vector2 offset = mat.mainTextureOffset;
            if (invertMovement)
            {
                //transform.Translate(-menu.positionX * Time.deltaTime, -menu.positionZ * Time.deltaTime, 0f);
                //offset.x = -(menu.transform.position.x / transform.localScale.x / parralax);
                offset.x = 0f;
                //offset.y = -((scroll_Speed * Time.deltaTime) / transform.localScale.y / parralax);
                offset.y = -(scroll_Speed * Time.time)/parallax;
            }
            else
            {
                //transform.Translate(menu.positionX * Time.deltaTime, menu.positionZ * Time.deltaTime, 0f);
                //offset.x = menu.transform.position.x / transform.localScale.x / parralax;
                offset.x = 0f;
                //offset.y = (scroll_Speed * Time.deltaTime) / transform.localScale.y / parralax;
                offset.y = scroll_Speed * Time.time / parallax;
            }

            mat.mainTextureOffset = offset;
        }
    }
}
