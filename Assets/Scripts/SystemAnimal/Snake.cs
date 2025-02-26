using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Snake : AnimalEnemy
{
    public GameObject snakeMeatPrefab;

    public float normalSpeed; // �������ǻ��Ԣͧ�عѢ
    public float chaseSpeed; // ���������������������
    public float attackDamage = 10.0f; // �������������������ռ�����

    private PlayerMove playerMove;
    private Color originalColor;

    public Animator anim;
    public List<SpriteRenderer> spriteRenderers;
    private bool isFlipped = false; // ʶҹС�ÿ�Ի
    public Snake() : base("Snake", 50, null) { }

    protected override void Start()
    {
        base.Start();
        DropItem = snakeMeatPrefab.name; // Assign the drop item

        // ��Ǩ�ͺ��е�駤�� NavMeshAgent
        navMeshAgent = GetComponent<NavMeshAgent>();
        if (navMeshAgent != null)
        {
            navMeshAgent.speed = normalSpeed; // ��駤�Ҥ��������������
        }
        else
        {
            Debug.LogError("NavMeshAgent not found on Dog.");
        }

        // �� PlayerMove component
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
        // �� PlayerMove component �ҡ�ѧ��辺
        if (playerMove == null)
        {
            FindPlayerMove();
        }

        if (playerMove != null && navMeshAgent != null)
        {
            float distanceToPlayer = Vector3.Distance(transform.position, playerMove.transform.position);
            if (distanceToPlayer < ChaseDistance)
            {
                navMeshAgent.speed = chaseSpeed; // �������������������������
            }
            else
            {
                navMeshAgent.speed = normalSpeed; // �������ǻ�������ͼ�����������
            }

            // ��Ի��ҹ�����ȷҧ�������͹���
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
            sr.color = Color.red; // ����¹������ᴧ
        }
        yield return new WaitForSeconds(0.2f); // �� 0.2 �Թҷ�
        foreach (SpriteRenderer sr in spriteRenderers)
        {
            sr.color = originalColor; // ����¹�ա�Ѻ�������
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
