using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public enum BulletStates {HitUnit, HitNothing}

    public float force;
    private Rigidbody rb;
    public GameObject attackTarget;    // target to attack
    public int damage;
    private Vector3 direction;
    private BulletStates bulletStates;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        bulletStates = BulletStates.HitUnit;
        FlyToTarget();
    }

    // Update is called once per frame
    void Update()
    {
    }

    public void FlyToTarget()
    {
        // move to attack target
        direction = (attackTarget.transform.position - transform.position + Vector3.up).normalized;
        rb.AddForce(direction * force, ForceMode.Impulse);
    }

    void OnCollisionEneter(Collision other)
    {
        switch (bulletStates)
        {
            case BulletStates.HitUnit:
                // if hit unit, unit get hitted take damage
                if (other.gameObject.CompareTag("Character"))
                {
                    other.gameObject.GetComponent<CharacterStats>().TakeDamage(damage, other.gameObject.GetComponent<CharacterStats>());
                    bulletStates = BulletStates.HitNothing;
                }
                break;
        }
        Destroy(gameObject, 2f);
    }
}
