using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Snake : AnimalEnemy
{
    public GameObject snakeMeatPrefab;

    public float normalSpeed; // ความเร็วปกติของสุนัข
    public float chaseSpeed; // ความเร็วเมื่อไล่ผู้เล่น
    public float attackDamage = 10.0f; // ความเสียหายเมื่อโจมตีผู้เล่น
    public float attackCooldown = 2.0f; // ระยะเวลาคูลดาวน์ระหว่างการโจมตี

    private PlayerMove playerMove;
    private Color originalColor;

    public Animator anim;

    public List<SpriteRenderer> spriteRenderers;
    private bool isFlipped = false; // สถานะการฟลิป
    private bool canAttack = true; // สถานะการโจมตี
    public Snake() : base("Snake", 50, null) { }
    public AudioSource audioSource;

    protected override void Start()
    {
        base.Start();
        //DropItem = snakeMeatPrefab.name; // Assign the drop item

        // ตรวจสอบและตั้งค่า NavMeshAgent
        float randomDelay = Random.Range(0f, 5f); // สุ่มดีเลย์ 0-2 วินาที
        audioSource.PlayDelayed(randomDelay);
        navMeshAgent = GetComponent<NavMeshAgent>();
        if (navMeshAgent != null)
        {
            navMeshAgent.speed = normalSpeed; // ตั้งค่าความเร็วเริ่มต้น
        }
        else
        {
            Debug.LogError("NavMeshAgent not found on Dog.");
        }

        // หา PlayerMove component
        FindPlayerMove();
        if (spriteRenderers.Count > 0)
        {
            originalColor = spriteRenderers[0].color;
        }
    }

    protected override void Update()
    {
        base.Update();
        anim.SetFloat("Speed", navMeshAgent.velocity.magnitude);
        // หา PlayerMove component หากยังไม่พบ
        if (playerMove == null)
        {
            FindPlayerMove();
        }

        if (playerMove != null && navMeshAgent != null)
        {
            float distanceToPlayer = Vector3.Distance(transform.position, playerMove.transform.position);
            if (distanceToPlayer < ChaseDistance)
            {
                navMeshAgent.speed = chaseSpeed; // เพิ่มความเร็วเมื่อไล่ผู้เล่น
            }
            else
            {
                navMeshAgent.speed = normalSpeed; // ความเร็วปกติเมื่อผู้เล่นอยู่ไกล
            }

            // ฟลิปด้านตามทิศทางการเคลื่อนที่
            if (navMeshAgent.velocity.x > 0 && !isFlipped)
            {
                SetFlipX(true);
                isFlipped = true;
            }
            else if (navMeshAgent.velocity.x < 0 && isFlipped)
            {
                SetFlipX(false);
                isFlipped = false;
            }
        }
    }

    private void SetFlipX(bool flipX)
    {
        foreach (SpriteRenderer sr in spriteRenderers)
        {
            sr.flipX = flipX;
        }
    }

    public override void TakeDamage(HandleSlot weaponSlot)
    {
        base.TakeDamage(weaponSlot);
        StartCoroutine(FlashRed());
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

    private void FindPlayerMove()
    {
        GameObject player = GameObject.FindWithTag("Player");
        if (player != null)
        {
            playerMove = player.GetComponent<PlayerMove>();
            if (playerMove == null)
            {
                Debug.LogError("PlayerMove component not found on Player.");
            }
        }
        else
        {
            Debug.LogError("Player not found in the scene.");
        }
    }

    protected override void AttackPlayer()
    {
        if (playerMove != null && canAttack)
        {
            // หยุดการเคลื่อนที่
            navMeshAgent.isStopped = true;

            // เล่นอนิเมชั่นการโจมตี
            anim.SetTrigger("Attack");

            // ลดพลังชีวิตของผู้เล่น
            playerMove.TakeDamage(attackDamage);
            Debug.Log("Dog attacked the player!");

            // เริ่มคูลดาวน์การโจมตี
            StartCoroutine(AttackCooldown());
        }
    }

    private IEnumerator AttackCooldown()
    {
        canAttack = false;
        yield return new WaitForSeconds(attackCooldown);
        canAttack = true;
        // กลับมาเคลื่อนที่อีกครั้งหลังจากคูลดาวน์เสร็จสิ้น
        navMeshAgent.isStopped = false;
    }


    //protected override void AttackPlayer()
    //{
    //    if (playerMove != null)
    //    {
    //        // Example: Reduce player's health
    //        playerMove.TakeDamage(attackDamage);
    //        Debug.Log("Snake attacked the player!");
    //    }
    //}

    protected override bool IsWeaponEffective(string weapon)
    {
        return weapon == "001_Stick"; // Only effective with a stick
    }
}
