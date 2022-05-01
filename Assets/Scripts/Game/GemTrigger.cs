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
        float x = Random.Range(-Config.UNITY_MAP_DIMENSION_X / 2, Config.UNITY_MAP_DIMENSION_X / 2);
        float y = Random.Range(-Config.UNITY_MAP_DIMENSION_Y / 2, Config.UNITY_MAP_DIMENSION_Y / 2);
        this.gameObject.transform.localPosition = new Vector3(x, -4.7f, y);
        this.gameObject.SetActive(true);
        spawnGem.Play();
    }

}
