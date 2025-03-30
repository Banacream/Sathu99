using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public abstract class AnimalEnemy : MonoBehaviour
{
    public string Name { get; set; }  // Name of the animal
    public int Health { get; set; }  // Health of the animal
    public string DropItem { get; set; } // Item to drop upon death

    protected NavMeshAgent navMeshAgent;
    protected Transform playerTransform; // Reference to the player's transform
    public float ChaseDistance = 10.0f; // Distance at which the animal starts chasing the player
    public float AttackDistance = 2.0f; // Distance at which the animal attacks the player
    public float RandomMoveRadius = 10.0f; // Radius for random movement
    public float RandomMoveInterval = 3.0f; // Time interval for random movement

    private float randomMoveTimer = 0f; // Timer for random movement

    // Constructor to initialize common properties
    public AnimalEnemy(string name, int health, string dropItem)
    {
        Name = name;
        Health = health;
        DropItem = dropItem;
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
        if (navMeshAgent != null)
        {
            navMeshAgent.SetDestination(targetPosition);
        }
    }

    // Method to take damage from a weapon
    public virtual void TakeDamage(HandleSlot weaponSlot)
    {

        if (weaponSlot.item != null && weaponSlot.item.itemType == DataItem.ItemType.Tool)
        {
            DataItem weapon = weaponSlot.item;
            Health -= (int)weapon.damage;
            Debug.Log($"{Name} took {weapon.damage} damage. Health remaining: {Health}");

        }

        //if (IsWeaponEffective(weapon))
        //{
        //    Health -= 10; // Example damage value
        //    if (Health <= 0)
        //    {
        //        Drop();
        //    }
        //}
        //else
        //{
        //    Debug.Log($"{weapon} is not effective against {Name}.");
        //}
    }

    // Method to check weapon type against animal type
    public bool CheckWeaponAgainstAnimal(string weaponType)
    {
        return IsWeaponEffective(weaponType);
    }

    // Abstract method to check weapon effectiveness
    protected abstract bool IsWeaponEffective(string weapon);

    // Drop item upon death
    private void Drop()
    {
        Debug.Log($"{Name} has died and dropped: {DropItem}.");
    }

    // Check distance from the player and chase or attack if too close
    protected virtual void Update()
    {
        randomMoveTimer += Time.deltaTime;
        if (Health > 0)
        {
            if (playerTransform != null)
            {
                float distance = Vector3.Distance(transform.position, playerTransform.position);
                // Debug.Log($"Distance to player: {distance}");

                if (distance < AttackDistance)
                {
                    AttackPlayer();
                }
                else if (distance < ChaseDistance)
                {
                    ChasePlayer();
                }
                else if (randomMoveTimer >= RandomMoveInterval)
                {
                    RandomMove();
                    randomMoveTimer = 0f;
                }
            }
            else if (randomMoveTimer >= RandomMoveInterval)
            {
                RandomMove();
                randomMoveTimer = 0f;
            }
        }
    }

    // Method to chase the player
    protected void ChasePlayer()
    {
        if (playerTransform != null)
        {
            MoveTo(playerTransform.position);
        }
    }

    // Method to attack the player
    protected abstract void AttackPlayer();

    // Random movement within a radius
    private void RandomMove()
    {
        Vector3 randomDirection = Random.insideUnitSphere * RandomMoveRadius;
        randomDirection += transform.position;
        NavMeshHit hit;
        if (NavMesh.SamplePosition(randomDirection, out hit, RandomMoveRadius, 1))
        {
            MoveTo(hit.position);
        }
    }
}
