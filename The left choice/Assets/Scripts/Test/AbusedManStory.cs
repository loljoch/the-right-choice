﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AbusedManStory : MonoBehaviour
{
    //All items to do with projecting the message
    [SerializeField] private Text storyBox;
    [SerializeField] private Text leftName, rightName;
    private List<PersonScript> personScriptList;


    [SerializeField] private List<Button> buttonList;
    [SerializeField] private List<string> buttonTextList;
    private List<Message> messageList;
    private int messageNumber = -1;


    [SerializeField] private float timeBetweenMessages, standardTimeBetweenMessages;
    private WaitForSeconds waitTime;
    [SerializeField] private bool isWriting = false;
    [SerializeField] private bool isInChoice = false;
    [SerializeField] private bool isInFirstPick = true;
    private int chatBranch = 0;


    private void Start()
    {
        AssignButtons();
        AssignPersons();

        waitTime = new WaitForSeconds(timeBetweenMessages);
        buttonTextList = new List<string>();
        messageList = new List<Message>();

        //Sets the starting messages
        messageList.Add(new Message("Persoon 1", "Hallo, ik ben persoon 1", 1));
        messageList.Add(new Message("Persoon 2", "Hallo, ik ben persoon 2", 2));
        messageList.Add(new Message("Persoon 3", "Hallo, ik ben persoon 3", 3));
        messageList.Add(new Message("Persoon 4", "Hallo, ik ben persoon 4", 4));
        messageList.Add(new Message("Persoon 5", "Hallo, ik ben persoon 5", 5));

        //Sets the starting buttons
        buttonTextList.Add("Ja sgoed");
        buttonTextList.Add("Nee liever niet ik heb betere dingen te doen");
        buttonTextList.Add("Alleen als jij mij daarna ook helpt");
        buttonTextList.Add("ok");

    }

    //Spawns the choice buttons
    private void SpawnButtons()
    {
        for (int i = 0; i < buttonTextList.Count; i++)
        {
            buttonList[i].GetComponentInChildren<Text>().text = buttonTextList[i];
            buttonList[i].gameObject.SetActive(true);
        }
    }

    //Assigns the button list
    private void AssignButtons()
    {
        GameObject[] tempButtonList = GameObject.FindGameObjectsWithTag("Button");
        buttonList = new List<Button>();
        for (int i = 0; i < tempButtonList.Length; i++)
        {
            buttonList.Add(tempButtonList[i].GetComponent<Button>());
            tempButtonList[i].SetActive(false);
        }

    }


    //Assigns the person list
    private void AssignPersons()
    {
        GameObject[] tempPersonList = GameObject.FindGameObjectsWithTag("Person");
        personScriptList = new List<PersonScript>();
        for (int i = 0; i < tempPersonList.Length; i++)
        {
            personScriptList.Add(tempPersonList[i].GetComponent<PersonScript>());
        }

    }

    public void WriteMessage()
    {
        for (int i = 0; i < personScriptList.Count; i++)
        {
            personScriptList[i].speechBubble.SetActive(false);
        }

        for (int i = 0; i < personScriptList.Count; i++)
        {
            //Finds which person it is
            if (personScriptList[i].personNumber == messageList[messageNumber].characterNumber)
            {
                //Sets color of message text
                storyBox.color = personScriptList[i].textColor;

                //Activates the matching speech bubble;
                personScriptList[i].speechBubble.SetActive(true);

                //Sets the name on the right or left depending on position of character
                if (personScriptList[i].isPersonRight)
                {
                    leftName.gameObject.SetActive(false);
                    rightName.gameObject.SetActive(true);
                    rightName.color = personScriptList[i].textColor;
                    rightName.text = messageList[messageNumber].characterName;
                } else
                {
                    leftName.gameObject.SetActive(true);
                    rightName.gameObject.SetActive(false);
                    leftName.color = personScriptList[i].textColor;
                    leftName.text = messageList[messageNumber].characterName;
                }

            }
        }

        //Starts the typewriter effect
        StartCoroutine(PlayMessage());


    }


    //Sets the message into the box
    IEnumerator PlayMessage()
    {
        storyBox.text = "";
        foreach (char c in messageList[messageNumber].textMessage)
        {
            storyBox.text += c;
            yield return waitTime;
            waitTime = new WaitForSeconds(timeBetweenMessages);
        }

        isWriting = false;
        timeBetweenMessages = standardTimeBetweenMessages;

        //Spawn buttons
        if (messageNumber == messageList.Count-1)
        {
            SpawnButtons();
            isInChoice = true;
        } else
        {
            isInChoice = false;
        }
    }



    //Goes to previous message
    public void PreviousMessage()
    {
        if (!isWriting && (messageNumber-1 >= 0))
        {
            isWriting = true;
            messageNumber--;
            WriteMessage();
        } else if (!isInChoice)
        {
            timeBetweenMessages = 0;
        }
        
    }

    public void NextMessage()
    {
        if (!isWriting && !isInChoice)
        {
            isWriting = true;
            messageNumber++;
            WriteMessage();
        }else if (!isInChoice)
            {
            timeBetweenMessages = 0;
        }
    }


    public void NextPartOfStory(int buttonPressed)
    {
        if (isInFirstPick)
        {
            int tempButtonPress = buttonPressed;
        } else
        {
            messageList.Clear();
            buttonTextList.Clear();
            messageNumber = -1;
            isInChoice = false;

            //Deactivates buttons after press
            for (int i = 0; i < buttonList.Count; i++)
            {
                buttonList[i].gameObject.SetActive(false);
            }

            //Decides in which chatbranch you are (which story you're following)
            switch (chatBranch)
            {
                case 0:
                    //Decides which button is pressed to get the next story
                    switch (buttonPressed)
                    {
                        case 0:
                            chatBranch = 1;
                            messageList.Add(new Message("Ashley", "Ja sgoed", 1));
                            messageList.Add(new Message("Rick", "Top, dankjewel", 1));
                            messageList.Add(new Message("Tijd", "Paar dagen later", 2));
                            messageList.Add(new Message("Rick", "Hey, ik weet dat je het druk hebt, maar zou je me weer kunnen helpen met mijn huiswerk?", 4));

                            buttonTextList.Add("Nee, dat gaat echt niet nu");
                            buttonTextList.Add("Sorry, ik heb het nu echt heel druk");
                            buttonTextList.Add("Alleen als jij mij daarna ook helpt");
                            break;

                            //case 1:
                            //    messageList.Add(new AdvancedMessage("Ashley", "YEET", leftColor, ashley));
                            //    messageList.Add(new AdvancedMessage("Rick", "YEET", rightColor, rick));
                            //    messageList.Add(new AdvancedMessage("Tijd", "YEET", timeColor, rick));
                            //    messageList.Add(new AdvancedMessage("Rick", "???", rightColor, rick));

                            //    buttonTextList.Add("YEET");
                            //    buttonTextList.Add("YEET");
                            //    buttonTextList.Add("YEET");
                            //    StartTextChat();
                            //    break;

                            //default:
                            //    break;
                    }
                    break;

                default:
                    break;
            }



        }
    }
}
