using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using UnityEngine;
using Cinemachine;
using CodeMonkey.Utils;
using CodeMonkey;
using Microsoft.Cci;
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
    public AK.Wwise.Event footstep = new AK.Wwise.Event();
    public AK.Wwise.Event woodChopping;
    public AK.Wwise.Event jump; 
    public AK.Wwise.Event landing; 

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
        FootstepSound();

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
        Vector3 colliderSize = Vector3.one * .3f;
        Collider[] colliderArray = Physics.OverlapBox(hitArea.transform.position, colliderSize);
        foreach (Collider collider in colliderArray) {
            if (collider.TryGetComponent<ITreeDamageable>(out ITreeDamageable treeDamageable)) {

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

                // Spawn FX
                Instantiate(fxTreeHit, hitArea.transform.position, Quaternion.identity);
                Instantiate(fxTreeHitBlocks, hitArea.transform.position, Quaternion.identity);
            }
        }
    }

    

    private void HandleAttack() {
        if (Input.GetMouseButtonDown(0)) {
            if (animator != null) animator.SetTrigger("Attack");
            FunctionTimer.Create(AnimationEvent_OnHit, .5f);
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

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Overlapping with stump sphere radius");

    }

    private void FootstepSound()
    {
        
        Collider[] colliderArray = //Will return an array of colliders that the player's capsule collider overlaps with
            Physics.OverlapSphere(transform.position, controller.radius);
        foreach (Collider collider in colliderArray) //Loop through them
        {
            if (collider.TryGetComponent<Tree>(out Tree tree) ) //If the component is a stump
            {
                OnTriggerEnter(collider);
            }
        }


    }
    public void Damage(int amount) {
        // Damage!
    }
}
