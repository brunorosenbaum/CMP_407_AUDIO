using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CodeMonkey.Utils;
using CodeMonkey;
using Random = UnityEngine.Random;

public class Tree : MonoBehaviour, ITreeDamageable {

    public enum Type {
        Tree,
        Log,
        LogHalf,
        Stump
    }

    [SerializeField] public Type treeType;
    [SerializeField] private Transform fxTreeDestroyed;
    [SerializeField] private Transform fxTreeLogDestroyed;
    [SerializeField] private Transform fxTreeLogHalfDestroyed;
    [SerializeField] private Transform fxTreeStumpDestroyed;
    [SerializeField] private Transform treeLog;
    [SerializeField] private Transform treeLogHalf;
    [SerializeField] private Transform treeStump;

    private HealthSystem healthSystem;

    public GameObject birds; 

    //Sounds
    public AK.Wwise.Event felling; 
    public AK.Wwise.Event fallDown; 
    public AK.Wwise.Event rockLog; 
    public AK.Wwise.Event logHit; 
    public AK.Wwise.Event stopLeaves; 
    public AK.Wwise.Event lowerBirdVolume; 


    public AK.Wwise.Switch speciesSwitch;
    public AK.Wwise.Switch leavesRustling;


    private void Awake() {
        int healthAmount;

        switch (treeType) {
            default:
            case Type.Tree:     healthAmount = 30; break;
            case Type.Log:      healthAmount = 50; break;
            case Type.LogHalf:  healthAmount = 50; break;
            case Type.Stump:    healthAmount = 50; break;
        }

        healthSystem = new HealthSystem(healthAmount);
        healthSystem.OnDead += HealthSystem_OnDead;
        
    }

    private void HealthSystem_OnDead(object sender, System.EventArgs e) {
        switch (treeType) {
            default:
            case Type.Tree:
                // Spawn FX
                Instantiate(fxTreeDestroyed, transform.position, transform.rotation);

                // Spawn Log
                Vector3 treeLogOffset = transform.up * .8f;
                Instantiate(treeLog, transform.position + treeLogOffset, Quaternion.Euler(Random.Range(-1.5f, +1.5f), 0, Random.Range(-1.5f, +1.5f)));

                // Spawn Stump
                Instantiate(treeStump, transform.position, transform.rotation);
                
                //Play tree felling sound
                stopLeaves.Post(gameObject); 
                felling.Post(gameObject);
                lowerBirdVolume.Post(Birds.birdRef); 
                break;
            case Type.Log:
                // Spawn FX
                Instantiate(fxTreeLogDestroyed, transform.position, transform.rotation);

                // Spawn Log Half
                float logYPositionAboveStump = .8f;
                treeLogOffset = transform.up * logYPositionAboveStump;
                Instantiate(treeLogHalf, transform.position + treeLogOffset, transform.rotation);

                // Spawn Log Half
                float logYPositionAboveFirstLogHalf = 5.1f;
                treeLogOffset = transform.up * logYPositionAboveFirstLogHalf;
                Instantiate(treeLogHalf, transform.position + treeLogOffset, transform.rotation);
                break;
            case Type.LogHalf:
                // Spawn FX
                Instantiate(fxTreeLogHalfDestroyed, transform.position, transform.rotation);
                break;
            case Type.Stump:
                // Spawn FX
                Instantiate(fxTreeStumpDestroyed, transform.position, transform.rotation);
                break;
        }

        Destroy(gameObject);
    }

    public void Damage(int amount) {
        healthSystem.Damage(amount);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.TryGetComponent<ITreeDamageable>(out ITreeDamageable treeDamageable))
        {
            if (collision.relativeVelocity.magnitude > 1f)
            {
                logHit.Post(gameObject); 
                int damageAmount = Random.Range(5, 20);
                DamagePopup.Create(collision.GetContact(0).point, damageAmount, damageAmount > 14);
                treeDamageable.Damage(damageAmount);
            }
        }

        if ( treeType == Type.Log || treeType == Type.LogHalf)
        {
            foreach (ContactPoint contact in collision.contacts)
            {
                if (gameObject.GetComponent<Rigidbody>().velocity.magnitude > 0.1f)
                {
                    if(collision.gameObject.CompareTag("Ground"))
                        fallDown.Post(gameObject);
                    if (collision.gameObject.CompareTag("Rock"))
                        rockLog.Post(gameObject); 


                }
            }

        }



    }

}
