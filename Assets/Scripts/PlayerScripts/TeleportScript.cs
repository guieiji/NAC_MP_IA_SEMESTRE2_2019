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

            switch (this.name)
            {
                case "ExitTrigger 1":
                    PlayerStats.ambiente_player = 2;
                    break;
                case "ExitTrigger 2":
                    PlayerStats.ambiente_player = 3;
                    break;
                case "ExitTrigger 3":
                    PlayerStats.ambiente_player = 4;
                    break;
            }


            //Instantiate(player, destino.position, destino.rotation);
        }
    }

}
