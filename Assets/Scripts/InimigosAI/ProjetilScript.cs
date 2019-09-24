using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjetilScript : MonoBehaviour
{
    // Start is called before the first frame update
    private void Start()
    {
        Destroy(gameObject, 4f);
    }

    // Update is called once per frame
    private void Update()
    {
        transform.Translate(Vector3.forward * Time.deltaTime * (1 + PlayerStats.forcaLuz));
    }

    private void OnCollisionEnter(Collision collision)
    {
        Destroy(gameObject);
    }
}