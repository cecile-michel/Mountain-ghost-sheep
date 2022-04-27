using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GemTrigger : MonoBehaviour
{

    public AudioSource pickupGem;
    public AudioSource spawnGem;
    public int spawnInterval;

    // Start is called before the first frame update
    void Start()
    {
        respawn();
    }


    // Update is called once per frame
    void Update()
    {

    }
    void OnTriggerEnter(Collider other)
    {
        if (other.transform.parent.gameObject.CompareTag("Player"))
        {
            other.transform.GetComponentInParent<PowerUpHandler>().addPowerUp("steal");
            pickupGem.Play();
            this.gameObject.SetActive(false);
            Invoke("respawn", spawnInterval);
        }
    }

    void respawn()
    {
        this.gameObject.transform.SetPositionAndRotation(new Vector3(Random.Range(-11, 15), -4.7f, Random.Range(-7.5f, 11)), this.transform.rotation);
        this.gameObject.SetActive(true);
        spawnGem.Play();
    }

}
