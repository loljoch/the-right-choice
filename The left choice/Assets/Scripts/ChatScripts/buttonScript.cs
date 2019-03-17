using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class buttonScript : MonoBehaviour
{
    public int buttonValue;
    [SerializeField] private Image rightWrongColor;
    private MessageList buttonContent;
    private SecondPick secondButtonContent;
    public bool inFirstPick;
    public GameObject[] buttonList;

    private void Start()
    {
        buttonContent = GameObject.FindGameObjectWithTag("Scenemaker").GetComponent<MessageList>();
        secondButtonContent = GameObject.FindGameObjectWithTag("Secondpick").GetComponent<SecondPick>();
    }

    public void NextLog()
    {
        //If the first player picks
        if (inFirstPick == true)
        {
            //Gives the scenemaker information about which button is pressed
            secondButtonContent.buttonValue = buttonValue;
            buttonList = GameObject.FindGameObjectsWithTag("Button");
            secondButtonContent.buttonList = buttonList;
            secondButtonContent.SecondButtons();
            Debug.Log("Yeet");
            foreach (GameObject button in buttonList)
            {
                Destroy(button);
            }
        } 
        
        //If the group picks
        else if(inFirstPick == false)
        {
            //Tests if button is the same or not
            Debug.Log("NietYeet");
            if (secondButtonContent.buttonValue == buttonValue)
            {
                Image newColor = Instantiate(rightWrongColor, transform.parent.parent);
                newColor.color = new Color(0, 255, 0, 255);
            } 
            
            else
            {
                Image newColor = Instantiate(rightWrongColor, transform.parent.parent);
                newColor.color = new Color(255, 0, 0, 255);
            }

            //Makes story continue
            secondButtonContent.NextLog();

            //Deletes all buttons
            buttonList = GameObject.FindGameObjectsWithTag("Button");
            foreach (GameObject button in buttonList)
            {
                Destroy(button);
            }
        }
    }
}
