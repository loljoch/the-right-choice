using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManagerScript : MonoBehaviour
{
    public int amountOfPlayers;
    private int previousTurn;

    private void Start()
    {
        DontDestroyOnLoad(this);
        previousTurn = 0;
    }

    public void ArrayPlayers(int number)
    {
        amountOfPlayers = number;
    }

    public int CalculatePlayerTurn()
    {
        previousTurn += 1;
        if(previousTurn > amountOfPlayers)
        {
            previousTurn = 1;
        }
        return previousTurn;
    }

    public int PreviousPlayerTurn()
    {
        return previousTurn;
    }
}
