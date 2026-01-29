using UnityEngine;

public class enemy : MonoBehaviour
{
    [Header("Stats")]
    public int HP = 50;// Points de vie
    public int maxHP = 100;//point de vie max


    [Header("Déplacement")]
    public float Speed = 20f;//vitesse
    public float attackrange = 2f;//distance requise pour attaquer
    [Header("Détection")]
    public float detectionRange = 50f;
    private float lastAttackTime = 0f;
    private Transform currentTarget;
    public GameObject projectilePrefab;
    public float projectileCooldown = 10f;
    public Transform projectileSpawnPoint;
    private float lastProjectileTime = 0f;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void Takedamage(int damageAmount)
    {
        HP -= damageAmount;

        Debug.Log("HP du ninja: {HP}");

        if (HP <= 0)
        {
            Die(); //appeller la fonction a mourir
        }
    }


    void Die()
    {
        Debug.Log(" enenmy mort !");
        Destroy(gameObject);
    }
}
