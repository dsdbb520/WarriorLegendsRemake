using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhysicsCheck : MonoBehaviour
{
    private CapsuleCollider2D coll;
    [Header("µ±Ç°×´Ì¬")]
    public bool manual;
    public bool isGround;
    public bool isGroundAnimation;
    public bool touchedLeftWall;
    public bool touchedRightWall;
    public float hangTime;
    [Header("¼ì²â·¶Î§")]
    public float checkRaduis;
    public float isGroundAnimationCheckRaduis;
    public LayerMask groundLayer;
    public Vector2 bottomOffset;
    public Vector2 leftOffset;
    public Vector2 rightOffset;
    // Start is called before the first frame update
    private void Awake()
    {
        coll = GetComponent<CapsuleCollider2D>();
        if (!manual)
        {
            rightOffset = new Vector2((coll.bounds.size.x + coll.offset.x) / 2, coll.bounds.size.y / 2);
            leftOffset = new Vector2(-rightOffset.x, rightOffset.y);
        }
    }

    // Update is called once per frame
    void Update()
    {
        Check();
        if (!isGround)
        {
            hangTime += Time.deltaTime;
        }
        if (isGround)
        {
            hangTime = 0;
        }
    }
    public void Check()
    {
        //¼ì²âµØÃæ
        isGround = Physics2D.OverlapCircle((Vector2)transform.position + bottomOffset, checkRaduis, groundLayer);   
        isGroundAnimation = Physics2D.OverlapCircle((Vector2)transform.position + bottomOffset, checkRaduis + isGroundAnimationCheckRaduis, groundLayer);
        touchedLeftWall = Physics2D.OverlapCircle((Vector2)transform.position + leftOffset, checkRaduis, groundLayer);
        touchedRightWall = Physics2D.OverlapCircle((Vector2)transform.position + rightOffset, checkRaduis, groundLayer);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere((Vector2)transform.position + bottomOffset, checkRaduis);
        Gizmos.DrawWireSphere((Vector2)transform.position + leftOffset, checkRaduis);
        Gizmos.DrawWireSphere((Vector2)transform.position + rightOffset, checkRaduis);
    }
}
