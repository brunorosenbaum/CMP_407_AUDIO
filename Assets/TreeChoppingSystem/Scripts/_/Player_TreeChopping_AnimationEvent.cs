using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_TreeChopping_AnimationEvent : MonoBehaviour {

    public event EventHandler OnHit;


    public void Hit() {
        OnHit?.Invoke(this, EventArgs.Empty);
    }

}
