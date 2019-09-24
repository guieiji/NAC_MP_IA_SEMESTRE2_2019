using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeleportScript : MonoBehaviour
{

    public Transform destino;
    public GameObject player;



    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnTriggerEnter(Collider col)
    {
        if (col.CompareTag("Player"))
        {
            col.GetComponent<CharacterController>().enabled = false;
            col.transform.position = destino.position;
            col.GetComponent<CharacterController>().enabled = true;
            //Instantiate(player, destino.position, destino.rotation);
        }
    }

}
