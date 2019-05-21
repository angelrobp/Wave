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
    private MainCharacter personaje = null;
    private MenuAnimation menu = null;
    private void Start()
    {
        // Guardar personaje y menú en variables
        objeto = GameObject.Find("MainCharacter");
        if (objeto != null)
        {
            personaje = objeto.GetComponent<MainCharacter>();
        }

        objeto = GameObject.Find("MenuAnimation");
        if (objeto != null)
        {
            menu = objeto.GetComponent<MenuAnimation>();
        }

    }

    // Update is called once per frame
    void Update()
    {
        // Dirección del scroll de la textura del fondo en función de la dirección del personaje
        if (personaje != null)
        {
            mesh_Renderer = GetComponent<MeshRenderer>();
            Material mat = mesh_Renderer.material;
            Vector2 offset = mat.mainTextureOffset;
            if (invertMovement)
            {
                transform.Translate(-personaje.positionX * Time.deltaTime, -personaje.positionZ * Time.deltaTime, 0f);
                offset.x = -(personaje.transform.position.x / transform.localScale.x / parralax);
                offset.y = -(personaje.transform.position.z / transform.localScale.y / parralax);
            }
            else
            {
                transform.Translate(personaje.positionX * Time.deltaTime, personaje.positionZ * Time.deltaTime, 0f);
                offset.x = personaje.transform.position.x / transform.localScale.x / parralax;
                offset.y = personaje.transform.position.z / transform.localScale.y / parralax;
            }
            
            mat.mainTextureOffset = offset;

            
        }
        else if (menu != null) // Dirección del scroll de la textura del fondo en función de la dirección del menú
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
