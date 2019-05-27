using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Objecto de juego común para todas las escenas.
//Contiene el personaje seleccionado para jugar.
public class EstadoJuego : MonoBehaviour
{
    public static EstadoJuego estadoJuego;
    public CharacterSelectObject personajeSeleccionado;

    [Header("List of characters")]
    [SerializeField]
    private List<EstadoJuego.CharacterSelectObject> characterList = new List<EstadoJuego.CharacterSelectObject>();

    //Comprobacion y eliminación de copias de este objeto
    void Awake()
    {
        if (estadoJuego == null)
        {
            estadoJuego = this;
            DontDestroyOnLoad(gameObject);

            personajeSeleccionado = new CharacterSelectObject();
            personajeSeleccionado.setPower(0);
            personajeSeleccionado.setCharacterNamePower("Velocidad");
            personajeSeleccionado.setCharacterColor(new Color(1.0f, 0.5251f, 0.0f));
        }
        else if (estadoJuego!= this)
        {
            Destroy(gameObject);
        }
        
    }

    public List<EstadoJuego.CharacterSelectObject> getCharactersList ()
    {
        return characterList;
    }

    [System.Serializable]
    public class CharacterSelectObject
    {
        public int power;
        public string characterNamePower;
        public Color characterColor;

        public void setPower(int newPower)
        {
            power = newPower;
        }

        public void setCharacterNamePower(string newCharacterNamePower)
        {
            characterNamePower = newCharacterNamePower;
        }

        public void setCharacterColor(Color newCharacterColor)
        {
            characterColor = newCharacterColor;
        }
    }
}
