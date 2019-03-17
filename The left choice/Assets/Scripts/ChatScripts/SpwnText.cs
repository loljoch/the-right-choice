using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SpwnText : MonoBehaviour
{

    [SerializeField] private GameObject messagePrefab;


    public void WriteMessage(AdvancedMessage message)
    {
        Debug.Log("WroteMessage");
        GameObject newMessage = Instantiate(messagePrefab);
        newMessage.GetComponent<Text>().text = message.textMessage;
        newMessage.GetComponent<Text>().color = message.textColor;
        newMessage.GetComponentInChildren<Image>().sprite = message.profilePicture;
        Text[] childrenComponents = newMessage.GetComponentsInChildren<Text>();
        foreach (Text textComponent in childrenComponents)
        {
            if (textComponent.gameObject != newMessage)
            {
                textComponent.text = message.characterName;
                textComponent.color = message.textColor;
            }
        }

        //Sets the parent of the new gameobject to be the same as the textemplate itself
        newMessage.transform.SetParent(this.transform, false);


    }
}
