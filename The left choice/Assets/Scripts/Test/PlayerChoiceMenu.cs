using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerChoiceMenu : MonoBehaviour
{
    [SerializeField]private Animator startButtonAnim;
    private Animator myAnim;
    [SerializeField] private PlayerManagerScript pms;

    private void Start()
    {
        myAnim = GetComponent<Animator>();
    }

    public void PlayAnimations()
    {
        if (!myAnim.GetBool("PlayAnim"))
        {
            startButtonAnim.SetBool("PlayAnim", true);
            myAnim.SetBool("PlayAnim", true);
        } else if(pms.amountOfPlayers > 0)
        {
            SceneManager.LoadScene(1);
        }
    }
}
