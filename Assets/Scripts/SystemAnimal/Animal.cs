using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public abstract class Animal : MonoBehaviour
{

    public string Name { get; set; }  // Name of the animal
    public int Health { get; set; }  // Health of the animal
    public GameObject DropItemPrefab { get; set; }

    protected NavMeshAgent navMeshAgent;
    protected Transform playerTransform; // Reference to the player's transform
    public float EnemyDistanceRun = 5.0f; // Distance at which the animal starts running
    public float RandomMoveRadius = 50.0f; // Radius for random movement
    public float RandomMoveInterval = 1.0f; // Time interval for random movement

    private float randomMoveTimer = 0f; // Timer for random movement
    private bool isMovingToTarget = false;
    private bool isDead = false; // Status for dead

    // Constructor to initialize common properties
    public Animal(string name, int health, GameObject dropItemPrefab)
    {
        Name = name;
        Health = health;
        DropItemPrefab = dropItemPrefab;
    }

    // Unity Start method
    protected virtual void Start()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
        if (navMeshAgent == null)
        {
            Debug.LogError($"NavMeshAgent not found on {Name}.");
        }

        // Find the player in the scene
        GameObject player = GameObject.FindWithTag("Player");
        if (player != null)
        {
            playerTransform = player.transform;
        }
        else
        {
            Debug.LogError("Player not found in the scene.");
        }
    }

    // Method to move the animal to a target position
    public void MoveTo(Vector3 targetPosition)
    {
        if (navMeshAgent != null && !isDead)
        {
            navMeshAgent.SetDestination(targetPosition);
            isMovingToTarget = true;
            StartCoroutine(SlowDownWhenClose(targetPosition));
          
        }
    }

    // Coroutine to gradually slow down when close to the target position
    private IEnumerator SlowDownWhenClose(Vector3 targetPosition)
    {
        while (navMeshAgent != null && navMeshAgent.remainingDistance > 0.1f)
        {
            if (!navMeshAgent.isOnNavMesh || !navMeshAgent.isActiveAndEnabled || isDead)
            {
                yield break; // Exit the coroutine if the agent is not on a NavMesh, not active, or dead
            }

            float distance = Vector3.Distance(transform.position, targetPosition);
            if (distance < 2.0f) // Adjust this value as needed
            {
                navMeshAgent.speed = Mathf.Lerp(navMeshAgent.speed, 0, Time.deltaTime);
            }
            yield return null;
        }
        navMeshAgent.speed = navMeshAgent.speed; // Reset speed after reaching the target
        isMovingToTarget = false; // Update status after reaching the target
    }

    // Method to take damage from a weapon

    public void TakeDamage(HandleSlot handleSlot)
    {
        if (handleSlot == null || handleSlot.item == null && !isDead)
        {
            Debug.Log("No weapon equipped in the handle slot.");
            return;
        }

        string equippedWeapon = handleSlot.item.name; // Get weapon name from the HandleSlot
        if (IsWeaponEffective(equippedWeapon))
        {
            Health -= 10; // Example damage value
            if (Health <= 0)
            {
                Die();

            }
        }
        else
        {
            Debug.Log($"{equippedWeapon} is not effective against {Name}.");
        }
    }
    //public void TakeDamage(string weapon)
    //{
    //    if (IsWeaponEffective(weapon))
    //    {
    //        Health -= 10; // Example damage value
    //        if (Health <= 0)
    //        {
    //            Drop();
    //        }
    //    }
    //    else
    //    {
    //        Debug.Log($"{weapon} is not effective against {Name}.");
    //    }
    //}


    // Abstract method to check weapon effectiveness
    protected abstract bool IsWeaponEffective(string weapon);

    // Drop item upon death
    private void Drop()
    {
        Debug.Log($"{Name} has died and dropped an item.");
        SpawnItem.Instance.SpawnItemAtPosition(DropItemPrefab, transform.position); // Spawn item at the animal's position
        Destroy(gameObject); // Remove the animal from the scene
    }

    // Check distance from the player and run away if too close
    protected virtual void Update()
    {
        if (isDead) return;

        randomMoveTimer += Time.deltaTime;

        if (playerTransform != null)
        {
            float distance = Vector3.Distance(transform.position, playerTransform.position);
            // Debug.Log($"Distance to player: {distance}");

            if (distance < EnemyDistanceRun)
            {

                isMovingToTarget = true;
                Vector3 dirToPlayer = transform.position - playerTransform.position;
                Vector3 newPos = transform.position + dirToPlayer.normalized * 5.0f; // Move slightly further away
                MoveTo(newPos);
            }
            
            if (!isMovingToTarget && randomMoveTimer >= RandomMoveInterval)
            {
                RandomMove();
                randomMoveTimer = 0f;
                RandomMoveInterval = Random.Range(1.0f, 3.0f);
            }

            //else if (randomMoveTimer >= RandomMoveInterval)
            //{

            //    RandomMove();
            //    randomMoveTimer = 0f;
            //}
        }
        else if (randomMoveTimer >= RandomMoveInterval)
        {
  
            RandomMove();
            randomMoveTimer = 0f;
        }
    }

    private void Die()
    {
        isDead = true;
        navMeshAgent.isStopped = true; // Stop the NavMeshAgent
        Drop();
        // Add any additional logic for when the animal dies, such as playing a death animation
    }

    // Random movement within a radius
    private void RandomMove()
    {

        if (isDead) return;

        Vector3 randomDirection = Random.insideUnitSphere * RandomMoveRadius;
        randomDirection += transform.position;
        NavMeshHit hit;
        if (NavMesh.SamplePosition(randomDirection, out hit, RandomMoveRadius, 1))
        {
            MoveTo(hit.position);
        }
    }
}



