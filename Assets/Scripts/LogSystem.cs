using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LogSystem : MonoBehaviour
{
    public static LogSystem instance;
    public int maxMessages = 5;

    [SerializeField]
    List<Message> messageList = new List<Message>();

    public GameObject chatPanel, textObject;

    // Start is called before the first frame update
    void Start()
    {
        instance = this;
    }

    // Update is called once per frame
    void Update()
    {
        //if (Input.GetKeyDown(KeyCode.Space)) SendMessageToChat("You pressed space!");
    }

    private void SendMessageToChat(string text)
    {
        if (messageList.Count >= maxMessages)
        {
            Destroy(messageList[0].textObject.gameObject);
            messageList.Remove(messageList[0]);
        }
        Message newMessage = new Message();

        newMessage.text = text;

        GameObject newText = Instantiate(textObject, chatPanel.transform);
        newMessage.textObject = newText.GetComponent<Text>();
        newMessage.textObject.text = newMessage.text;

        messageList.Add(newMessage);
    }

    public static void SendMessageToChat_Static(string text)
    {
        instance.SendMessageToChat(">" + text);
    }

}

[System.Serializable]
public class Message
{
    public string text;
    public Text textObject;
}
