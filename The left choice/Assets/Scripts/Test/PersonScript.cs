using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PersonScript : MonoBehaviour
{
    public int personNumber;
    public bool isPersonRight;
    public Color textColor;
    public GameObject speechBubble;

    private void Start()
    {
        if(GetComponent<RectTransform>().anchoredPosition.x < 0){
            isPersonRight = true;
        } else
        {
            isPersonRight = false;
        }
    }
}
