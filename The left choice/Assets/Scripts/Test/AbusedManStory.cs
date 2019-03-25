using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AbusedManStory : MonoBehaviour
{
    [SerializeField] private Sprite[] availableSprites;


    //All items to do with projecting the message
    [SerializeField] private Text storyBox;
    [SerializeField] private Text leftName, rightName;
    [SerializeField]private List<PersonScript> personScriptList;

    //Choice button menu
    [SerializeField] private Animator choiceMenuAnimation;
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
    private SceneSetup nextSceneSetup;
    private int pressedButton = 0;

    private PlayerManagerScript playerManager;
    [SerializeField] private GameObject playerAnswerFeedback;
    [SerializeField] Color colorWrong, colorRight;


    private void Start()
    {
        playerManager = GameObject.FindGameObjectWithTag("GameController").GetComponent<PlayerManagerScript>();

        AssignButtons();
        nextSceneSetup = new SceneSetup(false, availableSprites[0], availableSprites[1], availableSprites[2], availableSprites[3], availableSprites[4]);
        NextScene(nextSceneSetup);

        waitTime = new WaitForSeconds(timeBetweenMessages);
        buttonTextList = new List<string>();
        messageList = new List<Message>();

        //Sets the starting messages
        messageList.Add(new Message("Felix", "Hey, daar zijn ze eindelijk hoor.", 4));

        //This is how to skip after a message
        //messageList.Add(new Message("Persoon 1", "~", 1));
        //nextSceneSetup = new SceneSetup(false, null, availableSprites[1], availableSprites[2], availableSprites[3], availableSprites[4]);
        messageList.Add(new Message("Mikey", "~", 1));
        nextSceneSetup = new SceneSetup(false, null, availableSprites[1], availableSprites[2], availableSprites[3], availableSprites[4]);
        messageList.Add(new Message("Mikey", "Ey, iedereen lang niet gezien, hoe gaat het met jullie?", 1));
        messageList.Add(new Message("Felix", "Ahh, met mij loopt het altijd wel op rolletjes, dat weet je toch.", 4));
        
        messageList.Add(new Message("Samantha", "Ik heb wat struggles op school, maar verder gaat het wel goed. Met jullie?", 3));
        messageList.Add(new Message("Mikey", "Nou, Laura en ik ga-", 1));
        messageList.Add(new Message("Laura", "Gaat super, we hebben zelfs een nieuwtje om met jullie te delen.", 0));

        //Sets the starting buttons
        buttonTextList.Add("Laura terug onderbreken");
        buttonTextList.Add("Laura niet terug onderbreken");

    }

    //Spawns the choice buttons
    public void SpawnButtons(bool spawnButtons)
    {
        if (!spawnButtons)
        {
            choiceMenuAnimation.SetBool("Open", true);
        } else {
            for (int i = 0; i < buttonTextList.Count; i++)
            {
                buttonList[i].GetComponentInChildren<Text>().text = buttonTextList[i];
                buttonList[i].gameObject.SetActive(true);
            }
            choiceMenuAnimation.GetComponentInChildren<Text>().text = "Speler " + playerManager.CalculatePlayerTurn() + "\n" + "Wat zou jij doen in deze situatie?";
            
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
            tempPersonList[i].SetActive(true);
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
        //Checks if it's a next scene message
        bool skipRoutine = false;
        foreach (char c in messageList[messageNumber].textMessage)
        {
            string tempString = c+"";
            Debug.Log(tempString);
            if(tempString != "~")
            {
                break;
            } else
            {
                NextScene(nextSceneSetup);
                skipRoutine = true;

                break;
            }
        }

        //Typewriter effect
        storyBox.text = "";
        foreach (char c in messageList[messageNumber].textMessage)
        {
            storyBox.text += c;
            yield return waitTime;
            waitTime = new WaitForSeconds(timeBetweenMessages);
        }

        isWriting = false;
        if (skipRoutine == true)
        {
            NextMessage();
        }

        timeBetweenMessages = standardTimeBetweenMessages;
            

        //Spawn buttons
        if (messageNumber == messageList.Count-1)
        {
            SpawnButtons(false);
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

    //Goes to next message
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
            isInFirstPick = false;
            pressedButton = buttonPressed;
            choiceMenuAnimation.GetComponentInChildren<Text>().text = "Andere spelers" + "\n" + "Wat denken jullie dat " + "Speler " + playerManager.PreviousPlayerTurn() + " heeft gekozen?";
        } else
        {
            //Deactivates all the buttons
            choiceMenuAnimation.SetBool("Open", false);
            for (int i = 0; i < buttonList.Count; i++)
            {
                buttonList[i].gameObject.SetActive(false);
            }

            //Checks if second team got the choice right
            if(buttonPressed == pressedButton)
            {
                playerAnswerFeedback.SetActive(true);
                playerAnswerFeedback.GetComponent<Text>().text = "Goed" + "\n" + "geraden!";
                playerAnswerFeedback.GetComponent<Text>().color = colorRight;
                playerAnswerFeedback.GetComponent<Animator>().SetBool("PlayAnim", true);
            } else
            {
                playerAnswerFeedback.SetActive(true);
                playerAnswerFeedback.GetComponent<Text>().text = "Fout" + "\n" + "geraden";
                playerAnswerFeedback.GetComponent<Text>().color = colorWrong;
                playerAnswerFeedback.GetComponent<Animator>().SetBool("PlayAnim", true);
            }

            //Sets everything up to continue the story
            messageList.Clear();
            buttonTextList.Clear();
            messageNumber = -1;
            isInChoice = false;
            isInFirstPick = true;


            //Decides in which chatbranch you are (which story you're following)
            switch (chatBranch)
            {
                case 0:
                    //Decides which button is pressed to get the next story
                    switch (pressedButton)
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

                        case 1:
                            chatBranch = 2;
                            messageList.Add(new Message("Ashley", "Ja sgoed", 1));
                            messageList.Add(new Message("Rick", "Top, dankjewel", 1));
                            messageList.Add(new Message("Tijd", "Paar dagen later", 2));
                            messageList.Add(new Message("Rick", "Hey, ik weet dat je het druk hebt, maar zou je me weer kunnen helpen met mijn huiswerk?", 4));

                            buttonTextList.Add("Nee, dat gaat echt niet nu");
                            buttonTextList.Add("Sorry, ik heb het nu echt heel druk");
                            buttonTextList.Add("Alleen als jij mij daarna ook helpt");
                            break;

                            //default:
                            //    break;
                    }
                    break;

                case 1:
                    switch (pressedButton)
                    {
                        case 0:
                            chatBranch = 2;
                            messageList.Add(new Message("Ashley", "Ja sgoed", 1));
                            messageList.Add(new Message("Rick", "Top, dankjewel", 1));
                            messageList.Add(new Message("Tijd", "Paar dagen later", 2));
                            messageList.Add(new Message("Rick", "Hey, ik weet dat je het druk hebt, maar zou je me weer kunnen helpen met mijn huiswerk?", 4));

                            buttonTextList.Add("Nee, dat gaat echt niet nu");
                            buttonTextList.Add("Sorry, ik heb het nu echt heel druk");
                            buttonTextList.Add("Alleen als jij mij daarna ook helpt");
                            break;

                        case 1:
                            chatBranch = 2;
                            messageList.Add(new Message("Ashley", "Ja sgoed", 1));
                            messageList.Add(new Message("Rick", "Top, dankjewel", 1));
                            messageList.Add(new Message("Tijd", "Paar dagen later", 2));
                            messageList.Add(new Message("Rick", "Hey, ik weet dat je het druk hebt, maar zou je me weer kunnen helpen met mijn huiswerk?", 4));

                            buttonTextList.Add("Nee, dat gaat echt niet nu");
                            buttonTextList.Add("Sorry, ik heb het nu echt heel druk");
                            buttonTextList.Add("Alleen als jij mij daarna ook helpt");
                            break;

                            //default:
                            //    break;
                    }
                    break;

                case 2:
                    switch (pressedButton)
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

                        case 1:
                            chatBranch = 2;
                            messageList.Add(new Message("Ashley", "Ja sgoed", 1));
                            messageList.Add(new Message("Rick", "Top, dankjewel", 1));
                            messageList.Add(new Message("Tijd", "Paar dagen later", 2));
                            messageList.Add(new Message("Rick", "Hey, ik weet dat je het druk hebt, maar zou je me weer kunnen helpen met mijn huiswerk?", 4));

                            buttonTextList.Add("Nee, dat gaat echt niet nu");
                            buttonTextList.Add("Sorry, ik heb het nu echt heel druk");
                            buttonTextList.Add("Alleen als jij mij daarna ook helpt");
                            break;

                            //default:
                            //    break;
                    }
                    break;



                default:
                    break;
            }



        }
    }

    //Changes the sprites of the character
    private void NextScene(SceneSetup nextSetup)
    {
        Debug.Log("I made a new scene");
        AssignPersons();
        

        //Assigns the sprites
        for (int i = 0; i < nextSetup.sceneSprites.Count; i++)
        {
            for (int w = 0; w < personScriptList.Count; w++)
            {
                if (personScriptList[w].personNumber == i)
                {
                    personScriptList[w].GetComponent<Image>().sprite = nextSetup.sceneSprites[i];
                    break;
                }
            }
        }

        //Sets the persons without sprites on false
        for (int i = 0; i < personScriptList.Count; i++)
        {
            if (personScriptList[i].GetComponent<Image>().sprite == null)
            {
                personScriptList[i].gameObject.SetActive(false);
            }
        }
    }
}
