using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManagement : MonoBehaviour
{
    // Carga la escena requerida. Es llamado desde la interfaz del menú
    public void cargarNivel(string nombreNivel)
    {
        SceneManager.LoadScene(nombreNivel);
    }

    // Cierra el programa. Es llamado al pulsar la opción salir del menú
    public void salir()
    {
        Application.Quit();
    }
}
