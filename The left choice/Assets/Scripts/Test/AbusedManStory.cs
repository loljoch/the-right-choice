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
    [SerializeField] private List<PersonScript> personScriptList;
    [SerializeField] private GameObject[] personList;

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
        personList = GameObject.FindGameObjectsWithTag("Person");
        playerManager = GameObject.FindGameObjectWithTag("GameController").GetComponent<PlayerManagerScript>();

        AssignButtons();
        nextSceneSetup = new SceneSetup(false, null, null, availableSprites[2], availableSprites[3], availableSprites[4]);
        NextScene(nextSceneSetup);

        waitTime = new WaitForSeconds(timeBetweenMessages);
        buttonTextList = new List<string>();
        messageList = new List<Message>();

        //Sets the starting messages
        messageList.Add(new Message("Felix", "Hey, daar zijn ze eindelijk hoor.", 4));

        //This is how to skip after a message
        //messageList.Add(new Message("Persoon 1", "~", 1));
        //nextSceneSetup = new SceneSetup(false, null, availableSprites[1], availableSprites[2], availableSprites[3], availableSprites[4]);
        nextSceneSetup = new SceneSetup(false, availableSprites[0], availableSprites[1], availableSprites[2], availableSprites[3], availableSprites[4]);
        messageList.Add(new Message("Mikey", "~", 1));
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
                //branch 0
                case 0:
                    //Decides which button is pressed to get the next story
                    switch (pressedButton)
                    {
                        case 0:
                            //Laura terug onderbreken
                            chatBranch = 1;
                            messageList.Add(new Message("Laura", "Mikey en ik gaan name-", 0));
                            messageList.Add(new Message("Mikey", "We gaan samenwonen! Ik trek bij haar in.", 1));
                            messageList.Add(new Message("Samantha", "O, wat leuk! Wanneer ga je verhuizen? Dan kunnen wij wel helpen, toch jongens?", 3));
                            messageList.Add(new Message("Rob", "Ja, tuurlijk helpen we Mikey wel met verhuizen, maar is het niet een beetje snel om nu al bij elkaar in te trekken?", 5));
                            messageList.Add(new Message("Felix", "Ah joh Rob, ze zijn toch al 4 maanden samen. Het leven is kort, vriend. En natuurlijk helpen we je met verhuizen, Mikey!", 4));
                            messageList.Add(new Message("Mikey", "Dat is wel heel aardig van jullie jongens, maar we hebben vandaag al alles verhuisd. Het is alleen nog maar uitpakken nu.", 1));
                            messageList.Add(new Message("Laura", "Yep, mijn stoere bink heeft het helemaal zelf gesjouwd, ook de zwaarste dozen.", 5));
                            messageList.Add(new Message("Samantha", "Wat leuk, ik wed dat jullie het naar je zin gaan hebben samen. Jullie twee zijn zo’n lief koppeltje.", 5));
                            messageList.Add(new Message("Felix", "Ja, jullie zijn inderdaad altijd een stel tortelduiven.", 4));
                            messageList.Add(new Message("Rob", "Daar ben ik het mee eens, maar heb je je appartement al helemaal verlaten?", 5));
                            messageList.Add(new Message("Laura", "Yep, ene Gerry zit nu in zijn appartement, hij wilde gelijk intrekken.", 0));
                            messageList.Add(new Message("Felix", "Awh man, dat was altijd zo’n cool appartement, met die loungehoek die je had gemaakt. Jammer, daar zullen jullie daar nu geen ruimte meer voor hebben. Maar even iets heel anders: hoe gaat het met eigenlijk met jou, Rob?", 4));
                            messageList.Add(new Message("Rob", "Gaat wel goed! Ik heb laatst een loonsverhoging gekregen waardoor ik nu tenminste als een normaal persoon kan avondeten.", 5));
                            messageList.Add(new Message("Mikey", "~", 1));
                            nextSceneSetup = new SceneSetup(true, availableSprites[0], null, null, availableSprites[3], null);
                            messageList.Add(new Message("Mikey", "Hè hè, dat was me het avondje wel. Felix kon echt niet ophouden over zijn nieuwe raceautootje. Wat een gozer is-ie toch. Was sowieso echt weer gezellig.", 0));
                            messageList.Add(new Message("Laura", "Haha, ja inderdaad… Maar hé, ik vond het alleen niet zo fijn dat je me onderbrak vanavond.", 5));

                            buttonTextList.Add("Excuses aanbieden");
                            buttonTextList.Add("Duidelijke maken dat je het probleem niet echt ziet");
                            break;

                        case 1:
                            //Laura niet terug onderbreken
                            chatBranch = 2;
                            messageList.Add(new Message("Laura", "Mikey en ik gaan namelijk samenwonen!", 0));
                            messageList.Add(new Message("Samantha", "O, wat leuk! Wanneer ga je verhuizen? Dan kunnen wij wel helpen, toch jongens?", 3));
                            messageList.Add(new Message("Rob", "Ja, tuurlijk helpen we Mikey wel met verhuizen, maar is het niet een beetje snel om nu al bij elkaar in te trekken?", 5));
                            messageList.Add(new Message("Felix", "Ah joh Rob, ze zijn toch al 4 maanden samen. Het leven is kort, vriend. En natuurlijk helpen we je met verhuizen, Mikey!", 4));
                            messageList.Add(new Message("Mikey", "Dat is wel heel aardig van jullie jongens, maar we hebben vandaag al alles verhuisd. Het is alleen nog maar uitpakken nu.", 1));
                            messageList.Add(new Message("Laura", "Yep, mijn stoere bink heeft het helemaal zelf gesjouwd, ook de zwaarste dozen.", 5));
                            messageList.Add(new Message("Samantha", "Wat leuk, ik wed dat jullie het naar je zin gaan hebben samen. Jullie twee zijn zo’n lief koppeltje.", 5));
                            messageList.Add(new Message("Felix", "Ja, jullie zijn inderdaad altijd een stel tortelduiven.", 4));
                            messageList.Add(new Message("Rob", "Daar ben ik het mee eens, maar heb je je appartement al helemaal verlaten?", 5));
                            messageList.Add(new Message("Laura", "Yep, ene Gerry zit nu in zijn appartement, hij wilde gelijk intrekken.", 0));
                            messageList.Add(new Message("Felix", "Awh man, dat was altijd zo’n cool appartement, met die loungehoek die je had gemaakt. Jammer, daar zullen jullie daar nu geen ruimte meer voor hebben. Maar even iets heel anders: hoe gaat het met eigenlijk met jou, Rob?", 4));
                            messageList.Add(new Message("Rob", "Gaat wel goed! Ik heb laatst een loonsverhoging gekregen waardoor ik nu tenminste als een normaal persoon kan avondeten.", 5));
                            messageList.Add(new Message("Mikey", "~", 0));
                            nextSceneSetup = new SceneSetup(true, availableSprites[0], null, null, availableSprites[3], null);
                            messageList.Add(new Message("Mikey", "Hè hè, dat was me het avondje wel. Felix kon echt niet ophouden over zijn nieuwe raceautootje. Wat een gozer is-ie toch. Was sowieso echt weer gezellig.", 0));
                            messageList.Add(new Message("Laura", "Haha, ja inderdaad. Je zei alleen niet zoveel over het samenwonen, ben je er wel enthousiast over?", 5));
                            messageList.Add(new Message("Mikey", "Ja, tuurlijk ben ik er enthousiast over babe, ik liet jou het gewoon vertellen.", 0));
                            messageList.Add(new Message("Laura", "Hmm, oké dan. Maar je mag volgende keer best wat inbreng geven hoor. Ik was bang dat je het helemaal niet zo zag zitten om bij me te gaan wonen…", 5));
                            messageList.Add(new Message("Mikey", "Hé sorry, zo bedoelde ik het natuurlijk niet. Ik kijk hier hartstikke naar uit, Lau. Kom, laten we nu maar gauw beginnen met uitpakken, dan kunnen we er echt ons paleisje van maken, samen.", 0));
                            messageList.Add(new Message("Mikey", "~", 0));
                            nextSceneSetup = new SceneSetup(true, availableSprites[0], null, null, null, null);
                            messageList.Add(new Message("Mikey", " ", 0));
                            messageList.Add(new Message("Mikey", "~", 0));
                            nextSceneSetup = new SceneSetup(false, availableSprites[0], null, null, availableSprites[3], null);
                            messageList.Add(new Message("Mikey", "Hey schat, hoe was je dag?", 0));
                            messageList.Add(new Message("Laura", "Goed hoor… Ik had alleen wel verwacht dat je zou stofzuigen?", 0));
                            messageList.Add(new Message("Mikey", "Dat heb ik daarstraks gedaan, hoezo?", 0));
                            messageList.Add(new Message("Laura", "Nou, daar is dan niets van te merken. Kijk nou hoeveel stof er nog ligt! Je moet het niet zo afraffelen.", 0));


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
                            messageList.Add(new Message("Laura", "Oké oké, zand erover. Maar als ik het nog een keer zie gebeuren dan mag je op de bank slapen, ja?", 5));
                            messageList.Add(new Message("Mikey", "Haha prima, ik begrijp het.", 0));
                            messageList.Add(new Message("Mikey", "Laten we nou maar deze dozen even uitpakken, anders staan ze er morgen nog.", 0));
                            messageList.Add(new Message("Mikey", "~", 0));
                            nextSceneSetup = new SceneSetup(true, availableSprites[0], null, null,null, null);
                            messageList.Add(new Message("Mikey", " ", 0));
                            messageList.Add(new Message("Mikey", "~", 0));
                            nextSceneSetup = new SceneSetup(false, availableSprites[0], null, null, availableSprites[3], null);
                            messageList.Add(new Message("Mikey", "Hey schat, hoe was je dag?", 0));
                            messageList.Add(new Message("Laura", "Goed hoor… Ik had alleen wel verwacht dat je zou stofzuigen?", 0));
                            messageList.Add(new Message("Mikey", "Dat heb ik daarstraks gedaan, hoezo?", 0));
                            messageList.Add(new Message("Laura", "Nou, daar is dan niets van te merken. Kijk nou hoeveel stof er nog ligt! Je moet het niet zo afraffelen.", 0));


                            buttonTextList.Add("Excuses aanbieden en het nog een keer stofzuigen");
                            buttonTextList.Add("Zeggen dat je hebt gedaan wat ze van je vroeg en dat het onredelijk is dat ze het niet goed genoeg vindt");
                            break;

                        //Duidelijke maken dat je het probleem niet echt ziet
                        case 1:
                            chatBranch = 2;
                            messageList.Add(new Message("Mikey", "Ach joh, jij onderbrak me daarvoor ook. Daar maak ik toch ook geen probleem van?", 0));
                            messageList.Add(new Message("Laura", "Nouja, oké dan. Maar ik wil liever niet dat je het nog een keer doet, goed?", 5));
                            messageList.Add(new Message("Mikey", "Oké, is goed schatje. Laten maar beginnen met uitpakken, anders staan deze dozen er morgenochtend nog.", 0));
                            messageList.Add(new Message("Mikey", "~", 0));
                            nextSceneSetup = new SceneSetup(true, availableSprites[0], null, null, null, null);
                            messageList.Add(new Message("Mikey", " ", 0));
                            messageList.Add(new Message("Mikey", "~", 0));
                            nextSceneSetup = new SceneSetup(false, availableSprites[0], null, null, availableSprites[3], null);
                            messageList.Add(new Message("Mikey", "Hey schat, hoe was je dag?", 0));
                            messageList.Add(new Message("Laura", "Goed hoor… Ik had alleen wel verwacht dat je zou stofzuigen?", 0));
                            messageList.Add(new Message("Mikey", "Dat heb ik daarstraks gedaan, hoezo?", 0));
                            messageList.Add(new Message("Laura", "Nou, daar is dan niets van te merken. Kijk nou hoeveel stof er nog ligt! Je moet het niet zo afraffelen.", 0));


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
                            messageList.Add(new Message("Laura", "Mooi, ik heb een drukke dag gehad, aardig dat je daarnaar gevraagd hebt, trouwens. Maar ik ga naar boven", 5));
                            messageList.Add(new Message("Mikey", "Oh oké", 0));
                            messageList.Add(new Message("Mikey", "~", 0));
                            nextSceneSetup = new SceneSetup(true, availableSprites[0], null, null, null, null);

                            buttonTextList.Add("Felix bellen en vertellen over de situatie");
                            buttonTextList.Add("Felix niet lastigvallen met jullie onenigheidje");
                            break;

                        //Zeggen dat je hebt gedaan wat ze van je vroeg en dat het onredelijk is dat ze het niet goed genoeg vindt
                        case 1:
                            chatBranch = 3;
                            messageList.Add(new Message("Mikey", "Ik zei toch dat ik het al had gedaan? Het is misschien niet perfect, maar het is echt wel schoon.", 0));
                            messageList.Add(new Message("Laura", "Het is maar wat je schoon noemt. Zelfs een beetje stofzuigen kun je nog niet goed. Het is heel vervelend dat ik nooit aan je kan vragen om iets te doen, want als je het al doet - en dat is een grote als! - dan moet ik het altijd zelf nog afmaken omdat jij het niet goed genoeg hebt gedaan.", 5));
                            messageList.Add(new Message("Laura", "Ik dacht: ik kom thuis van een drukke dag en kan dan in ons schone huisje eindelijk uitrusten, maar néé, ik kom thuis is een rotzooi die jij eigenlijk op zou ruimen. Terwijl jij in plaats van luieren het ook gewoon echt goed had kunnen doen!", 5));
                            messageList.Add(new Message("Laura", "Ugh, ik ga naar boven. Ik heb hier nu echt geen zin in. Beter is de vloer schoon als ik straks beneden kom, hoor je me?", 5));
                            messageList.Add(new Message("Mikey", "Ja...", 0));
                            messageList.Add(new Message("Mikey", "~", 0));
                            nextSceneSetup = new SceneSetup(true, availableSprites[0], null, null, null, null);

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
                            messageList.Add(new Message("Felix", "Hey Mikey, waddup", 5));
                            messageList.Add(new Message("Mikey", "Hey Felix, ik wou even je mening horen over een iets dat net is gebeurd tussen mij en Laura.", 0));
                            messageList.Add(new Message("Felix", "Oké, vertel maar.", 5));
                            messageList.Add(new Message("Mikey", "Ik lag dus op de bank en toen kwam Laura binnen en vertelde me dat ik had moeten stofzuigen en toen ik zei dat ik het zo ging doen flipte ze hem helemaal!", 0));
                            messageList.Add(new Message("Felix", "Ah joh, zulke dingen gebeuren wel eens in relaties, ik denk dat je het gewoon moet vergeten. Zij had vast ook een zware dag achter de rug.", 5));
                            messageList.Add(new Message("Mikey", "Hmm... Misschien heb je gelijk en moet ik het laten zitten. Ze is vast gewoon moe van vandaag. Thanks, Felix. ", 0));
                            messageList.Add(new Message("Felix", "Geen probleem, Mikey!", 5));
                            messageList.Add(new Message("Mikey", "~", 0));
                            nextSceneSetup = new SceneSetup(true, null, availableSprites[0], availableSprites[2], availableSprites[4], availableSprites[3]);
                            messageList.Add(new Message("Felix", "Ik zag laatst trouwens een garage sale en daar zaten een paar prachtige race autootjes bij", 5));
                            messageList.Add(new Message("Samantha", "Oh, cool dus die zitten nu bij je collectie?", 4));
                            messageList.Add(new Message("Felix", "Yep", 5));
                            messageList.Add(new Message("Mikey", "~", 0));
                            nextSceneSetup = new SceneSetup(false, availableSprites[0], null, null, null, availableSprites[2]);
                            messageList.Add(new Message("Mikey", "Hey Laura", 0));
                            messageList.Add(new Message("Laura", "Hey waar ben je op dit moment?", 5));
                            messageList.Add(new Message("Mikey", "In het café samen met Rob en Samantha", 0));
                            messageList.Add(new Message("Laura", "Waarom zit je met Samantha zonder mij?, ze is mijn vriendin", 5));
                            messageList.Add(new Message("Mikey", "Uhm ze hoort toch gewoon bij de groep", 0));
                            messageList.Add(new Message("Laura", "Vind je haar leuk ofzo?", 5));

                            buttonTextList.Add("Verbaasd zijn over hoe ze dat kan zeggen");
                            buttonTextList.Add("Sorry zeggen dat je daar niet over na had gedacht");

                            break;

                        //Felix niet lastigvallen met jullie onenigheidje
                        case 1:
                            chatBranch = 4;
                            messageList.Add(new Message("Felix", "~", 5));
                            nextSceneSetup = new SceneSetup(true, null, availableSprites[0], availableSprites[2], availableSprites[4], availableSprites[3]);
                            messageList.Add(new Message("Felix", "Ik zag laatst trouwens een garage sale en daar zaten een paar prachtige race autootjes bij", 5));
                            messageList.Add(new Message("Samantha", "Oh, cool dus die zitten nu bij je collectie?", 4));
                            messageList.Add(new Message("Felix", "Yep", 5));
                            messageList.Add(new Message("Mikey", "~", 0));
                            nextSceneSetup = new SceneSetup(false, availableSprites[0], null, null, null, availableSprites[2]);
                            messageList.Add(new Message("Mikey", "Hey Laura", 0));
                            messageList.Add(new Message("Laura", "Hey waar ben je op dit moment?", 5));
                            messageList.Add(new Message("Mikey", "In het café samen met Rob en Samantha", 0));
                            messageList.Add(new Message("Laura", "Waarom zit je met Samantha zonder mij?, ze is mijn vriendin", 5));
                            messageList.Add(new Message("Mikey", "Uhm ze hoort toch gewoon bij de groep", 0));
                            messageList.Add(new Message("Laura", "Vind je haar leuk ofzo?", 5));

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
                            chatBranch = 4;
                            messageList.Add(new Message("Mikey", "Hey Laura", 0));
                            messageList.Add(new Message("Laura", "Nou ik zie je wel naar haar kijken, houd je nog wel van me?", 5));
                            messageList.Add(new Message("Mikey", "Natuurlijk houd ik nog van je schat.", 0));
                            messageList.Add(new Message("Laura", "Mooi, dan wil ik niet meer dat je met Samantha omgaat zonder dat ik erbij ben", 5));
                            messageList.Add(new Message("Mikey", "Is dat niet een beetje drastisch?", 0));
                            messageList.Add(new Message("Laura", "EEN BEETJE DRASTISCH?, ik mag hopen dat je dat niet meende, ohja en nog iets, ik wil dat je nu gelijk naar huis komt", 5));
                            messageList.Add(new Message("Mikey", "Ja is goed, ik kom naar je toe", 0));

                            buttonTextList.Add("Verbaasd zijn over hoe ze dat kan zeggen");
                            buttonTextList.Add("Sorry zeggen dat je daar niet over na had gedacht");

                            break;

                        //Sorry zeggen dat je daar niet over na had gedacht
                        case 1:
                            chatBranch = 4;
                            messageList.Add(new Message("Mikey", "Tuurlijk niet, ik heb jou toch", 0));
                            messageList.Add(new Message("Laura", "Heb je sowieso wel nagedacht over hoe ik me nu voel?", 5));
                            messageList.Add(new Message("Laura", "Ik hou zoveel van je en dan spreek je gewoon met haar af achter me rug om", 5));
                            messageList.Add(new Message("Mikey", "Dat is niet precies hoe het is gegaan babe", 0));
                            messageList.Add(new Message("Laura", "MAAKT ME NIET UIT, ik wil niet meer dat je nog met haar omgaat zonder dat ik erbij ben", 5));
                            messageList.Add(new Message("Laura", "Oh en ik mag hopen dat je al onderweg naar huis bent", 5));
                            messageList.Add(new Message("Mikey", "Uhh ja ik kom er nu gelijk aan, totzo", 0));


                            buttonTextList.Add("Verbaasd zijn over hoe ze dat kan zeggen");
                            buttonTextList.Add("Sorry zeggen dat je daar niet over na had gedacht");
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
        foreach (GameObject person in personList)
        {
            Debug.Log("IK WORD GECODE");
            person.SetActive(true);
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
    }
}
