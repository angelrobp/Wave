using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SelectCharacter : MonoBehaviour
{
    private int selectedCharacterIndex = 1;
    private Color desiredColor;

    private GameObject objectEstadoJuego;
    private EstadoJuego estadoJuego;

    [Header("UI References")]
    [SerializeField] private TextMeshProUGUI characterNamePower;
    [SerializeField] private Image characterObject;
    [SerializeField] private Image backgroundColor;
    [SerializeField] public GameObject personaje;

    [Header("Sounds")]
    [SerializeField] private AudioClip arrowClickSFX;
    [SerializeField] private AudioClip characterSelectMusic;

    private void Start ()
    {

        objectEstadoJuego = GameObject.FindGameObjectWithTag("EstadoJuego");
        estadoJuego = objectEstadoJuego.GetComponent<EstadoJuego>();

        selectedCharacterIndex = estadoJuego.personajeSeleccionado.power;

        personaje = GameObject.FindGameObjectWithTag("SelectorPersonaje");
        personaje.SetActive(false);

        characterNamePower = TextMeshProUGUI.FindObjectOfType<TextMeshProUGUI>();
        UpdateCharacterSelectionUI();
    }

    public void SelectCharacterButton()
    {
        //personaje.SetActive(false);

        estadoJuego.personajeSeleccionado.setPower(estadoJuego.getCharactersList()[selectedCharacterIndex].power);
        estadoJuego.personajeSeleccionado.setCharacterNamePower(estadoJuego.getCharactersList()[selectedCharacterIndex].characterNamePower);
        estadoJuego.personajeSeleccionado.setCharacterColor(estadoJuego.getCharactersList()[selectedCharacterIndex].characterColor);
        AudioManager.Instance.PlaySFX(characterSelectMusic);
    }

    public void BackButton()
    {
        AudioManager.Instance.PlaySFX(arrowClickSFX);
    }

    public void LeftArrow()
    {
        personaje.SetActive(false);
        selectedCharacterIndex--;
        if (selectedCharacterIndex < 0)
        {
            selectedCharacterIndex = estadoJuego.getCharactersList().Count - 1;
        }
        UpdateCharacterSelectionUI();
        AudioManager.Instance.PlaySFX(arrowClickSFX);
    }

    public void RightArrow()
    {
        personaje.SetActive(false);
        selectedCharacterIndex++;
        if (selectedCharacterIndex == estadoJuego.getCharactersList().Count)
        {
            selectedCharacterIndex = 0;
        }
        UpdateCharacterSelectionUI();
        AudioManager.Instance.PlaySFX(arrowClickSFX);
    }

    private void UpdateCharacterSelectionUI()
    {
        personaje.SetActive(true);
        characterNamePower.text = "Poder especial: " + estadoJuego.getCharactersList()[selectedCharacterIndex].characterNamePower;
        personaje.GetComponent<Renderer>().material.color = estadoJuego.getCharactersList()[selectedCharacterIndex].characterColor;
    }
}
