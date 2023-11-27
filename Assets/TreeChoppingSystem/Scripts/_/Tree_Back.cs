using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CodeMonkey.Utils;

public class Tree_Back : MonoBehaviour, ITreeDamageable {

    [SerializeField] private Transform fxTreeDestroyed;
    [SerializeField] private Transform treeLog;
    [SerializeField] private Transform treeStump;

    private HealthSystem healthSystem;

    private void Awake() {
        healthSystem = new HealthSystem(30);
        healthSystem.OnDead += HealthSystem_OnDead;
    }

    private void HealthSystem_OnDead(object sender, System.EventArgs e) {
        healthSystem.OnDead -= HealthSystem_OnDead;

        Instantiate(fxTreeDestroyed, transform.position, Quaternion.identity);

        Vector3 treeLogOffset = transform.up * .8f;
        Instantiate(treeLog, transform.position + treeLogOffset, Quaternion.Euler(Random.Range(-1.5f, +1.5f), 0, Random.Range(-1.5f, +1.5f)));

        Instantiate(treeStump, transform.position, Quaternion.identity);

        Destroy(gameObject);
    }

    public void Damage(int damageAmount) {
        healthSystem.Damage(10);
    }

}
