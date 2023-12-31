﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using UnityEngine;
using Cinemachine;
//using CodeMonkey.Utils;
//using CodeMonkey;
//using Microsoft.Cci;
using Unity.VisualScripting;
using UnityEngine.Serialization;

public class Player_TreeChopping : MonoBehaviour, ITreeDamageable {

    [SerializeField] private Animator animator;
    [SerializeField] private Transform visualTransform;
    [SerializeField] private Transform cameraTransform;
    [SerializeField] private CinemachineImpulseSource treeShake;
    [SerializeField] private GameObject hitArea;
    [SerializeField] private GameObject fxTreeHit;
    [SerializeField] private GameObject fxTreeHitBlocks;
  


    //Movement
    public CharacterController controller;
    public float speed = 10f;
    public float gravity = -9.8f;
    public Transform groundCheck;
    public float groundDistance = 0.4f;
    public LayerMask groundMask;

    private Vector3 velocity;
    private bool isGrounded;
    public float jumpHeight = 3f;

    //Sounds
    public AK.Wwise.Event footstep;
    public AK.Wwise.Event woodChopping;
    public AK.Wwise.Event jump; 
    public AK.Wwise.Event landing;
    public AK.Wwise.Event rockHit; 
    public AK.Wwise.Event logCollision;

    public AK.Wwise.Switch footstepSwitch; 

    //Walking bools
    private bool isWalking = false;
    private float walkCount = 0.0f;
    /// The speed at which footstep sounds are triggered.
    [Range(0.01f, 1.0f)] public float footstepRate = 0.3f;
    ///	Used to ensure we play the Jump Land sound when we hit the ground.
    private bool isJumping = false;
    ///	Used to ensure we don't trigger a false Jump Land when the game starts.
    private int inAirCount = 16;


    private void Update()
    {
        HandleAttack();
        HandleMovement();
        

        if (((Mathf.Abs(Input.GetAxisRaw("Horizontal")) > 0.0f) ||
             (Mathf.Abs(Input.GetAxisRaw("Vertical")) > 0.0f)))
        {
            isWalking = true;
        }
        else
        {
            isWalking = false;

            walkCount = footstepRate;
        }

        if (!controller.isGrounded) //If controller isnt grounded its jumping
            isJumping = true;
        else
        {
            if (isJumping && (inAirCount < 1)) //
                landing.Post(gameObject);

            isJumping = false;
        }
        if (inAirCount > 0)
            --inAirCount;

        if (isWalking && !isJumping)
        {
            walkCount += Time.deltaTime * (speed / 10.0f);

            if (walkCount > footstepRate)
            {
                footstep.Post(gameObject); 

                walkCount = 0.0f;
            }
        }
    }


    private void AnimationEvent_OnHit() {
        // Find objects in Hit Area
        Vector3 colliderSize = Vector3.one /** .3f*/;
        Collider[] colliderArray = Physics.OverlapBox(hitArea.transform.position, colliderSize);
        foreach (Collider collider in colliderArray)
        {
            if (collider.TryGetComponent<ITreeDamageable>(out ITreeDamageable treeDamageable))
            {
                
                //Play wood chopping sound
                if (collider.TryGetComponent<Tree>(out Tree tree))
                {
                    tree.speciesSwitch.SetValue(gameObject);
                    woodChopping.Post(gameObject);
                }

                // Damage Popup
                int damageAmount = UnityEngine.Random.Range(10, 30);
                DamagePopup.Create(hitArea.transform.position, damageAmount, damageAmount > 14);

                // Damage Tree
                treeDamageable.Damage(damageAmount);


                // Shake Camera
                treeShake.GenerateImpulse();


            }

            if (collider.TryGetComponent<StumpSound>(out StumpSound stump) && collider == null)
            {
                footstepSwitch.SetValue(gameObject);
            }

            if (collider.CompareTag("Rock"))
            {

                rockHit.Post(gameObject);

            }
            
            Instantiate(fxTreeHit, hitArea.transform.position, Quaternion.identity);
            Instantiate(fxTreeHitBlocks, hitArea.transform.position, Quaternion.identity);
        }
    }

    private void HandleAttack() {
        if (Input.GetMouseButtonDown(0)) {
            AnimationEvent_OnHit();
           //if (animator != null) animator.SetTrigger("Attack");
           // FunctionTimer.Create(AnimationEvent_OnHit, .5f);
        }
    }

    private void HandleMovement()
    {

        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);

        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f; 
        }
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        Vector3 move = transform.right * x + transform.forward * z;

        controller.Move(move * speed * Time.deltaTime);

        if (Input.GetButtonDown("Jump") && isGrounded)
        {

            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
            jump.Post(gameObject);

        }
        velocity.y += gravity * Time.deltaTime;

        controller.Move(velocity * Time.deltaTime); 

    }

    private void OnCollisionEnter(Collision other)
    {
        if (other.collider is CapsuleCollider && isWalking)
        {
            logCollision.Post(gameObject);
        }

    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<StumpSound>(out StumpSound stump) && other.gameObject) //If the component is a stump
        {
            //Debug.Log("Overlapping with stump sphere radius");
            stump.soundMaterial.SetValue(gameObject);

        }

        if (other is CapsuleCollider)
        {
            logCollision.Post(gameObject); 
        }

        

    }

  


    private void OnTriggerExit(Collider other)
    {
        footstepSwitch.SetValue(gameObject);
        
    }
   
    public void Damage(int amount) {
        // Damage!
    }
}
