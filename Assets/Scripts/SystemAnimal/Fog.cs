using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Fog : Animal
{
    public GameObject fogMeatPrefab;

    public float normalSpeed; // ความเร็วปกติของไก่
    public float slowSpeed; // ความเร็วปกติของไก่
    public float runAwaySpeed; // ความเร็วเมื่อวิ่งหนี

    public Animator anim;
    public List<SpriteRenderer> spriteRenderers;

    private bool isFlipped = false; // สถานะการฟลิป
    private bool isStopped = false; // สถานะการหยุดนิ่ง
    private bool isDeadF = false; // สถานะการตาย

    private PlayerMove playerMove;
    private Color originalColor;
    public AudioSource audioSource;
    public Fog() : base("Fog", 30, null) { }

    protected override void Start()
    {
        base.Start();
        float randomDelay = Random.Range(0f, 5f); // สุ่มดีเลย์ 0-2 วินาที
        audioSource.PlayDelayed(randomDelay);
        navMeshAgent = GetComponent<NavMeshAgent>();
        DropItemPrefab = fogMeatPrefab;// Assign the drop item
        // Example: Move to a specific position when spawned
        //MoveTo(new Vector3(Random.Range(-5, 5), 0, Random.Range(-5, 5)));

        navMeshAgent = GetComponent<NavMeshAgent>();
        if (navMeshAgent != null)
        {
            navMeshAgent.speed = normalSpeed; // ตั้งค่าความเร็วเริ่มต้น
        }
        else
        {
            Debug.LogError("NavMeshAgent not found on Chicken.");
        }


        FindPlayerMove();
        if (spriteRenderers.Count > 0)
        {
            originalColor = spriteRenderers[0].color;
        }

    }

    protected override void Update()
    {
        if (isDeadF) return; // ถ้าหมาตายแล้ว ไม่รับความเสียหายอีก

        base.Update();

        AnimatorStateInfo currentState = anim.GetCurrentAnimatorStateInfo(0);
        if (!currentState.IsName("Dead")) // ถ้าอนิเมชั่นที่เล่นอยู่ไม่ใช่อนิเมชั่นตาย
        {
            anim.SetFloat("Speed", navMeshAgent.velocity.magnitude); // อัปเดตความเร็วในอนิเมชั่น
        }

        // หา PlayerMove component หากยังไม่พบ
        if (playerMove == null)
        {
            FindPlayerMove();
        }

        if (playerMove != null && navMeshAgent != null)
        {
            float distanceToPlayer = Vector3.Distance(transform.position, playerMove.transform.position);
            if (distanceToPlayer < EnemyDistanceRun)
            {
                if (playerMove.IsCrouching())
                {
                    if (!isStopped)
                    {
                        StartCoroutine(StopAndResume());
                    }
                }
                else if (playerMove.GetCurrentSpeed() > playerMove.moveSpeed)
                {
                    navMeshAgent.speed = runAwaySpeed; // เพิ่มความเร็วเมื่อผู้เล่นเข้าหาด้วยความเร็วสูง
                }
                else
                {
                    navMeshAgent.speed = normalSpeed; // ความเร็วปกติ
                }
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
        if (isDeadF) return; // ถ้าหมาตายแล้ว ไม่รับความเสียหายอีก

        base.TakeDamage(weaponSlot);

        if (Health <= 0)
        {
            StartCoroutine(HandleDeath());
        }
        else
        {
            StartCoroutine(FlashRed());
        }
    }

    private IEnumerator HandleDeath()
    {
        isDeadF = true; // ตั้งสถานะว่าหมาตายแล้ว
        navMeshAgent.isStopped = true; // หยุดการเคลื่อนที่
        navMeshAgent.velocity = Vector3.zero;
        anim.SetFloat("Speed", 0f);
        anim.SetTrigger("Dead");


        yield return new WaitForSeconds(anim.GetCurrentAnimatorStateInfo(0).length); // รอจนกว่าอนิเมชั่นตายจะจบ
        

    }

    private IEnumerator StopAndResume()
    {
        isStopped = true;
        navMeshAgent.isStopped = true; // หยุดการเคลื่อนที่
        yield return new WaitForSeconds(3f); // หยุดนิ่งเป็นเวลา 3 วินาที
        navMeshAgent.isStopped = false; // กลับมาเคลื่อนที่อีกครั้ง
        navMeshAgent.speed = slowSpeed; // ลดความเร็วเมื่อผู้เล่นย่อ
        isStopped = false;
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
        if (isDeadF) return; // ถ้าหมาตายแล้ว ไม่รับความเสียหายอีก

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


    protected override bool IsWeaponEffective(string weapon)
    {
        return weapon == "Weapon_Knife"; // Only effective with a knife
    }

}
