using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class ninja : MonoBehaviour
{
    public int mouseX = 10;
    public Vector3 direction = Vector3.forward;

    // Variables pour le saut et mouvement
    public int HP = 100;
    public int maxHP = 200;
    public float jumpForce = 5f;
    public float moveSpeed = 20f;
    public float airControl = 0.3f;
    public bool isGrounded = true;
    public Transform ninjaenemy;
    public GameObject projectilePrefab;
    public float projectileCooldown = 10f;
    public Transform projectileSpawnPoint;
    private float lastProjectileTime = 0f;
    public int damage = 40;
    public float attackrange = 2f;
    public bool justJumped = false;
    public TextMeshProUGUI perduText;
    public TextMeshProUGUI hpText;

    private Rigidbody rb;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
        justJumped = false;
        if (perduText != null)
        perduText.gameObject.SetActive(false);
    }

    void Update()
    {
        if (hpText != null)
        {
            hpText.text = "HP: " + HP;
        }
        if (ninjaenemy == null || !ninjaenemy.gameObject.activeInHierarchy)
        {
            GameObject enemyObj = GameObject.FindGameObjectWithTag("enemi");
            if (enemyObj != null)
            {
                ninjaenemy = enemyObj.transform;
                Debug.Log("Ennemi trouvé : " + ninjaenemy.name);
            }
        }

        Transform cam = Camera.main.transform;

        Vector3 forward = cam.forward;
        Vector3 right = cam.right;

        forward.y = 0;
        right.y = 0;
        forward.Normalize();
        right.Normalize();

        Vector3 movement = Vector3.zero;

        if (Input.GetKey(KeyCode.W)) movement += forward; // déplacement
        if (Input.GetKey(KeyCode.S)) movement += -forward;
        if (Input.GetKey(KeyCode.D)) movement += right;
        if (Input.GetKey(KeyCode.A)) movement += -right;

        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            AttackTarget(); //appelle l'attaque
        }

        if (Input.GetKeyDown(KeyCode.LeftControl))
        {
            ShootProjectile();
        }

        if (isGrounded)// si au sol
        {
            Vector3 newVelocity = movement * moveSpeed;
            newVelocity.y = rb.linearVelocity.y;
            rb.linearVelocity = newVelocity;
            justJumped = false;
        }
        else
        {
            rb.AddForce(movement * moveSpeed * airControl, ForceMode.Acceleration); //sensibilité du controle dans les airs
            justJumped = true;

            Vector3 horizontalVelocity = new Vector3(rb.linearVelocity.x, 0, rb.linearVelocity.z);
            if (horizontalVelocity.magnitude > moveSpeed)
            {
                horizontalVelocity = horizontalVelocity.normalized * moveSpeed;
                rb.linearVelocity = new Vector3(horizontalVelocity.x, rb.linearVelocity.y, horizontalVelocity.z);
            }
        }

        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            isGrounded = false;
        }
    }

    void OnCollisionEnter(Collision collision)  //savoir si on est au sol ou non
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

    public void Takedamagee(int damage)  //script dégat
    {
        HP -= damage;
        Debug.Log("HP: " + HP);

        if (HP <= 0)
        {
            StartCoroutine(GameOver());
        }
    }

    IEnumerator GameOver()
    {
        if (perduText != null)
        {
            perduText.gameObject.SetActive(true);
            perduText.text = "PERDU !";
        }

        yield return new WaitForSeconds(2f);

        UnityEngine.SceneManagement.SceneManager.LoadScene(
            UnityEngine.SceneManagement.SceneManager.GetActiveScene().name
        );
    }

    void AttackTarget()
    {
        // Cherche tous les ennemis dans la portée
        Collider[] hits = Physics.OverlapSphere(transform.position, attackrange);
        foreach (Collider hit in hits)
        {
            if (hit.CompareTag("enemi"))
            {
                enemy en = hit.GetComponent<enemy>();
                if (en != null)
                {
                    en.Takedamage(damage);
                }
            }
        }
    }


    void ShootProjectile()
    {
        if (projectilePrefab == null)
        {
            Debug.LogError("Prefab du projectile manquant !");
            return;
        }

        Vector3 spawnPos;

        if (projectileSpawnPoint != null)
        {
            spawnPos = projectileSpawnPoint.position;
        }
        else
        {
            spawnPos = transform.position + transform.forward * 1f + Vector3.up * 0.5f;
        }

        GameObject projectile = Instantiate(projectilePrefab, spawnPos, Quaternion.identity);

        // Direction vers le centre de l'écran (FPS)
        Ray ray = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));
        RaycastHit hit;

        Vector3 targetPoint;

        if (Physics.Raycast(ray, out hit))
        {
            targetPoint = hit.point;
        }
        else
        {
            targetPoint = ray.GetPoint(100f);
        }

        Vector3 shootDirection = (targetPoint - spawnPos).normalized;

        shuriken sh = projectile.GetComponent<shuriken>();
        if (sh != null)
        {
            sh.SetDirection(shootDirection);
        }
    }
}
