using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Wind : MonoBehaviour
{
    public static GameObject windRef { get; private set; }
    public static int felledTrees;
     public static bool isTreeChopped; 

    
    //Sounds
    public AK.Wwise.Event playWind;
    public AK.Wwise.Event increaseWind;
    public AK.Wwise.RTPC windIntensity; 
    int temp = 0;
   
    private void Awake()
    {
        isTreeChopped = false;
        windRef = this.gameObject;
    }

   
    // Start is called before the first frame update
    void Start()
    {
       
        playWind.Post(gameObject); //This event will play, but it'll be quiet
        windIntensity.SetGlobalValue(0);
        felledTrees = 0;
    }

    // Update is called once per frame
    void Update()
    {
        
        if (felledTrees > 3)
        { //When enough trees are cut down, volume of the wind starts rising
            if (isTreeChopped)
            {
                //Debug.Log("Tree increase");
                temp++; 
                windIntensity.SetValue(gameObject, temp);
                increaseWind.Post(gameObject);
                isTreeChopped = false; 
            }
        }

    }

    
}
