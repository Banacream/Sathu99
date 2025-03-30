using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class PlayerMove : MonoBehaviour
{
    AudioManager audioManager;

    public Inventory inventory;
    public PanelManager panelManager;
    public HandleSlot handleSlot; // Add reference to HandleSlot
    public GameObject confirmForDiePanel;
    public Rigidbody theRB;
    public float moveSpeed;
    private Vector2 moveInput;

    private bool isGrounded;
    private bool isCrouching = false;
    private bool isAttack = false;
    private bool canAttack = true; // สถานะการโจมตี
    private bool isAttacking = false; // สถานะการโจมตี
    public float crouchSpeed = 2.0f;
    public float jumpForce = 15.0f;
    public float fallMultiplier = 2.5f;
    public float health = 2.0f;

    public Animator anim;

    public List<string> validWeapons;
    public List<SpriteRenderer> spriteRenderers;
    public List<SpriteRenderer> armSpriteRenderer; // SpriteRenderer ที่เป็นแขน
    public float attackRange = 8.0f; // ระยะการโจมตี
    private bool isFlipped = false; // สถานะการฟลิป
    private static readonly DataItem EMTRY_ITEM;
    private Color originalColor;

    public string sceneName;


    private void Awake()
    {
        audioManager = GameObject.FindGameObjectWithTag("Audio").GetComponent<AudioManager>();
    }

    private void Start()
    {
 
        if (spriteRenderers.Count > 0)
        {
            originalColor = spriteRenderers[0].color;
        }
    }


    void Update()
    {
        AnimatorStateInfo currentState = anim.GetCurrentAnimatorStateInfo(0);
        if (!currentState.IsName("Attack")& !currentState.IsName("Attack&Walk")) // ถ้าอนิเมชั่นที่เล่นอยู่ไม่ใช่อนิเมชั่นตาย
        {
            anim.SetFloat("Speed", theRB.velocity.magnitude);// อัปเดตความเร็วในอนิเมชั่น
        }
       
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

        float currentSpeed = isCrouching ? crouchSpeed : (isAttacking ? moveSpeed / 2 : moveSpeed);
        theRB.velocity = new Vector3(moveInput.x * currentSpeed, theRB.velocity.y, moveInput.y * currentSpeed);

        if (theRB.velocity.magnitude >= 1 && isGrounded)
        {
            if (!audioManager.IsPlaying())
            {
                audioManager.PlayfootStepsmusicSource(audioManager.footSteps);
            }
        }
        else
        {
            audioManager.StopSFX();
        }


        if (!isAttacking)
        {
            if (moveInput.x > 0 && !isFlipped)
            {
                SetFlipX(true);
                isFlipped = true;
            }
            else if (moveInput.x < 0 && isFlipped)
            {
                SetFlipX(false);
                isFlipped = false;
            }
        }


        //if (!theSR.flipX && moveInput.x < 0)
        //{
        //    theSR.flipX = true;
        //}
        //else if (theSR.flipX && moveInput.x > 0)
        //{
        //    theSR.flipX = false;
        //}

        // Check for jump input
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            Jump();
        }

        // Apply additional gravity when falling
        if (theRB.velocity.y < 0)
        {
            theRB.velocity += Vector3.up * Physics.gravity.y * (fallMultiplier - 1) * Time.deltaTime;
        }
        else if (theRB.velocity.y > 0 && !Input.GetButton("Jump"))
        {
            theRB.velocity += Vector3.up * Physics.gravity.y * (fallMultiplier - 1) * Time.deltaTime;
        }

        // Check for mouse click to attack
        if (Input.GetMouseButtonDown(0) && canAttack) // Left mouse button
        {
           
                if (validWeapons.Contains(handleSlot.item.name)) // Check if there is a valid weapon in the handle slot
                {
                
                    Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                    RaycastHit hit;
                    if (Physics.Raycast(ray, out hit))
                    {
                        Animal animal = hit.collider.GetComponent<Animal>();
                        AnimalEnemy animalEnemy = hit.collider.GetComponent<AnimalEnemy>();
                   
                    if (animal != null && Vector3.Distance(transform.position, animal.transform.position) <= attackRange)
                        {
                        Attack(animal);
                        StartCoroutine(AttackCooldown());
                    }
                    if (animalEnemy != null && Vector3.Distance(transform.position, animalEnemy.transform.position) <= attackRange)
                    {
                        AttackEnemy(animalEnemy);
                        StartCoroutine(AttackCooldown());
                    }
                }
                }
                else
                {
                    Debug.Log("No valid weapon in handle slot!");
                }
            

        }

        if (health < 10)
        {
            panelManager.ShowLowHealthWarningLevel3();
        }
        else if (health < 15)
        {
            panelManager.ShowLowHealthWarningLevel2();
        }
        else if (health < 25)
        {
            panelManager.ShowLowHealthWarningLevel1();
        }
        else
        {
            panelManager.HideLowHealthWarning();
        }


    }


    private void SetFlipX(bool flipX)
    {
        foreach (SpriteRenderer sr in spriteRenderers)
        {
            sr.flipX = flipX;
        }

        // Flip และเลื่อนตำแหน่ง X ของแขนเมื่อถูกฟลิป
        if (armSpriteRenderer != null)
        {
            foreach (SpriteRenderer sr in armSpriteRenderer)
            {
                sr.flipX = flipX;
                Vector3 armPosition = sr.transform.localPosition;
                armPosition.x = -0.5f; // ตั้งค่า X ของแขนเป็น 0
                sr.transform.localPosition = armPosition;
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
        isAttacking = true;


        if (animal != null)
        {
            if (animal.transform.position.x < transform.position.x && isFlipped) // ถ้าเป้าหมายอยู่ทางขวาและตัวละครหันซ้าย
            {
                SetFlipX(false); // หันไปทางขวา
                isFlipped = false;
            }
            else if (animal.transform.position.x > transform.position.x && !isFlipped) // ถ้าเป้าหมายอยู่ทางซ้ายและตัวละครหันขวา
            {
                SetFlipX(true); // หันไปทางซ้าย
                isFlipped = true;
            }
        }
        anim.SetFloat("Speed",0f);
        if (theRB.velocity.magnitude >= 1)
        {
            anim.SetTrigger("Attack&Walk");
            audioManager.PlaySFX(audioManager.attackPlayer);
        }
        else
        {
            anim.SetTrigger("Attack");
            audioManager.PlaySFX(audioManager.attackPlayer);
        }
        animal.TakeDamage(handleSlot); // Use HandleSlot to check weapon
    }

    public void AttackEnemy(AnimalEnemy animalEnemy)
    {
        isAttacking = true;
        if (theRB.velocity.magnitude > 0)
        {
            anim.SetTrigger("Attack&Walk");
            audioManager.PlaySFX(audioManager.attackPlayer);
        }
        else
        {
            anim.SetTrigger("Attack");
            audioManager.PlaySFX(audioManager.attackPlayer);
        }
        animalEnemy.TakeDamage(handleSlot); // Use HandleSlot to check weapon
    }

    private IEnumerator AttackCooldown()
    {
        canAttack = false;
        yield return new WaitForSeconds(1.5f); // คูลดาวน์ 2 วินาที
        canAttack = true;
        isAttacking = false;
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
        audioManager.PlaySFX(audioManager.playerDamage);
        StartCoroutine(FlashRed());
       

        if (health <= 0)
        {
            Die();
        }
    }

    private IEnumerator FlashRed()
    {
        foreach (SpriteRenderer sr in spriteRenderers)
        {
            sr.color = Color.red; // เปลี่ยนสีเป็นสีแดง
        }
        yield return new WaitForSeconds(0.2f); // รอ 0.2 วินาที
        foreach (SpriteRenderer sr in spriteRenderers)
        {
            sr.color = originalColor; // เปลี่ยนสีกลับเป็นสีเดิม
        }
    }

    // ฟังก์ชันจัดการการตายของผู้เล่น
    private void Die()
    {

        confirmForDiePanel.SetActive(true);
        Time.timeScale = 0;
        Debug.Log("Player has died.");
        // เพิ่มการจัดการการตายของผู้เล่น เช่น การรีสตาร์ทเกมหรือแสดงหน้าจอ Game Over
    }
}
