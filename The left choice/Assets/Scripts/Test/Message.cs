using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Message
{
    public string characterName;
    public string textMessage;
    //public Color textColor;
    public int characterNumber;

    //public Message(string newCharacterName, string newTextMessage, Color newTextColor, int newCharacterNumber)
    //{
    //    characterName = newCharacterName;
    //    textMessage = newTextMessage;
    //    textColor = newTextColor;
    //    characterNumber = newCharacterNumber;

    //}

    public Message(string newCharacterName, string newTextMessage, int newCharacterNumber)
    {
        characterName = newCharacterName;
        textMessage = newTextMessage;
        characterNumber = newCharacterNumber;

    }
}
