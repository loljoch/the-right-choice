using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MessageList : MonoBehaviour
{

    [SerializeField] private Button button;
    [SerializeField] private List<string> buttonTextList;
    private int chatBranch = 0;


    [SerializeField] private SpwnText spawnText;
    [SerializeField] private Color leftColor, rightColor, timeColor;
    [SerializeField] private float timeBetweenMessages;
    private WaitForSeconds waitTime;
    private List<AdvancedMessage> messageList;
    [SerializeField] private Sprite ashley, rick;




    private void Start()
    {
        waitTime = new WaitForSeconds(timeBetweenMessages);
        buttonTextList = new List<string>();
        messageList = new List<AdvancedMessage>();

        //Sets the starting messages
        messageList.Add(new AdvancedMessage("Ashley", "Ik yeet even deze blikje weg", leftColor, ashley));
        messageList.Add(new AdvancedMessage("Rick", "Aye scool", rightColor, rick));
        messageList.Add(new AdvancedMessage("Ashley", "Ben er weer :D", leftColor, ashley));
        messageList.Add(new AdvancedMessage("Rick", "Top, zou je me straks even willen helpen met mijn huiswerk?", rightColor, rick));

        //Sets the starting buttons
        buttonTextList.Add("Ja sgoed");
        buttonTextList.Add("Nee liever niet ik heb betere dingen te doen");
        buttonTextList.Add("Alleen als jij mij daarna ook helpt");
        buttonTextList.Add("ok");

    }

    //Activates the messages
    public void StartTextChat()
    {
        StartCoroutine(LogText());
    }


    private IEnumerator LogText()
    {
        //Creates the messages
        foreach (AdvancedMessage message in messageList)
        {
            spawnText.WriteMessage(message);
            yield return waitTime;
        }
        CreateButtons(buttonTextList.Count);

    }

    private void CreateButtons(int amountOfButtons)
    {
        //Creates the buttons
        for (int i = 0; i < amountOfButtons; i++)
        {
            Button newButton = Instantiate(button, this.transform);
            newButton.GetComponentInChildren<Text>().text = buttonTextList[i];
            newButton.GetComponent<buttonScript>().buttonValue = i;
            newButton.GetComponent<buttonScript>().inFirstPick = true;
        }




    }

    public void ButtonPress(int buttonPressed)
    {
        messageList.Clear();
        buttonTextList.Clear();

        //Decides in which chatbranch you are (which story you're following)
        switch (chatBranch)
        {
            case 0:
                //Decides which button is pressed to get the next story
                switch (buttonPressed)
                {
                    case 0:
                        chatBranch = 1;
                        messageList.Add(new AdvancedMessage("Ashley", "Ja sgoed", leftColor, ashley));
                        messageList.Add(new AdvancedMessage("Rick", "Top, dankjewel", rightColor, rick));
                        messageList.Add(new AdvancedMessage("Tijd", "Paar dagen later", timeColor, rick));
                        messageList.Add(new AdvancedMessage("Rick", "Hey, ik weet dat je het druk hebt, maar zou je me weer kunnen helpen met mijn huiswerk?", rightColor, rick));

                        buttonTextList.Add("Nee, dat gaat echt niet nu");
                        buttonTextList.Add("Sorry, ik heb het nu echt heel druk");
                        buttonTextList.Add("Alleen als jij mij daarna ook helpt");
                        StartTextChat();
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
