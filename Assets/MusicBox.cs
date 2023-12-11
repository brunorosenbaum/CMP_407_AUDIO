using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicBox : MonoBehaviour
{
    //public static GameObject musicBoxRef { get; private set; }
    public AK.Wwise.Event musicBox;
    int treesTrigger = 17;

    private bool isEnabled;
    private bool doOnce = true; 
    private void Awake()
    {
        isEnabled = false; 
    }


    private void Update()
    {
        if (!doOnce)
        {
            return;
        }

        if (Wind.felledTrees > treesTrigger)
        {
            doOnce = false; 
            isEnabled = true;
        }

        if (isEnabled)
        {
            
            musicBox.Post(gameObject);
            isEnabled = false; 
        }

    }

}
