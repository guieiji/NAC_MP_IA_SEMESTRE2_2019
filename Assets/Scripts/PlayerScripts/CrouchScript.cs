using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrouchScript : MonoBehaviour
{
    private CharacterController controller;
    bool agachado;
    public float impulso = 5f;

    // Start is called before the first frame update
    void Start()
    {
        controller = GetComponent<CharacterController>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.C))
        {
            if (!agachado)
            {
                controller.height = 0.5f;
                agachado = true;
            }
            else if (agachado)
            {
                controller.height = 1.8f;
                agachado = false;
            }
        }





    }


    void OnTriggerEnter(Collider col)
    {
        if (col.CompareTag("Teleport"))
        {
            transform.position = col.GetComponent<TeleportScript>().destino.position;
            Destroy(gameObject);
        }
    }
}
