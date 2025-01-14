using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    // Start is called before the first frame update
    public Inventory inventory;

    public Rigidbody theRB;
    public float moveSpeed;
    private Vector2 moveInput;
    private bool isGrounded;

    public Animator anim;
    public SpriteRenderer theSR;

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        anim.SetFloat("moveSpeed", theRB.velocity.magnitude);
        moveInput.x = Input.GetAxis("Horizontal");
        moveInput.y = Input.GetAxis("Vertical");
        moveInput.Normalize();

        theRB.velocity = new Vector3(moveInput.x * moveSpeed, theRB.velocity.y, moveInput.y * moveSpeed);

        //anim.SetBool("onGround",isGrounded );
        if(!theSR.flipX && moveInput.x < 0)
        {
            theSR.flipX = true;
        }
        else if(theSR.flipX && moveInput.x > 0)
        {
            theSR.flipX = false;
        }

    }
}
