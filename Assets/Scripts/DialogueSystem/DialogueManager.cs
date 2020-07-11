using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DialogueManager : MonoBehaviour
{
    public Text nameText;
    public Text dialogueText;
    public Animator animator;
    private List<string> sentences;
    public bool continueButton;
    public int index = 0;
    string[] ans = null;
    public List<GameObject> gameObjects;
    void Start()
    {

    }
    private void Update()
    {

    }
}
