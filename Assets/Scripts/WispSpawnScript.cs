using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WispSpawnScript : MonoBehaviour
{

    public Transform[] spawnpoints_amb1;
    public Transform[] spawnpoints_amb2;
    public Transform[] spawnpoints_amb3;

    public GameObject wisp;

    public float wispIntervalo = 5f;
    float wispTempoUltimoInvocado;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (wispIntervalo + wispTempoUltimoInvocado < Time.time)
        {
            switch (PlayerStats.ambiente_player)
            {
                case 1:
                    InvocarWisp(spawnpoints_amb1);
                    break;
                case 2:
                    InvocarWisp(spawnpoints_amb2);
                    break;
                case 3:
                    InvocarWisp(spawnpoints_amb3);
                    break;
            }

            wispTempoUltimoInvocado = Time.time;
        }
    }

    void InvocarWisp(Transform[] spawnPoints)
    {
        int indexSpawn = Random.Range(0, spawnPoints.Length);

        Instantiate(wisp, spawnPoints[indexSpawn].position, spawnPoints[indexSpawn].rotation);
        wisp.GetComponent<ExploderIA>().posicaoInicial = spawnPoints[indexSpawn];

    }




}
