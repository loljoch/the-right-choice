using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SceneSetup
{
    public bool withTransition;
    public List<Sprite> sceneSprites;
    //public Sprite person1;
    //public Sprite person2;
    //public Sprite person3;
    //public Sprite person4;
    //public Sprite person5;


    public SceneSetup(bool newWithTransition, Sprite newPerson1, Sprite newPerson2, Sprite newPerson3, Sprite newPerson4, Sprite newPerson5)
    {
        withTransition = newWithTransition;
        sceneSprites = new List<Sprite>
        {
            newPerson1,
            newPerson2,
            newPerson3,
            newPerson4,
            newPerson5
        };
    }
}
