using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Chicken : Animal
{
    public GameObject chickenMeatPrefab;

    public float normalSpeed; // ความเร็วปกติของไก่
    public float slowSpeed; // ความเร็วปกติของไก่
    public float runAwaySpeed; // ความเร็วเมื่อวิ่งหนี

    private PlayerMove playerMove;
    public Chicken() : base("Chicken", 30, null) { }

    protected override void Start()
    {
        base.Start();
        DropItemPrefab = chickenMeatPrefab;// Assign the drop item
        // Example: Move to a specific position when spawned
        MoveTo(new Vector3(Random.Range(-5, 5), 0, Random.Range(-5, 5)));

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
    }

    protected override void Update()
    {
        base.Update();

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
                    navMeshAgent.speed = slowSpeed; // ลดความเร็วเมื่อผู้เล่นย่อ
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


    protected override bool IsWeaponEffective(string weapon)
    {
        return weapon == "Weapon_Knife"; // Only effective with a knife
    }

}
