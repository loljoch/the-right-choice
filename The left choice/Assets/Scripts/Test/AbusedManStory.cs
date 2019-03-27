using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class AbusedManStory : MonoBehaviour
{
    [SerializeField] private Sprite[] availableSprites;
    [SerializeField] private Sprite[] backgroundSprites;


    //All items to do with projecting the message
    [SerializeField] private Text storyBox;
    [SerializeField] private Text leftName, rightName;
    [SerializeField] private List<PersonScript> personScriptList;
    [SerializeField] private GameObject[] personList;
    [SerializeField] private Image background;

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
    private List<SceneSetup> nextSceneSetup;
    private int pressedButton = 0;

    private PlayerManagerScript playerManager;
    [SerializeField] private GameObject playerAnswerFeedback;
    [SerializeField] private GameObject sceneTransition;
    [SerializeField] Color colorWrong, colorRight;


    private void Start()
    {
        nextSceneSetup = new List<SceneSetup>();
        personList = GameObject.FindGameObjectsWithTag("Person");
        playerManager = GameObject.FindGameObjectWithTag("GameController").GetComponent<PlayerManagerScript>();

        AssignButtons();


        waitTime = new WaitForSeconds(timeBetweenMessages);
        buttonTextList = new List<string>();
        messageList = new List<Message>();

        nextSceneSetup.Add(new SceneSetup(false, availableSprites[1], availableSprites[3], availableSprites[4], null, null, backgroundSprites[0]));
        StartCoroutine(NextScene(nextSceneSetup[0]));

        //Sets the starting messages
        messageList.Add(new Message("Felix", "Hey, daar zijn ze eindelijk hoor.", 0));

        //This is how to skip after a message
        //messageList.Add(new Message("Persoon 1", "~", 1));
        //nextSceneSetup = new SceneSetup(false, null, availableSprites[1], availableSprites[2], availableSprites[3], availableSprites[4]);

        nextSceneSetup.Add(new SceneSetup(false, availableSprites[1], availableSprites[3], availableSprites[4], availableSprites[0], availableSprites[2], backgroundSprites[0]));
        messageList.Add(new Message("Mikey", "~", 3));
        messageList.Add(new Message("Mikey", "Ey, iedereen lang niet gezien, hoe gaat het met jullie?", 3));
        messageList.Add(new Message("Felix", "Ahh, met mij loopt het altijd wel op rolletjes, dat weet je toch.", 0));
        
        messageList.Add(new Message("Samantha", "Ik heb wat struggles op school, maar verder gaat het wel goed. Met jullie?", 2));
        messageList.Add(new Message("Mikey", "Nou, Laura en ik ga-", 3));
        messageList.Add(new Message("Laura", "Gaat super, we hebben zelfs een nieuwtje om met jullie te delen.", 4));

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
            choiceMenuAnimation.GetComponentInChildren<Text>().text = "Speler " + playerManager.CalculatePlayerTurn() + "\n" + "Wat zou jij doen in deze situatie als je Mikey was?";
            
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
                    leftName.transform.parent.gameObject.SetActive(false);
                    rightName.gameObject.SetActive(true);
                    rightName.transform.parent.gameObject.SetActive(true);
                    rightName.color = personScriptList[i].textColor;
                    rightName.text = messageList[messageNumber].characterName;
                } else
                {
                    leftName.gameObject.SetActive(true);
                    leftName.transform.parent.gameObject.SetActive(true);
                    rightName.gameObject.SetActive(false);
                    rightName.transform.parent.gameObject.SetActive(false);
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
            if(tempString != "~")
            {
                break;
            } else if(tempString == "~")
            {
                StartCoroutine(NextScene(nextSceneSetup[0]));
                skipRoutine = true;

                break;
            } else if(tempString == "#")
            {
                StartCoroutine(EndGame());
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
            foreach (char c in messageList[messageNumber-1].textMessage)
            {
                string tempString = c + "";
                if (tempString != "~")
                {
                    isWriting = true;
                    messageNumber--;
                    WriteMessage();
                    break;
                } else { 
                    break;
                }
            }
            
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

            if (buttonPressed < 3)
            {
                //Checks if second team got the choice right
                if (buttonPressed == pressedButton)
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
                //branch 0
                case 0:
                    //Decides which button is pressed to get the next story
                    switch (pressedButton)
                    {
                        case 0:
                            //Laura terug onderbreken
                            chatBranch = 1;
                            messageList.Add(new Message("Laura", "Mikey en ik gaan name-", 4));
                            messageList.Add(new Message("Mikey", "We gaan samenwonen! Ik trek bij haar in.", 3));
                            messageList.Add(new Message("Samantha", "O, wat leuk! Wanneer ga je verhuizen? Dan kunnen wij wel helpen, toch jongens?", 2));
                            messageList.Add(new Message("Rob", "Ja, tuurlijk helpen we Mikey wel met verhuizen, maar is het niet een beetje snel om nu al bij elkaar in te trekken?", 1));
                            messageList.Add(new Message("Felix", "Ah joh Rob, ze zijn toch al 4 maanden samen. Het leven is kort, vriend. En natuurlijk helpen we je met verhuizen, Mikey!", 0));
                            messageList.Add(new Message("Mikey", "Dat is wel heel aardig van jullie jongens, maar we hebben vandaag al alles verhuisd. Het is alleen nog maar uitpakken nu.", 3));
                            messageList.Add(new Message("Laura", "Yep, mijn stoere bink heeft het helemaal zelf gesjouwd, ook de zwaarste dozen.", 4));
                            messageList.Add(new Message("Samantha", "Wat leuk, ik wed dat jullie het naar je zin gaan hebben samen. Jullie twee zijn zo’n lief koppeltje.", 2));
                            messageList.Add(new Message("Felix", "Ja, jullie zijn inderdaad altijd een stel tortelduiven.", 0));
                            messageList.Add(new Message("Rob", "Daar ben ik het mee eens, maar heb je je appartement al helemaal verlaten?", 1));
                            messageList.Add(new Message("Laura", "Yep, ene Gerry zit nu in zijn appartement, hij wilde gelijk intrekken.", 4));
                            messageList.Add(new Message("Felix", "Awh man, dat was altijd zo’n cool appartement, met die loungehoek die je had gemaakt. Jammer, daar zullen jullie daar nu geen ruimte meer voor hebben. Maar even iets heel anders: hoe gaat het met eigenlijk met jou, Rob?", 0));
                            messageList.Add(new Message("Rob", "Gaat wel goed! Ik heb laatst een loonsverhoging gekregen waardoor ik nu tenminste als een normaal persoon kan avondeten.", 1));
                            nextSceneSetup.Add(new SceneSetup(true, availableSprites[0], null, null, null, availableSprites[2], backgroundSprites[6]));
                            messageList.Add(new Message("Mikey", "~", 0));
                            messageList.Add(new Message("Mikey", "Hè hè, dat was me het avondje wel. Felix kon echt niet ophouden over zijn nieuwe raceautootje. Wat een gozer is-ie toch. Was sowieso echt weer gezellig.", 0));
                            messageList.Add(new Message("Laura", "Haha, ja inderdaad… Maar hé, ik vond het alleen niet zo fijn dat je me onderbrak vanavond.", 4));

                            buttonTextList.Add("Excuses aanbieden");
                            buttonTextList.Add("Duidelijke maken dat je het probleem niet echt ziet");
                            break;

                        case 1:
                            //Laura niet terug onderbreken
                            chatBranch = 2;
                            messageList.Add(new Message("Laura", "Mikey en ik gaan namelijk samenwonen!", 4));
                            messageList.Add(new Message("Samantha", "O, wat leuk! Wanneer ga je verhuizen? Dan kunnen wij wel helpen, toch jongens?", 2));
                            messageList.Add(new Message("Rob", "Ja, tuurlijk helpen we Mikey wel met verhuizen, maar is het niet een beetje snel om nu al bij elkaar in te trekken?", 1));
                            messageList.Add(new Message("Felix", "Ah joh Rob, ze zijn toch al 4 maanden samen. Het leven is kort, vriend. En natuurlijk helpen we je met verhuizen, Mikey!", 0));
                            messageList.Add(new Message("Mikey", "Dat is wel heel aardig van jullie jongens, maar we hebben vandaag al alles verhuisd. Het is alleen nog maar uitpakken nu.", 3));
                            messageList.Add(new Message("Laura", "Yep, mijn stoere bink heeft het helemaal zelf gesjouwd, ook de zwaarste dozen.", 4));
                            messageList.Add(new Message("Samantha", "Wat leuk, ik wed dat jullie het naar je zin gaan hebben samen. Jullie twee zijn zo’n lief koppeltje.", 2));
                            messageList.Add(new Message("Felix", "Ja, jullie zijn inderdaad altijd een stel tortelduiven.", 0));
                            messageList.Add(new Message("Rob", "Daar ben ik het mee eens, maar heb je je appartement al helemaal verlaten?", 1));
                            messageList.Add(new Message("Laura", "Yep, ene Gerry zit nu in zijn appartement, hij wilde gelijk intrekken.", 4));
                            messageList.Add(new Message("Felix", "Awh man, dat was altijd zo’n cool appartement, met die loungehoek die je had gemaakt. Jammer, daar zullen jullie daar nu geen ruimte meer voor hebben. Maar even iets heel anders: hoe gaat het met eigenlijk met jou, Rob?", 0));
                            messageList.Add(new Message("Rob", "Gaat wel goed! Ik heb laatst een loonsverhoging gekregen waardoor ik nu tenminste als een normaal persoon kan avondeten.", 1));
                            nextSceneSetup.Add(new SceneSetup(true, availableSprites[0], null, null, null, availableSprites[2], backgroundSprites[6]));
                            messageList.Add(new Message("Mikey", "~", 0));
                            messageList.Add(new Message("Mikey", "Hè hè, dat was me het avondje wel. Felix kon echt niet ophouden over zijn nieuwe raceautootje. Wat een gozer is-ie toch. Was sowieso echt weer gezellig.", 0));
                            messageList.Add(new Message("Laura", "Haha, ja inderdaad. Je zei alleen niet zoveel over het samenwonen, ben je er wel enthousiast over?", 4));
                            messageList.Add(new Message("Mikey", "Ja, tuurlijk ben ik er enthousiast over babe, ik liet jou het gewoon vertellen.", 0));
                            messageList.Add(new Message("Laura", "Hmm, oké dan. Maar je mag volgende keer best wat inbreng geven hoor. Ik was bang dat je het helemaal niet zo zag zitten om bij me te gaan wonen…", 4));
                            messageList.Add(new Message("Mikey", "Hé sorry, zo bedoelde ik het natuurlijk niet. Ik kijk hier hartstikke naar uit, Lau. Kom, laten we nu maar gauw beginnen met uitpakken, dan kunnen we er echt ons paleisje van maken, samen.", 0));
                            nextSceneSetup.Add(new SceneSetup(true, availableSprites[0], null, null, null, null, backgroundSprites[5]));
                            messageList.Add(new Message("Mikey", "~", 0));
                         
                            messageList.Add(new Message("Mikey", " ", 0));
                            nextSceneSetup.Add(new SceneSetup(false, availableSprites[0], null, null, null, availableSprites[2], backgroundSprites[6]));
                            messageList.Add(new Message("Mikey", "~", 0));
                            
                            messageList.Add(new Message("Mikey", "Hey schat, hoe was je dag?", 0));
                            messageList.Add(new Message("Laura", "Goed hoor… Ik had alleen wel verwacht dat je zou stofzuigen?", 4));
                            messageList.Add(new Message("Mikey", "Dat heb ik daarstraks gedaan, hoezo?", 0));
                            messageList.Add(new Message("Laura", "Nou, daar is dan niets van te merken. Kijk nou hoeveel stof er nog ligt! Je moet het niet zo afraffelen.", 4));


                            buttonTextList.Add("Excuses aanbieden en het nog een keer stofzuigen");
                            buttonTextList.Add("Zeggen dat je hebt gedaan wat ze van je vroeg en dat het onredelijk is dat ze het niet goed genoeg vindt");
                            break;

                            //default:
                            //    break;
                    }
                    break;
                
                //branch1
                case 1:
                    switch (pressedButton)
                    {
                        //Excuses aanbieden
                        case 0:
                            chatBranch = 2;
                            messageList.Add(new Message("Mikey", "O sorry, ik wist niet dat je het zo graag zelf wou vertellen.", 0));
                            messageList.Add(new Message("Laura", "Oké oké, zand erover. Maar als ik het nog een keer zie gebeuren dan mag je op de bank slapen, ja?", 4));
                            messageList.Add(new Message("Mikey", "Haha prima, ik begrijp het.", 0));
                            messageList.Add(new Message("Mikey", "Laten we nou maar deze dozen even uitpakken, anders staan ze er morgen nog.", 0));
                            nextSceneSetup.Add(new SceneSetup(true, availableSprites[0], null, null, null, null, backgroundSprites[5]));
                            messageList.Add(new Message("Mikey", "~", 0));
                            
                            messageList.Add(new Message("Mikey", " ", 0));
                            nextSceneSetup.Add(new SceneSetup(false, availableSprites[0], null, null, null, availableSprites[2], backgroundSprites[6]));
                            messageList.Add(new Message("Mikey", "~", 0));
                            
                            messageList.Add(new Message("Mikey", "Hey schat, hoe was je dag?", 0));
                            messageList.Add(new Message("Laura", "Goed hoor… Ik had alleen wel verwacht dat je zou stofzuigen?", 4));
                            messageList.Add(new Message("Mikey", "Dat heb ik daarstraks gedaan, hoezo?", 0));
                            messageList.Add(new Message("Laura", "Nou, daar is dan niets van te merken. Kijk nou hoeveel stof er nog ligt! Je moet het niet zo afraffelen.", 4));


                            buttonTextList.Add("Excuses aanbieden en het nog een keer stofzuigen");
                            buttonTextList.Add("Zeggen dat je hebt gedaan wat ze van je vroeg en dat het onredelijk is dat ze het niet goed genoeg vindt");
                            break;

                        //Duidelijke maken dat je het probleem niet echt ziet
                        case 1:
                            chatBranch = 2;
                            messageList.Add(new Message("Mikey", "Ach joh, jij onderbrak me daarvoor ook. Daar maak ik toch ook geen probleem van?", 0));
                            messageList.Add(new Message("Laura", "Nouja, oké dan. Maar ik wil liever niet dat je het nog een keer doet, goed?", 4));
                            messageList.Add(new Message("Mikey", "Oké, is goed schatje. Laten maar beginnen met uitpakken, anders staan deze dozen er morgenochtend nog.", 0));
                            nextSceneSetup.Add(new SceneSetup(true, availableSprites[0], null, null, null, null, backgroundSprites[5]));
                            messageList.Add(new Message("Mikey", "~", 0));
                           
                            messageList.Add(new Message("Mikey", " ", 0));
                            nextSceneSetup.Add(new SceneSetup(false, availableSprites[0], null, null, null, availableSprites[2], backgroundSprites[6]));
                            messageList.Add(new Message("Mikey", "~", 0));
                            
                            messageList.Add(new Message("Mikey", "Hey schat, hoe was je dag?", 0));
                            messageList.Add(new Message("Laura", "Goed hoor… Ik had alleen wel verwacht dat je zou stofzuigen?", 4));
                            messageList.Add(new Message("Mikey", "Dat heb ik daarstraks gedaan, hoezo?", 0));
                            messageList.Add(new Message("Laura", "Nou, daar is dan niets van te merken. Kijk nou hoeveel stof er nog ligt! Je moet het niet zo afraffelen.", 4));


                            buttonTextList.Add("Excuses aanbieden en het nog een keer stofzuigen");
                            buttonTextList.Add("Zeggen dat je hebt gedaan wat ze van je vroeg en dat het onredelijk is dat ze het niet goed genoeg vindt");
                            break;

                            //default:
                            //    break;
                    }
                    break;

                //branch 2
                case 2:
                    switch (pressedButton)
                    {
                        //Excuses aanbieden en het nog een keer stofzuigen
                        case 0:
                            chatBranch = 3;
                            messageList.Add(new Message("Mikey", "Sorry, het is inderdaad niet helemaal goed schoon… Ik zal de boel nog een keer stofzuigen, dan.", 0));
                            messageList.Add(new Message("Laura", "Mooi, ik heb een drukke dag gehad, aardig dat je daarnaar gevraagd hebt, trouwens. Maar ik ga naar boven", 4));
                            messageList.Add(new Message("Mikey", "Oh oké", 0));
                            nextSceneSetup.Add(new SceneSetup(true, availableSprites[0], null, null, null, null, backgroundSprites[5]));
                            messageList.Add(new Message("Mikey", "~", 0));
                            
                            messageList.Add(new Message("Mikey", " ", 0));

                            buttonTextList.Add("Felix bellen en vertellen over de situatie");
                            buttonTextList.Add("Felix niet lastigvallen met jullie onenigheidje");
                            break;

                        //Zeggen dat je hebt gedaan wat ze van je vroeg en dat het onredelijk is dat ze het niet goed genoeg vindt
                        case 1:
                            chatBranch = 3;
                            messageList.Add(new Message("Mikey", "Ik zei toch dat ik het al had gedaan? Het is misschien niet perfect, maar het is echt wel schoon.", 0));
                            messageList.Add(new Message("Laura", "Het is maar wat je schoon noemt. Zelfs een beetje stofzuigen kun je nog niet goed. Het is heel vervelend dat ik nooit aan je kan vragen om iets te doen, want als je het al doet - en dat is een grote als! - dan moet ik het altijd zelf nog afmaken omdat jij het niet goed genoeg hebt gedaan.", 4));
                            messageList.Add(new Message("Laura", "Ik dacht: ik kom thuis van een drukke dag en kan dan in ons schone huisje eindelijk uitrusten, maar néé, ik kom thuis is een rotzooi die jij eigenlijk op zou ruimen. Terwijl jij in plaats van luieren het ook gewoon echt goed had kunnen doen!", 4));
                            messageList.Add(new Message("Laura", "Ugh, ik ga naar boven. Ik heb hier nu echt geen zin in. Beter is de vloer schoon als ik straks beneden kom, hoor je me?", 4));
                            messageList.Add(new Message("Mikey", "Ja...", 0));
                            nextSceneSetup.Add(new SceneSetup(true, availableSprites[0], null, null, null, null, backgroundSprites[5]));
                            messageList.Add(new Message("Mikey", "~", 0));
                            
                            messageList.Add(new Message("Mikey", " ", 0));

                            buttonTextList.Add("Felix bellen en vertellen over de situatie");
                            buttonTextList.Add("Felix niet lastigvallen met jullie onenigheidje");
                            break;

                            //default:
                            //    break;
                    }
                    break;

                //branch 3
                case 3:
                    switch (pressedButton)
                    {
                        //Felix bellen en vertellen over de situatie
                        case 0:
                            chatBranch = 4;
                            nextSceneSetup.Add(new SceneSetup(true, availableSprites[6], null, null, null, availableSprites[5], backgroundSprites[8]));
                            messageList.Add(new Message("Mikey", "~", 0));
                            
                            messageList.Add(new Message("Felix", "Hey Mikey, waddup", 4));
                            messageList.Add(new Message("Mikey", "Hey Felix, ik wou even je mening horen over een iets dat net is gebeurd tussen mij en Laura.", 0));
                            messageList.Add(new Message("Felix", "Oké, vertel maar.", 4));
                            messageList.Add(new Message("Mikey", "Ik lag dus op de bank en toen kwam Laura binnen en vertelde me dat ik had moeten stofzuigen en toen ik zei dat ik het zo ging doen flipte ze hem helemaal!", 0));
                            messageList.Add(new Message("Felix", "Ah joh, zulke dingen gebeuren wel eens in relaties, ik denk dat je het gewoon moet vergeten. Zij had vast ook een zware dag achter de rug.", 4));
                            messageList.Add(new Message("Mikey", "Hmm... Misschien heb je gelijk en moet ik het laten zitten. Ze is vast gewoon moe van vandaag. Thanks, Felix. ", 0));
                            messageList.Add(new Message("Felix", "Geen probleem, Mikey!", 4));
                            nextSceneSetup.Add(new SceneSetup(true, null, availableSprites[0], availableSprites[1], availableSprites[4], availableSprites[3], backgroundSprites[2]));
                            messageList.Add(new Message("Felix", "~", 1));
                            
                            messageList.Add(new Message("Felix", "Ik zag laatst trouwens een garage sale en daar zaten een paar prachtige race autootjes bij", 1));
                            messageList.Add(new Message("Samantha", "Oh, cool dus die zitten nu bij je collectie?", 3));
                            messageList.Add(new Message("Felix", "Yep", 1));
                            nextSceneSetup.Add(new SceneSetup(false, availableSprites[6], null, null, null, availableSprites[7], backgroundSprites[9]));
                            messageList.Add(new Message("Mikey", "~", 0));
                            
                            messageList.Add(new Message("Mikey", "Hey Laura", 0));
                            messageList.Add(new Message("Laura", "Hey waar ben je op dit moment?", 4));
                            messageList.Add(new Message("Mikey", "In het café samen met Rob, Felix en Samantha", 0));
                            messageList.Add(new Message("Laura", "Waarom zit je met Samantha zonder mij?, ze is mijn vriendin", 4));
                            messageList.Add(new Message("Mikey", "Uhm ze hoort toch gewoon bij de groep", 0));
                            messageList.Add(new Message("Laura", "Vind je haar leuk ofzo?", 4));

                            buttonTextList.Add("Verbaasd zijn over hoe ze dat kan zeggen");
                            buttonTextList.Add("Sorry zeggen dat je daar niet over na had gedacht");

                            break;

                        //Felix niet lastigvallen met jullie onenigheidje
                        case 1:
                            chatBranch = 4;
                            nextSceneSetup.Add(new SceneSetup(true, null, availableSprites[0], availableSprites[1], availableSprites[4], availableSprites[3], backgroundSprites[2]));
                            messageList.Add(new Message("Felix", "~", 1));
                            
                            messageList.Add(new Message("Felix", "Ik zag laatst trouwens een garage sale en daar zaten een paar prachtige race autootjes bij", 1));
                            messageList.Add(new Message("Samantha", "Oh, cool dus die zitten nu bij je collectie?", 3));
                            messageList.Add(new Message("Felix", "Yep", 1));
                            nextSceneSetup.Add(new SceneSetup(false, availableSprites[6], null, null, null, availableSprites[7], backgroundSprites[9]));
                            messageList.Add(new Message("Mikey", "~", 0));
                           
                            messageList.Add(new Message("Mikey", "Hey Laura", 0));
                            messageList.Add(new Message("Laura", "Hey waar ben je op dit moment?", 4));
                            messageList.Add(new Message("Mikey", "In het café samen met Rob, Felix en Samantha", 0));
                            messageList.Add(new Message("Laura", "Waarom zit je met Samantha zonder mij?, ze is mijn vriendin", 4));
                            messageList.Add(new Message("Mikey", "Uhm ze hoort toch gewoon bij de groep", 0));
                            messageList.Add(new Message("Laura", "Vind je haar leuk ofzo?", 4));

                            buttonTextList.Add("Verbaasd zijn over hoe ze dat kan zeggen");
                            buttonTextList.Add("Sorry zeggen dat je daar niet over na had gedacht");
                            break;

                            //default:
                            //    break;
                    }
                    break;

                //branch 4
                case 4:
                    switch (pressedButton)
                    {
                        //Verbaasd zijn over hoe ze dat kan zeggen
                        case 0:
                            chatBranch = 5;
                            messageList.Add(new Message("Mikey", "Wut, hoe komt dat sowieso in je hoofd op?", 0));
                            messageList.Add(new Message("Laura", "Nou ik zie je wel naar haar kijken, houd je nog wel van me?", 4));
                            messageList.Add(new Message("Mikey", "Natuurlijk houd ik nog van je schat.", 0));
                            messageList.Add(new Message("Laura", "Mooi, dan wil ik niet meer dat je met Samantha omgaat zonder dat ik erbij ben", 4));
                            messageList.Add(new Message("Mikey", "Is dat niet een beetje drastisch?", 0));
                            messageList.Add(new Message("Laura", "EEN BEETJE DRASTISCH?, ik mag hopen dat je dat niet meende, ohja en nog iets, ik wil dat je nu gelijk naar huis komt", 4));
                            messageList.Add(new Message("Mikey", "Ja is goed, ik kom naar je toe", 0));
                            nextSceneSetup.Add(new SceneSetup(true, null, availableSprites[0], availableSprites[1], availableSprites[4], availableSprites[3], backgroundSprites[2]));
                            messageList.Add(new Message("Rob", "~", 4));
                            
                            messageList.Add(new Message("Rob", "Yo is alles oké?", 4));

                            buttonTextList.Add("Ja Laura vroeg zich gewoon af waar ik was(Hou het gesprek geheim)");
                            buttonTextList.Add("Nee niet echt (vertel over het telefoongesprek)");

                            break;

                        //Sorry zeggen dat je daar niet over na had gedacht
                        case 1:
                            chatBranch = 5;
                            messageList.Add(new Message("Mikey", "Tuurlijk niet, ik heb jou toch", 0));
                            messageList.Add(new Message("Laura", "Heb je sowieso wel nagedacht over hoe ik me nu voel?", 4));
                            messageList.Add(new Message("Laura", "Ik hou zoveel van je en dan spreek je gewoon met haar af achter me rug om", 4));
                            messageList.Add(new Message("Mikey", "Dat is niet precies hoe het is gegaan babe", 0));
                            messageList.Add(new Message("Laura", "MAAKT ME NIET UIT, ik wil niet meer dat je nog met haar omgaat zonder dat ik erbij ben", 4));
                            messageList.Add(new Message("Laura", "Oh en ik mag hopen dat je al onderweg naar huis bent", 4));
                            messageList.Add(new Message("Mikey", "Uhh ja ik kom er nu gelijk aan, totzo", 0));
                            nextSceneSetup.Add(new SceneSetup(true, null, availableSprites[0], availableSprites[1], availableSprites[4], availableSprites[3], backgroundSprites[2]));
                            messageList.Add(new Message("Rob", "~", 4));
                           
                            messageList.Add(new Message("Rob", "Yo is alles oké?", 4));

                            buttonTextList.Add("Ja Laura vroeg zich gewoon af waar ik was(Hou het gesprek geheim)");
                            buttonTextList.Add("Nee, niet echt (vertel over het telefoongesprek)");
                            break;

                            //default:
                            //    break;
                    }
                    break;

                //branch 5
                case 5:
                    switch (pressedButton)
                    {
                        //Ja Laura vroeg zich gewoon af waar ik was(Hou het gesprek geheim)
                        case 0:
                            chatBranch = 6;
                            nextSceneSetup.Add(new SceneSetup(true, availableSprites[0], null, null, null, availableSprites[3], backgroundSprites[1]));
                            messageList.Add(new Message("Rob", "~", 4));
                            
                            messageList.Add(new Message("Rob", "Ey hoe gaat het nu eigenlijk tussen jullie twee?", 4));

                            buttonTextList.Add("Gaat wel goed");
                            buttonTextList.Add("Een paar hobbeltjes in de weg");

                            break;

                        //Nee niet echt (vertel over het telefoongesprek)
                        case 1:
                            chatBranch = 6;
                            messageList.Add(new Message("Mikey", "Nee, niet echt, Laura wil eigenlijk niet meer dat ik met jullie afspreek zonder haar", 0));
                            messageList.Add(new Message("Rob", "Dat klinkt nogal manipulatief Mikey, misschien moe..", 4));
                            messageList.Add(new Message("Samantha", "Klinkt best logisch, ze is gewoon bang dat je haar verlaat", 3));
                            messageList.Add(new Message("Mikey", "Ja klopt, zouden jullie het erg vinden als ik voortaan alleen kom wanneer Laura ook kan komen", 0));
                            messageList.Add(new Message("Samantha", "Nee hoor, we snappen het helemaal", 3));
                            messageList.Add(new Message("Mikey", "Oké top jongens, ik moet er nu ook vandoor dus ik zie jullie de volgende keer wel", 0));
                            messageList.Add(new Message("Rob", "Oké, laters", 4));
                            messageList.Add(new Message("Samantha", "Doei Mikey", 3));
                            nextSceneSetup.Add(new SceneSetup(true, availableSprites[0], null, null, null, availableSprites[3], backgroundSprites[5]));
                            messageList.Add(new Message("Rob", "~", 4));
                            
                            messageList.Add(new Message("Rob", "Ey hoe gaat het nu eigenlijk tussen jullie twee?", 4));

                            buttonTextList.Add("Gaat wel goed");
                            buttonTextList.Add("Een paar hobbeltjes in de weg");
                            break;

                            //default:
                            //    break;
                    }
                    break;

                //branch 6
                case 6:
                    switch (pressedButton)
                    {
                        //Gaat wel goed
                        case 0:
                            chatBranch = 7;
                            messageList.Add(new Message("Mikey", "Het gaat wel goed", 0));
                            messageList.Add(new Message("Rob", "Dat is goed om te horen, ik krijg soms wel is me twijfels over jullie samen", 4));
                            messageList.Add(new Message("Mikey", "Ah dat is niet nodig joh, het gaat prima met Laura en mij", 0));
                            nextSceneSetup.Add(new SceneSetup(true, availableSprites[0], null, null, null, availableSprites[2], backgroundSprites[6]));
                            messageList.Add(new Message("Mikey", "~", 0));
                            
                            messageList.Add(new Message("Mikey", "Hey schat, ik vind dat je de laatste tijd wel een beetje negatief doet tegen me", 0));
                            messageList.Add(new Message("Laura", "Wanneer dan?", 4));
                            messageList.Add(new Message("Mikey", "Nou het feit dat je niet wilt dat ik met me vrienden omga zonder jou, vind ik best wel negatief", 0));
                            messageList.Add(new Message("Laura", "Ja, maar dat komt omdat jij niet denkt aan hoe ik me voel op dat soort momenten", 4));
                            messageList.Add(new Message("Mikey", "Alsnog denk ik dat je je daar wat minder over moeten stressen", 0));
                            messageList.Add(new Message("Laura", "Ik kan me niet zomaar minder stressen, wanneer jij met mijn beste vriendin zit te drinken. Je doet soms gewoon zo negatief tegen me en ik kan dat emotioneel gewoon niet aan", 4));

                            buttonTextList.Add("Troost haar");
                            buttonTextList.Add("Zeg dat jij helemaal niet zo negatief doet");

                            break;

                        //Een paar hobbeltjes in de weg
                        case 1:
                            chatBranch = 7;
                            messageList.Add(new Message("Mikey", "Er zitten soms wat hobbeltjes in de weg, maar verder gaat het wel goed", 0));
                            messageList.Add(new Message("Rob", "Wat houden die hobbeltjes dan in?", 4));
                            messageList.Add(new Message("Mikey", "Nou soms dan doet ze net alsof ik met niemand anders om mag gaan of dat ik niets nuttigs kan doen", 0));
                            messageList.Add(new Message("Rob", "Damn, dat klinkt niet zo gezond dude. Je zou het me wel laten weten als je ergens mee zit hé?, je kunt altijd op mij rekenen", 4));
                            messageList.Add(new Message("Mikey", "Ahh ik overleef het wel, toch bedankt dat je me verteld dat ik op je kan rekenen.", 0));
                            nextSceneSetup.Add(new SceneSetup(true, availableSprites[0], null, null, null, availableSprites[2], backgroundSprites[6]));
                            messageList.Add(new Message("Mikey", "~", 0));
                            
                            messageList.Add(new Message("Mikey", "Hey schat, ik vind dat je de laatste tijd wel een beetje negatief doet tegen me", 0));
                            messageList.Add(new Message("Laura", "Wanneer dan?", 4));
                            messageList.Add(new Message("Mikey", "Nou het feit dat je niet wilt dat ik met me vrienden omga zonder jou, vind ik best wel negatief", 0));
                            messageList.Add(new Message("Laura", "Ja, maar dat komt omdat jij niet denkt aan hoe ik me voel op dat soort momenten", 4));
                            messageList.Add(new Message("Mikey", "Alsnog denk ik dat je je daar wat minder over moeten stressen", 0));
                            messageList.Add(new Message("Laura", "Ik kan me niet zomaar minder stressen, wanneer jij met mijn beste vriendin zit te drinken. Je doet soms gewoon zo negatief tegen me en ik kan dat emotioneel gewoon niet aan", 4));

                            buttonTextList.Add("Troost haar");
                            buttonTextList.Add("Zeg dat jij helemaal niet zo negatief doet");
                            break;

                            //default:
                            //    break;
                    }
                    break;

                //branch 7
                case 7:
                    switch (pressedButton)
                    {
                        //Troost haar
                        case 0:
                            chatBranch = 7;
                            messageList.Add(new Message("Mikey", "Ohh sorry babe, ik heb dat dan gewoon niet doorgehad", 0));
                            messageList.Add(new Message("Laura", "Het is oké liefje, ik hou gewoon zoveel van je en wil je niet kwijt door je negatieve gedrag.", 4));
                            messageList.Add(new Message("Mikey", "Ik snap het babe, ik zal beter nadenken over wat ik zeg", 0));
                            messageList.Add(new Message("Mikey", "@", 0));


                            break;

                        //Zeg dat jij helemaal niet zo negatief doet
                        case 1:
                            chatBranch = 8;
                            messageList.Add(new Message("Mikey", "Zo negatief doe ik niet, volgens mij stel je je een beetje aan", 0));
                            messageList.Add(new Message("Laura", "En volgens mij moet jij niet zo gemeen doen, je weet dat ik dit apartment betaal en dat je wel bij me moet blijven", 4));
                           
                            buttonTextList.Add("Sorry zeggen(blijven)");
                            buttonTextList.Add("Bij je vrienden hulp zoeken(weggaan)");
                            break;

                            //default:
                            //    break;
                    }
                    break;

                //branch 8
                case 8:
                    switch (pressedButton)
                    {
                        //Sorry zeggen(blijven)
                        case 0:
                            chatBranch = 9;
                            messageList.Add(new Message("Mikey", "Het spijt me babe, ik draaide zeker weer door. Ik zal voortaan beter nadenken over wat ik zeg", 0));
                            messageList.Add(new Message("Mikey", "@", 0));


                            break;

                        //Bij je vrienden hulp zoeken(weggaan)
                        case 1:
                            chatBranch = 9;
                            messageList.Add(new Message("Mikey", "Ik trek dit niet langer, ik ga bij een van onze vrienden overnachten", 0));
                            messageList.Add(new Message("Mikey", "@", 0));
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
    IEnumerator NextScene(SceneSetup nextSetup)
    {

        if (nextSetup.withTransition)
        {
            sceneTransition.SetActive(true);
            sceneTransition.GetComponent<Animator>().SetBool("PlayAnim", true);
            yield return new WaitForSeconds(1.5f);
        }


        Debug.Log("I made a new scene");
        foreach (GameObject person in personList)
        {
            if (!person.activeSelf)
            {
                person.SetActive(true);
            }
        }
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

        background.sprite = nextSetup.background;
        nextSceneSetup.Remove(nextSceneSetup[0]);
        //if (nextSceneSetup[0] != null)
        //{
        //    Debug.Log("het werkt");
        //} else
        //{
        //    Debug.Log("het werkt helaas niet");
        //}

    }

    IEnumerator EndGame()
    {
        sceneTransition.SetActive(true);
        sceneTransition.GetComponent<Animator>().SetBool("PlayAnim", true);
        yield return new WaitForSeconds(1.5f);
        playerManager.amountOfPlayers = 0;
        playerManager.previousTurn = 0;
        SceneManager.LoadScene(1);
    }
}
