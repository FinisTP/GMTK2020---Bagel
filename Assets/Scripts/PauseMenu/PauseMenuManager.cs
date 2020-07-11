using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PauseMenuManager : MonoBehaviour
{
    public Slider sound, music;
    public Animator animator;
    public void StartPauseMenu()
    {
        animator.SetBool("IsOpen", true);
    }
    void Update()
    {
        //Set sound to sound slider value
        //Set music to music slider value
    }
    public void ClickOn_BackToMenu()
    {

    }

    public void ClickOn_Resume()
    {
        animator.SetBool("IsOpen", false);
    }
}
