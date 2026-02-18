using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class enemy : MonoBehaviour
{
    [Header("Stats")]
    public int HP = 100;
    public int maxHP = 100;

    [Header("Déplacement")]
    public float moveSpeed = 3f;
    public float stoppingDistance = 2f; // Distance à laquelle il s'arrête

    [Header("Combat")]
    public float attackRange = 2.5f;
    public int damage = 15;
    public float attackCooldown = 1.5f;
    private float lastAttackTime = 0f;

    [Header("Détection")]
    public float detectionRange = 15f;

    private Transform player;
    private bool hasDetectedPlayer = false;

    private Animator run;
    private string runn = "mixamo_com";
    private Animator idle;
    private string idlle = "idle";
    private bool isGrounded = true;
    private Rigidbody rb;
    public float jumpForce = 5f;
    public float airControl = 0.3f;
    void Start()
    {
        HP = maxHP;
        // Trouve le joueur au démarrage
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            player = playerObj.transform;
            run = GetComponent<Animator>();
            idle = GetComponent<Animator>();
            idle.Play(idlle);
            rb = GetComponent<Rigidbody>();
            rb.freezeRotation = true;
        }
    }

    void Update()
    {
        // Si pas de joueur, cherche-le
        if (player == null)
        {
            idle.Play(idlle);
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null)
            {
                player = playerObj.transform;
            }
            return;
        }

        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        // Détection du joueur
        if (distanceToPlayer <= detectionRange)
        {
            hasDetectedPlayer = true;

        }

        if (hasDetectedPlayer)
        {

            // Si trop loin, se déplacer vers le joueur
            if (distanceToPlayer > stoppingDistance)
            {
                MoveTowardsPlayer();
                run.SetBool("isRunning", true);
                run.Play(runn);


            }
            else
            {
                // Arrête de bouger et regarde le joueur
                LookAtPlayer();
                run.SetBool("isRunning", false);


            }

            // Attaque si assez proche et cooldown terminé
            if (distanceToPlayer <= attackRange && Time.time >= lastAttackTime + attackCooldown)
            {
                AttackPlayer();
                lastAttackTime = Time.time;
            }
        }
 
    }

    

    void OnCollisionEnter(Collision collision)
    {
        isGrounded = true;
    }

    void OnCollisionStay(Collision collision)
    {
        isGrounded = true;
    }

    void OnCollisionExit(Collision collision)
    {
        isGrounded = false;
    }


    void MoveTowardsPlayer()
    {
        
        // Calcule la direction vers le joueur
        Vector3 direction = (player.position - transform.position).normalized;

        // Garde seulement les axes X et Z (pas de vol)
        direction.y = 0;

        // Déplace l'ennemi
        transform.position += direction * moveSpeed * Time.deltaTime;

        // Regarde vers le joueur
        LookAtPlayer();
        
    }

    void LookAtPlayer()
    {
        // Fait tourner l'ennemi vers le joueur
        Vector3 direction = (player.position - transform.position).normalized;
        direction.y = 0; // Garde la rotation horizontale seulement

        if (direction != Vector3.zero)
        {
            Quaternion lookRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 5f);
        }
    }

    void AttackPlayer()
    {
        Debug.Log("L'ennemi attaque le joueur !");

        // Inflige des dégâts au joueur
        ninja ninja = GetComponent<ninja>();
        if (ninja != null)
        {
            ninja.Takedamagee(damage);
        }
    }

    public void Takedamage(int damage)
    {
        HP -= damage;
        Debug.Log($"HP de l'ennemi: {HP}");

        if (HP <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        Debug.Log("Ennemi mort !");
        Destroy(gameObject);
    }
}