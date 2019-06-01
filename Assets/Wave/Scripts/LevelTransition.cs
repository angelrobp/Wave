using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelTransition : MonoBehaviour
{
    public bool transicionEnPartida;
    public Animator animator;
    private LevelManagement levelManagement;
    private string levelToLoad;
    private Game game;
    private Text texto;

    private bool inicioTransicion;

    // Start is called before the first frame update
    void Start()
    {
        if (transicionEnPartida)
        {
            game = GameObject.FindGameObjectWithTag("game").GetComponent<Game>();
            texto = GameObject.Find("FinalText").GetComponent<Text>();
            texto.text = "Has perdido";

            inicioTransicion = false;
        }

        levelManagement = GameObject.Find("EventSystem").GetComponent<LevelManagement>();
    }

    // Update is called once per frame
    void Update()
    {
        if (transicionEnPartida && !inicioTransicion)
        {
            if (game.isEndGame())
            {
                inicioTransicion = true;

                AudioManager.Instance.SetMusicVolume(0);
                if (game.isWinGame()) 
                {
                    //Inicio musica de victoria
                    AudioManager.Instance.PlaySFX(AudioManager.Instance.getClip(5));

                    texto.GetComponent<Text>().color = new Color(.07f, 1f, .427f);
                    FadeToLevel("Menu", "¡Has ganado!", "Fade_Out");
                }
                else {
                    //Inicio musica de derrota
                    AudioManager.Instance.PlaySFX(AudioManager.Instance.getClip(6));

                    texto.GetComponent<Text>().color = new Color(1f, .08235294f, 0f);
                    FadeToLevel("Menu", "Has perdido", "Fade_Out");
                }
            
            }
        }
        
            
    }

    public void FadeToLevel (string levelIndex, string newText, string idTransition)
    {
        levelToLoad = levelIndex;
        texto.text = newText;
        
        animator.SetTrigger(idTransition);

    }

    public void AbandonarPartida()
    {
        AudioManager.Instance.SetMusicVolume(0);
        //Inicio musica de derrota
        AudioManager.Instance.PlaySFX(AudioManager.Instance.getClip(6));
        texto.GetComponent<Text>().color = new Color(1f, .08235294f, 0f);
        FadeToLevel("Menu", "Has perdido", "Fade_Out");

    }

    public void FadeToLevelOptions (string levelIndex)
    {
        levelToLoad = levelIndex;

        animator.SetTrigger("Menu_Fade_Out");

    }

    public void OnFadeComplete(string levelIndex)
    {
        AudioManager.Instance.PlayMusicWithCrossFade(AudioManager.Instance.getClip(0));
        levelManagement.cargarNivel(levelToLoad);
    }

    public void OnFadeCompleteOptions()
    {
        levelManagement.cargarNivel(levelToLoad);
    }
}
