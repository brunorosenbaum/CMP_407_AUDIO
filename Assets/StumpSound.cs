using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StumpSound : MonoBehaviour
{
   //A class for retrieving a sound material to turn on a switch
   //For the ground around the stump's sphere collider to change the footstep sound
   //As it'll simulate tree branches and twigs as a result of the felling

   //This can be done via the editor as well but this is also to showcase it's possible in code. 
   public AK.Wwise.Switch soundMaterial;
   //public AK.Wwise.Switch destroyMtl;
   private void OnTriggerEnter(Collider other)
   {
       if (other.TryGetComponent<CapsuleCollider>(out CapsuleCollider capsuleCollider)
           && other.TryGetComponent<Tree>(out Tree tree)) //If the component is a stump
       {

            Physics.IgnoreCollision(capsuleCollider, GetComponent<SphereCollider>(), true);
            Physics.IgnoreCollision(capsuleCollider, GetComponent<CapsuleCollider>(), true);


       }

   }

  
}
