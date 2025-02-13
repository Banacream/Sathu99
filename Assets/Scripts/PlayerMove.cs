using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    public Inventory inventory;
    public HandleSlot handleSlot; // Add reference to HandleSlot

    public Rigidbody theRB;
    public float moveSpeed;
    private Vector2 moveInput;

    private bool isGrounded;
    private bool isCrouching = false;
    private bool isAttack = false;
    public float crouchSpeed = 2.0f;
    public float jumpForce = 15.0f;
    public float health = 100.0f;

    public Animator anim;
    public SpriteRenderer theSR;

    public float attackRange = 2.0f; // ระยะการโจมตี


    void Update()
    {
        anim.SetFloat("moveSpeed", theRB.velocity.magnitude);
        moveInput.x = Input.GetAxis("Horizontal");
        moveInput.y = Input.GetAxis("Vertical");
        moveInput.Normalize();

        // Check for crouch input
        if (Input.GetKey(KeyCode.LeftShift))
        {
            isCrouching = true;
        }
        else
        {
            isCrouching = false;
        }

        float currentSpeed = isCrouching ? crouchSpeed : moveSpeed;
        theRB.velocity = new Vector3(moveInput.x * currentSpeed, theRB.velocity.y, moveInput.y * currentSpeed);


        if (!theSR.flipX && moveInput.x < 0)
        {
            theSR.flipX = true;
        }
        else if (theSR.flipX && moveInput.x > 0)
        {
            theSR.flipX = false;
        }

        // Check for jump input
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            Jump();
        }

        // Check for mouse click to attack
        if (Input.GetMouseButtonDown(0)) // Left mouse button
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit))
            {
                Animal animal = hit.collider.GetComponent<Animal>();
                if (animal != null && Vector3.Distance(transform.position, animal.transform.position) <= attackRange)
                {
                    Attack(animal);
                }
            }
        }
    }
    private void Jump()
    {
        theRB.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        isGrounded = false;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = true;
        }
    }
    public void Attack(Animal animal)
    {
        animal.TakeDamage(handleSlot); // Use HandleSlot to check weapon
    }

    public bool AttackforRunAway()
    {
        return isAttack; // Use HandleSlot to check weapon
    }

    public bool IsCrouching()
    {
        return isCrouching;
    }

    public float GetCurrentSpeed()
    {
        return theRB.velocity.magnitude;
    }

    // ฟังก์ชันรับดาเมจ
    public void TakeDamage(float damage)
    {
        health -= damage;
        if (health <= 0)
        {
            Die();
        }
    }

    // ฟังก์ชันจัดการการตายของผู้เล่น
    private void Die()
    {
        Debug.Log("Player has died.");
        // เพิ่มการจัดการการตายของผู้เล่น เช่น การรีสตาร์ทเกมหรือแสดงหน้าจอ Game Over
    }
}
