using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Birds : MonoBehaviour
{
    public AK.Wwise.Event birdsPlay;
    public static GameObject birdRef { get; private set; }

    private void Awake()
    {
        birdRef = this.gameObject; 
    }

    // Start is called before the first frame update
    void Start()
    {
        birdsPlay.Post(gameObject); 
        
    }

    // Update is called once per frame
    void Update()
    {
    }
}
