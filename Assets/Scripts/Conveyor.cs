using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Conveyor : MonoBehaviour
{
    [SerializeField] float speed = 10;

    private void OnCollisionStay2D(Collision2D collision)
    {
        Rigidbody2D rb;
        if (collision.transform.TryGetComponent<Rigidbody2D>(out rb))
        {
            rb.velocity = new Vector2(speed, 0) + rb.velocity;
        }
    }
}
