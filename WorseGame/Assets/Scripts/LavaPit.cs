using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LavaPit : MonoBehaviour
{
    private GameObject player;
    // Start is called before the first frame update
    void Start()
    {
        GameManager.OnGameStart += FindPlayer;
    }
    private void LateUpdate()
    {
        if(player != null)
            transform.position = new Vector3(player.transform.position.x, transform.position.y, transform.position.z);
    }
    void FindPlayer()
    {
        player = GameObject.FindGameObjectWithTag("Player");
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            other.GetComponentInChildren<Player>().Damage(true, true);
        }
    }
}
