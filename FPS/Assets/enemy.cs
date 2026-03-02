using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;

public class enemy : MonoBehaviour
{
    [Header("Stats")]
    public int HP = 100;
    public int maxHP = 100;

    [Header("Errance")]
    public float wanderRadius = 5f;
    public float wanderCooldown = 3f;
    private float lastWanderTime = 0f;
    private Vector3 wanderTarget;

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
    private Animator dégat;
    private string déégat = "dégat";
    private Animator punch;
    private string punnch = "punch";
    private bool hasJumped = false;
    void Start()
    {
        HP = maxHP;
        // Trouve le joueur au démarrage
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            player = playerObj.transform;
            dégat = GetComponent<Animator>();
            run = GetComponent<Animator>();
            idle = GetComponent<Animator>();
            punch= GetComponent<Animator>();
            rb = GetComponent<Rigidbody>();
            rb.freezeRotation = true;

        }
    }

    void Update()
    {
        // Si pas de joueur, cherche-le
        if (player == null)
        {
            
            
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
            ninja monninja = player.GetComponent<ninja>();
            if (monninja != null && !monninja.isGrounded && isGrounded && !hasJumped && monninja.justJumped)
            {
                Debug.Log("L'ennemi saute !");
                run.SetBool("saut", true);
                Debug.Log("isGrounded: " + isGrounded + " | hasJumped: " + hasJumped + " | ninja grounded: " + monninja.isGrounded);
                Vector3 jumpDirection = (player.position - transform.position).normalized;
                jumpDirection.y = 0;
                rb.linearVelocity = new Vector3(rb.linearVelocity.x, 0, rb.linearVelocity.z);
                rb.AddForce(Vector3.up * 10f, ForceMode.Impulse);
                isGrounded = false;
                hasJumped = true;
            }

            if (isGrounded) // Reset quand il retouche le sol
            {
                hasJumped = false;
                run.SetBool("saut", false);
                run.SetBool("IsRunning", true);
            }
            else if (hasJumped)
            {
                run.SetBool("saut",true);
                run.SetBool("IsRunning", false);
                hasJumped = true;
            }


            // Si trop loin, se déplacer vers le joueur
            if (distanceToPlayer > stoppingDistance)
            {
                MoveTowardsPlayer();
                run.SetBool("IsRunning", true);
                if (hasJumped) {
                    run.SetBool("saut", true);
                    run.SetBool("IsRunning", false);
                    hasJumped = true;


                }
                else
                {
                    run.SetBool("IsRunning", true);
                    run.SetBool("saut", false);
                    hasJumped = false;
                    

                }



            }
            else
            {
                // Arrête de bouger et regarde le joueur
                LookAtPlayer();
                run.SetBool("IsRunning", false);
                if (hasJumped)
                {
                    run.SetBool("saut", true);
                    run.SetBool("IsRunning", false);
                    hasJumped = true;
                }
                else 
                {
                    run.SetBool("saut",false);
                    hasJumped = false;

                }



                }

            // Attaque si assez proche et cooldown terminé
            if (distanceToPlayer <= attackRange && Time.time >= lastAttackTime + attackCooldown)
            {
                AttackPlayer();
                run.SetBool("saut",false);

                
                lastAttackTime = Time.time;
            }
            
        }
 
    }



    void OnCollisionEnter(Collision collision)
    {
        if (collision.contacts[0].normal.y > 0.5f)
        {
            StartCoroutine(ResetJump());
        }
    }

    IEnumerator ResetJump()
    {
        yield return new WaitForSeconds(0.2f);
        isGrounded = true;
        hasJumped = false;
    }

    void OnCollisionExit(Collision collision)
    {
        isGrounded = false;
        hasJumped = true;
    }

    void MoveTowardsPlayer()
    {
        
        // Calcule la direction vers le joueur
        Vector3 direction = (player.position - transform.position).normalized;

        // Garde seulement les axes X et Z (pas de vol)
        direction.y = 0;

        // Déplace l'ennemi
        rb.linearVelocity = new Vector3(direction.x * moveSpeed, rb.linearVelocity.y, direction.z * moveSpeed);

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
        ninja monninja = player.GetComponent<ninja>();
        if (monninja != null)
        {
            monninja.Takedamagee(damage);
            run.SetTrigger("Attack");
        }

    }

    public void Takedamage(int damage)
    {
        run.SetTrigger("Damage");
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