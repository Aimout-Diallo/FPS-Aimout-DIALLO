using UnityEngine;

public class spawnenemy : MonoBehaviour
{
    public GameObject enemi;
    public Transform spawnPoint;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

        
        if (enemi != null && spawnPoint != null)
        {
            Instantiate(enemi, spawnPoint.position, Quaternion.identity);
        }


    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
