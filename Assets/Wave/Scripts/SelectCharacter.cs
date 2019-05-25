using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SelectCharacter : MonoBehaviour
{
    private int selectedCharacterIndex = 1;
    private Color desiredColor;

    [Header("List of characters")]
    [SerializeField]
    private List<CharacterSelectObject> characterList = new List<CharacterSelectObject>();

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
        for (int i=0; i<characterList.Count; i++)
        {
            personaje = characterList[i].personaje;
            personaje.SetActive(false);
        }

        characterNamePower = TextMeshProUGUI.FindObjectOfType<TextMeshProUGUI>();
        UpdateCharacterSelectionUI();
    }

    public void LeftArrow()
    {
        personaje.SetActive(false);
        selectedCharacterIndex--;
        if (selectedCharacterIndex < 0)
        {
            selectedCharacterIndex = characterList.Count - 1;
        }
        UpdateCharacterSelectionUI();
    }

    public void RightArrow()
    {
        personaje.SetActive(false);
        selectedCharacterIndex++;
        if (selectedCharacterIndex == characterList.Count)
        {
            selectedCharacterIndex = 0;
        }
        UpdateCharacterSelectionUI();
    }

    private void UpdateCharacterSelectionUI()
    {/*
        switch (selectedCharacterIndex)
        {
            case 1:
                personaje = GameObject.Find("PersonajeVelocidad");
                personaje.SetActive(true);
                break;
            case 2:
                personaje = GameObject.Find("PersonajeInvisibilidad");
                personaje.SetActive(true);
                break;
            case 3:
                personaje = GameObject.Find("PersonajeRepulsion");
                personaje.SetActive(true);
                break;
        }
        */
        personaje = characterList[selectedCharacterIndex].personaje;
        personaje.SetActive(true);
        characterNamePower.text = characterList[selectedCharacterIndex].characterNamePower;
        personaje.GetComponent<Renderer>().material.color = characterList[selectedCharacterIndex].characterColor;


    }

    [System.Serializable]
    public class CharacterSelectObject
    {
        public GameObject personaje;
        public string characterNamePower;
        public Color characterColor;

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
