using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMovimentScript : MonoBehaviour
{
    //Ground variables
    [SerializeField]
    private Transform groundLimit;
    [SerializeField]
    private Transform groundCheck;
    private float groundRadius = .1f;
    private bool isGroundAhead;
    private bool isGrounded = false;
    [SerializeField]
    private LayerMask groundLayer;

    //Direction variables
    private float direction = 1;
    private bool m_FacingRight = true;

    //Moviment variables
    [Range(0, .3f)] [SerializeField] private float m_MovementSmoothing = .05f;
    private Vector3 m_Velocity = Vector3.zero;
    private Vector3 targetVelocity;
    [SerializeField]
    private float normalSpeed = 20f;
    [SerializeField]
    private Transform rayPosition;
    [SerializeField]
    private float rayLength = 1f;
    private RaycastHit2D enemyAhead;
    [SerializeField]
    private LayerMask playerMask;
    [SerializeField]
    private float runSpeed = 40f;

    private Rigidbody2D enemyRb;

    private void Start()
    {
        enemyRb = GetComponent<Rigidbody2D>();
    }

    private void FixedUpdate()
    {
        Collider2D groundColliders = Physics2D.OverlapCircle(groundLimit.position, groundRadius, groundLayer);

        if (groundColliders == null)
        {
            isGroundAhead = false;
        }
        else
        {
            isGroundAhead = true;
        }

        Collider2D groundCheckColliders = Physics2D.OverlapCircle(groundCheck.position, groundRadius, groundLayer);
        if (groundCheckColliders != null)
        {
            isGrounded = true;            
        }
        else
        {
            isGrounded = false;
        }

        Move();
    }

    private void Move()
    {
        
        
        if (isGroundAhead == false)
        {
            direction = -direction;
        }

        enemyAhead = Physics2D.Raycast(rayPosition.position, rayPosition.transform.right, rayLength, playerMask);
        
        if (enemyAhead.collider != null)
        {
            targetVelocity = new Vector2(direction * runSpeed, enemyRb.velocity.y);
        }
        else
        {
            targetVelocity = new Vector2(direction * normalSpeed, enemyRb.velocity.y);
        }              

        enemyRb.velocity = Vector3.SmoothDamp(enemyRb.velocity, targetVelocity, ref m_Velocity, m_MovementSmoothing);


        if (isGrounded)
        {
            if (direction > 0 && !m_FacingRight)
            {
                Flip();
            }
            else if (direction < 0 && m_FacingRight)
            {
                Flip();
            }
        }       
        
    }

    public void Flip()
    {
        // Switch the way the player is labelled as facing.
        m_FacingRight = !m_FacingRight;

        transform.Rotate(0f, 180f, 0f);
    }

    public void ChangeDirection()
    {
        direction = -direction;               
    }
}
