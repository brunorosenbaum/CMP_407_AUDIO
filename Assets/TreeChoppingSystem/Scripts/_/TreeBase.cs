using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CodeMonkey.Utils;
using CodeMonkey;

public class TreeBase : MonoBehaviour, ITreeDamageable {

    [SerializeField] private Transform fxTreeLogHalfDestroyed;

    private HealthSystem healthSystem;

    private void Awake() {
        healthSystem = new HealthSystem(30);
        healthSystem.OnDead += HealthSystem_OnDead;
    }

    private void HealthSystem_OnDead(object sender, System.EventArgs e) {
        healthSystem.OnDead -= HealthSystem_OnDead;

        Instantiate(fxTreeLogHalfDestroyed, transform.position, transform.rotation);

        Destroy(gameObject);
    }

    public void Damage(int amount) {
        healthSystem.Damage(10);
    }

}
