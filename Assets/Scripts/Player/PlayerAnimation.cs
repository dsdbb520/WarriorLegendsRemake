using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimation : MonoBehaviour
{
    private Control control;
    private Animator animator;
    public Rigidbody2D rb;
    public PhysicsCheck physicsCheck;
    private void Awake()
    {
        control = GetComponent<Control>();
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        physicsCheck = GetComponent<PhysicsCheck>();
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        SetAnimation();
    }

    public void SetAnimation()
    {
        animator.SetFloat("velocityX", Mathf.Abs(rb.velocity.x));
        animator.SetFloat("velocityY", Mathf.Abs(rb.velocity.y));
        animator.SetBool("isGround", physicsCheck.isGroundAnimation);
        animator.SetBool("isDead", control.isDead);
        animator.SetBool("isAttack", control.isAttack);
    }

    public void Attack()
    {
        animator.SetTrigger("Attacking");
    }

    public void Dodge()
    {
        animator.SetTrigger("Dodge");
    }

    public void Injurt()
    {
        animator.SetTrigger("Injurt");
    }

}
