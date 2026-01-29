using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class ninja : MonoBehaviour
{
    public int mouseX = 10;
    public Vector3 direction = Vector3.forward;

    // Variables pour le saut et mouvement
    public int HP = 100;
    public float jumpForce = 5f;
    public float moveSpeed = 20f;
    public float airControl = 0.3f; // Contr�le réduit en l'air
    private bool isGrounded = true;
    public GameObject ninjaenemy;
    public GameObject projectilePrefab;//pour les shurikens
    public float projectileCooldown = 10f;
    public Transform projectileSpawnPoint;
    private float lastProjectileTime = 0f;
    public int damage = 40;

    private Rigidbody rb;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
    }

    void Update()
    {
        // Recupere la cam�ra principale
        Transform cam = Camera.main.transform;

        // R�cup�re les directions de la camera
        Vector3 forward = cam.forward;
        Vector3 right = cam.right;

        // Supprime la composante Y pour rester au sol
        forward.y = 0;
        right.y = 0;
        forward.Normalize();
        right.Normalize();

        // Calcule le mouvement
        Vector3 movement = Vector3.zero;

        if (Input.GetKey(KeyCode.W))
        {
            movement += forward;
        }
        if (Input.GetKey(KeyCode.S))
        {
            movement += -forward;
        }
        if (Input.GetKey(KeyCode.D))
        {
            movement += right;
        }
        if (Input.GetKey(KeyCode.A))
        {
            movement += -right;
        }
        if (Input.GetKey(KeyCode.Mouse0))
        {
            AttackTarget();
        }

        // Applique le mouvement differemment selon si on est au sol ou en l'air
        if (isGrounded)
        {
            // Au sol : controle total
            Vector3 newVelocity = movement * moveSpeed;
            newVelocity.y = rb.linearVelocity.y;
            rb.linearVelocity = newVelocity;
        }
        else
        {
            // En l'air : controle réduit (on ajoute une petite force)
            rb.AddForce(movement * moveSpeed * airControl, ForceMode.Acceleration);

            // Limite la vitesse horizontale en l'air
            Vector3 horizontalVelocity = new Vector3(rb.linearVelocity.x, 0, rb.linearVelocity.z);
            if (horizontalVelocity.magnitude > moveSpeed)
            {
                horizontalVelocity = horizontalVelocity.normalized * moveSpeed;
                rb.linearVelocity = new Vector3(horizontalVelocity.x, rb.linearVelocity.y, horizontalVelocity.z);
            }
        }

        // SAUT
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            isGrounded = false;
        }
    }

    // Détecte si on touche le sol
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
    void AttackTarget()
    {

        if (ninjaenemy == null) return;

        // ATTAQUE LE NINJA 
        if (ninjaenemy.CompareTag("enemi"))
        {
            enemy ninja = ninjaenemy.GetComponent<enemy>();
            if (ninja != null)
            {
                Debug.Log("ninja attaqué ");
                ninja.Takedamage(damage);
            }
        }
    }
    void ShootProjectile() //fonction pour le projectile
    {
        if (projectilePrefab == null) //si les données du projectile sont pas présents
        {
            Debug.LogError("Prefab du projectile manquant !");
            return;
        }

        Vector3 spawnPos; //apparition
        if (projectileSpawnPoint != null)
        {
            spawnPos = projectileSpawnPoint.position;
        }
        else
        {
            spawnPos = transform.position + transform.forward * 1f + Vector3.up * 0.5f;
        }

        GameObject projectile = Instantiate(projectilePrefab, spawnPos, Quaternion.identity);

        Vector3 shootDirection;
        if (currentTarget != null)
        {
            shootDirection = (currentTarget.position - spawnPos).normalized;
        }
        else
        {
            shootDirection = transform.forward;
        }

        shuriken projectileScript = projectile.GetComponent<shuriken>();
        if (projectileScript != null)
        {
            projectileScript.SetDirection(shootDirection);
        }

        Debug.Log("Projectile lance !");
    }
}
