using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EndingType
{
    gameOver,
    keepItAlive,
    keepYouAlive,
    keepHerAlive,
    keepHimAlive,
    keepThemAlive,
    keepUsAlive,
    keepMeAlive
}

public class EndingManager : MonoBehaviour
{
    public TextTrigger transitionText;
    public EndingType endingType;
    public TextBehaviour endingText;
    // Start is called before the first frame update
    void Start()
    {
        //GameObject.Find("DialogueManager").GetComponent<DialogueManager>().sendMessage("System", "You got ending " + endingType.ToString());
        transitionText.text.sentences = endingText.sentences;
    }
}
