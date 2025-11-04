using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Attack : MonoBehaviour
{
    public int damage;
    public float attackRange;
    public float attackSpeed;



    private void OnTriggerStay2D(Collider2D collision)
    {
        Debug.Log("¹¥»÷´¥·¢Æ÷Åöµ½: " + collision.name);
        var character = collision.GetComponent<Character>();
        if (character != null)
        {
            Debug.Log("ÃüÖÐ Character: " + collision.name);
            character.TakeDamage(this);
        }
    }
}
