using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeactivateObjectScript : MonoBehaviour
{
    public void DeactivateObject()
    {
        this.gameObject.SetActive(false);
    }
}

