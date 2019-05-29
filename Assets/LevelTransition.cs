using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelTransition : MonoBehaviour
{
    public Animator animator;
    private LevelManagement levelManagement;
    private string levelToLoad;
    private Game game;
    private Text texto;

    // Start is called before the first frame update
    void Start()
    {
        game = GameObject.FindGameObjectWithTag("game").GetComponent<Game>();
        levelManagement = GameObject.Find("EventSystem").GetComponent<LevelManagement>();
        texto = GameObject.Find("FinalText").GetComponent<Text>();
        texto.text = "Has perdido";
    }

    // Update is called once per frame
    void Update()
    {
        if (game.isEndGame())
        {
            if (game.isWinGame())
            {
                texto.GetComponent<Text>().color = new Color(.07f, 1f, .427f);
                FadeToLevel("Menu", "Has ganado");
            }
            else {
                texto.GetComponent<Text>().color = new Color(1f, .08235294f, 0f);
                FadeToLevel("Menu", "Has perdido");
            }
            
        }
            
    }

    public void FadeToLevel (string levelIndex, string newText)
    {
        levelToLoad = levelIndex;
        texto.text = newText;
        
        animator.SetTrigger("Fade_Out");

    }

    public void OnFadeComplete(string levelIndex)
    {
        levelManagement.cargarNivel(levelToLoad);
    }
}
