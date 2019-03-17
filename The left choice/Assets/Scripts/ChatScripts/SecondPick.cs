using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SecondPick : MonoBehaviour
{
    [SerializeField] private MessageList buttonContent;
    [HideInInspector] public GameObject[] buttonList;
    [HideInInspector] public int buttonValue;

    public void SecondButtons()
    {
        //Creates the buttons
        for (int i = 0; i < buttonList.Length; i++)
        {
            GameObject newButton = Instantiate(buttonList[i], this.transform);
            newButton.GetComponent<buttonScript>().inFirstPick = false;
        }
    }


    //Lets new story start
    public void NextLog()
    {
        buttonContent.ButtonPress(buttonValue);
    }
}
