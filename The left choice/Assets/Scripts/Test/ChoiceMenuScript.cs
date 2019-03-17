using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChoiceMenuScript : MonoBehaviour
{
    [SerializeField] private AbusedManStory story;

    public void SpawnButtons()
    {
        story.SpawnButtons(true);
    }
}
