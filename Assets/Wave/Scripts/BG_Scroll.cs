using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BG_Scroll : MonoBehaviour
{
    // Retardo del movimiento
    public float parralax = 2f; 
    public float scroll_Speed = 0.05f;
    public bool invertMovement = false;

    private MeshRenderer mesh_Renderer;

    private GameObject objeto = null;
    private MenuAnimation menu = null;
    private void Start()
    {
        // Guardar menú en variable

        objeto = GameObject.Find("MenuAnimation");
        if (objeto != null)
        {
            menu = objeto.GetComponent<MenuAnimation>();
        }

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
            offset.x = -(position.x / transform.localScale.x / parralax);
            offset.y = -(position.y / transform.localScale.y / parralax);
        }
        else
        {
            this.transform.position = position;
            offset.x = position.x / transform.localScale.x / parralax;
            offset.y = position.y / transform.localScale.y / parralax;
        }

        mat.mainTextureOffset = offset;
    }
    // Update is called once per frame
    void Update()
    {
        if (menu != null) // Parallax del fondo en el menú
        {
            mesh_Renderer = GetComponent<MeshRenderer>();
            Material mat = mesh_Renderer.material;
            Vector2 offset = mat.mainTextureOffset;
            if (invertMovement)
            {
                transform.Translate(-menu.positionX * Time.deltaTime, -menu.positionZ * Time.deltaTime, 0f);
                offset.x = -(menu.transform.position.x / transform.localScale.x / parralax);
                offset.y = -(menu.transform.position.z / transform.localScale.y / parralax);
            }
            else
            {
                transform.Translate(menu.positionX * Time.deltaTime, menu.positionZ * Time.deltaTime, 0f);
                offset.x = menu.transform.position.x / transform.localScale.x / parralax;
                offset.y = menu.transform.position.z / transform.localScale.y / parralax;
            }

            mat.mainTextureOffset = offset;
        }
    }
}
