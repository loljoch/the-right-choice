using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AdvancedMessage
{
    public string characterName;
    public string textMessage;
    public Color textColor;
    public Sprite profilePicture;

    public AdvancedMessage(string newCharacterName, string newTextMessage, Color newTextColor, Sprite newProfilePicture)
    {
        characterName = newCharacterName;
        textMessage = newTextMessage;
        textColor = newTextColor;
        profilePicture = newProfilePicture;

    }
}
