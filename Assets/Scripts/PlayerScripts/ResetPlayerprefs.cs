using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResetPlayerprefs : MonoBehaviour
{
    [SerializeField] private string[] keys;

    //When called Delates a key of a player pref given by an array of strings
    void DelateKey()
    {
        foreach(string s in keys)
        {
            PlayerPrefs.DeleteKey(s);
        }
    }

    //Calls a function when the scee changes
    private void OnDisable()
    {
        DelateKey();
    }
}
