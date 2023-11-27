using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using CodeMonkey.Utils;
using CodeMonkey;

public class Player_TreeChopping : MonoBehaviour, ITreeDamageable {

    [SerializeField] private Animator animator;
    [SerializeField] private Transform visualTransform;
    [SerializeField] private Transform cameraTransform;
    [SerializeField] private CinemachineImpulseSource treeShake;
    [SerializeField] private GameObject hitArea;
    [SerializeField] private GameObject fxTreeHit;
    [SerializeField] private GameObject fxTreeHitBlocks;


    private void Awake() {
        Cursor.lockState = CursorLockMode.Locked;
    }

    private void AnimationEvent_OnHit() {
        // Find objects in Hit Area
        Vector3 colliderSize = Vector3.one * .3f;
        Collider[] colliderArray = Physics.OverlapBox(hitArea.transform.position, colliderSize);
        foreach (Collider collider in colliderArray) {
            if (collider.TryGetComponent<ITreeDamageable>(out ITreeDamageable treeDamageable)) {
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

    private void Update() {
        HandleAttack();
        HandleMovement();
    }

    private void HandleAttack() {
        if (Input.GetMouseButtonDown(0)) {
            if (animator != null) animator.SetTrigger("Attack");
            FunctionTimer.Create(AnimationEvent_OnHit, .5f);
        }
    }

    private void HandleMovement() {
        float moveX = 0f;
        float moveZ = 0f;

        if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.W)) {
            moveZ = +1f;
        }
        if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.S)) {
            moveZ = -1f;
        }
        if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.A)) {
            moveX = -1f;
        }
        if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.D)) {
            moveX = +1f;
        }

        if (moveX != 0 || moveZ != 0) {
            // Not idle
            if (animator != null) animator.SetBool("IsWalking", true);
        } else {
            if (animator != null) animator.SetBool("IsWalking", false);
        }

        Vector3 moveDir = (new Vector3(cameraTransform.forward.x, 0f, cameraTransform.forward.z) * moveZ) +
            (new Vector3(cameraTransform.right.x, 0f, cameraTransform.right.z) * moveX);
        moveDir = moveDir.normalized;

        transform.forward = new Vector3(cameraTransform.forward.x, 0f, cameraTransform.forward.z);
        float moveSpeed = 3f;
        transform.position += moveDir * moveSpeed * Time.deltaTime;

        visualTransform.localPosition += -visualTransform.localPosition * Time.deltaTime * .5f;
    }

    public void Damage(int amount) {
        // Damage!
    }
}
