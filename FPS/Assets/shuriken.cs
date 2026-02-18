using UnityEngine;

public class shuriken : MonoBehaviour
{
    [Header("Stats du Projectile")]
    public float speed = 20f; //propriété de la balle //vitesse, degat ,et sa distance max
    public int damage = 40;
    public float maxDistance = 20f;

    private Vector3 startPosition;
    private Vector3 direction;
    private float cooldoawn = 5f;
    private float nextplacetime = 0f;
    public float rotationSpeed = 1000f;

    void Start()
    {
        startPosition = transform.position;

    }

    void Update()
    {
        transform.Rotate(Vector3.forward * rotationSpeed * Time.deltaTime);

        transform.position += direction * speed * Time.deltaTime;

        float distanceTraveled = Vector3.Distance(startPosition, transform.position);
        if (distanceTraveled >= maxDistance)
        {
            Destroy(gameObject);
        }
    }

    public void SetDirection(Vector3 dir) //fonction appelant a donné la direction du shuriken
    {
        direction = dir.normalized;

        if (direction != Vector3.zero)
        {
            transform.rotation = Quaternion.LookRotation(direction)
                             * Quaternion.Euler(90f, 0f, 0f);
        }
    }



    void OnTriggerEnter(Collider other)
    {

        if (other.CompareTag("enemy"))// Vérifie si l'objet qui touche a le tag "ninja"
        {

            enemy ninja = other.GetComponent<enemy>();// Récupère les données du gobelin touché


            if (ninja != null) // Si le script existe bien sur le ninja 
            {


                ninja.Takedamage(damage); // Appelle la méthode Takedamage() du ninja pour lui infliger des dégâts


                Debug.Log("shuriken touche le ninja !");
            }


            Destroy(gameObject); // Détruit le projectile (shuriken) après avoir touché le gobelin
        }
    }
}
