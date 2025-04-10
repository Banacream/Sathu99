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
    public float attackCooldown = 2.0f; // �������Ҥ�Ŵ�ǹ������ҧ�������

    private PlayerMove playerMove;
    private Color originalColor;

    public Animator anim;

    public List<SpriteRenderer> spriteRenderers;
    private bool isFlipped = false; // ʶҹС�ÿ�Ի
    private bool canAttack = true; // ʶҹС������
    private bool isDead = false; // ʶҹС�õ��
    public Snake() : base("Snake", 50, null) { }
    public AudioSource audioSource;

    protected override void Start()
    {
        base.Start();
        //DropItem = snakeMeatPrefab.name; // Assign the drop item

        // ��Ǩ�ͺ��е�駤�� NavMeshAgent
        float randomDelay = Random.Range(0f, 2f); // ���������� 0-2 �Թҷ�
        audioSource.PlayDelayed(randomDelay);
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
        if (isDead) return;
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
        if (isDead) return; // �����ҵ������ ����Ѻ������������ա

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

    private IEnumerator HandleDeath()
    {
        isDead = true; // ���ʶҹ������ҵ������
        navMeshAgent.isStopped = true; // ��ش�������͹���
        navMeshAgent.velocity = Vector3.zero;
        anim.SetFloat("Speed", 0f);
        anim.SetTrigger("Dead"); // ���͹�����蹵��

        yield return new WaitForSeconds(anim.GetCurrentAnimatorStateInfo(0).length); // �ͨ�����͹�����蹵�¨Ш�

        Destroy(gameObject); // ź GameObject ���
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
            // ��ش�������͹���
            navMeshAgent.isStopped = true;

            // ���͹�����蹡������
            anim.SetTrigger("Attack");

            // Ŵ��ѧ���Ե�ͧ������
            playerMove.TakeDamage(attackDamage);
            Debug.Log("Dog attacked the player!");

            // �������Ŵ�ǹ�������
            StartCoroutine(AttackCooldown());
        }
    }

    private IEnumerator AttackCooldown()
    {
        canAttack = false;
        yield return new WaitForSeconds(attackCooldown);
        canAttack = true;
        // ��Ѻ������͹����ա������ѧ�ҡ��Ŵ�ǹ��������
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
