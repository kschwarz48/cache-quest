using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class Slime : MonoBehaviour
{

    private Animator animator;

    public float Health {
        set {
            health = value;

            if(value <= 0)
            {
                animator.SetTrigger("hit");
            }

            if (health <= 0)
            {
                animator.SetTrigger("death");
            }
        }
        get {
            return health;
        }
    }

    public float health = 3;

    public void Start()
    {
        animator = GetComponent<Animator>();
    }

    void OnHit(float damage)
    {
        Health -= damage;
        animator.SetTrigger("hit");
    }
}