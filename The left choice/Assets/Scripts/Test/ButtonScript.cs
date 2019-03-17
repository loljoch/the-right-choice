using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonScript : MonoBehaviour
{
    public int buttonValue;
    public AbusedManStory story;

    public void OnButtonPress()
    {
        story.NextPartOfStory(buttonValue);
    }


}
