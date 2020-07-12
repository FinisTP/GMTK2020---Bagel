using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class TextTrigger : MonoBehaviour
{
    public TextBehaviour text;
    
    void Start()
    {
        StartCoroutine(WaitForLoading());
    }

    IEnumerator WaitForLoading()
    {
        yield return new WaitForSeconds(1f);
        TriggerText();
    }
    public void TriggerText()
    {

    }
}
