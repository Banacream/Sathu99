using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Dog : AnimalEnemy
{
    public GameObject dogMeatPrefab;

    public float normalSpeed; // ความเร็วปกติของสุนัข
    public float chaseSpeed; // ความเร็วเมื่อไล่ผู้เล่น
    public float attackDamage = 10.0f; // ความเสียหายเมื่อโจมตีผู้เล่น

    private PlayerMove playerMove;

    public Dog() : base("Dog", 50, null) { }

    protected override void Start()
    {
        base.Start();
        DropItem = dogMeatPrefab.name; // Assign the drop item

        // ตรวจสอบและตั้งค่า NavMeshAgent
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
            if (distanceToPlayer < ChaseDistance)
            {
                navMeshAgent.speed = chaseSpeed; // เพิ่มความเร็วเมื่อไล่ผู้เล่น
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

    protected override void AttackPlayer()
    {
        if (playerMove != null)
        {
            // Example: Reduce player's health
            playerMove.TakeDamage(attackDamage);
            Debug.Log("Dog attacked the player!");
        }
    }

    protected override bool IsWeaponEffective(string weapon)
    {
        return weapon == "001_Stick"; // Only effective with a stick
    }
}
