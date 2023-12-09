using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Birds : MonoBehaviour
{
    public AK.Wwise.Event birdsPlay; 
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
