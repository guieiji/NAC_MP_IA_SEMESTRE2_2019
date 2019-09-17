using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PerseguidorIA : MonoBehaviour
{
    private float raySize = 5f;
    private RaycastHit hit;

    public enum Estados
    {
        PATRULHAR,

        ATIRAR,

        RECARREGAR
    }

    private Estados estadoAtual;
    private Transform alvo;
    private Transform player;

    [Header("ESTADO:PATRULHAR")]
    private float distanciaPatrulhar = 4f;

    [Header("ESTADO:ATIRAR")]
    private float tempoAtirar = 2f;
    private float tempoComecouAtirar;
    private float velRot;
    public GameObject bala;
    public Transform pontaArma;

    [Header("ESTADO:RECARREGAR")]
    private float tempoRecarregar = 2f;
    private float tempoComecouRecarregar;

    private void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        estadoAtual = Estados.PATRULHAR;
    }

    // Start is called before the first frame update
    private void Start()
    {
    }

    // Update is called once per frame
    private void Update()
    {
        Debug.DrawRay(transform.position, transform.forward * raySize, Color.red, 0.5f);
        switch (estadoAtual)
        {
            case Estados.PATRULHAR:
                if (PlayerNoAlcance())
                {
                    Atirar();
                }
                else
                {
                    alvo = transform;
                }
                break;

            case Estados.ATIRAR:
                if (AcabaramBalas())
                {
                    Recarregar();
                }
                else
                {
                    alvo = player;
                    if (Physics.Raycast(transform.position, transform.forward, out hit, distanciaPatrulhar))
                    {
                        if (hit.collider.tag == "Player")
                        {
                            AtirarNoJogador();
                        }
                    }
                }
                break;

            case Estados.RECARREGAR:
                if (RecarregouTempoSuficiente())
                {
                    Patrulhar();
                }
                else
                {
                    alvo = transform;
                }

                break;
        }

        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(alvo.position - transform.position), Time.deltaTime * velRot);
    }

    #region PATRULHAR

    private void Patrulhar()
    {
        estadoAtual = Estados.PATRULHAR;
    }

    #endregion PATRULHAR

    #region ATIRAR

    private void Atirar()
    {
        estadoAtual = Estados.ATIRAR;
    }

    private bool AcabaramBalas()
    {
        return tempoAtirar + tempoComecouAtirar < Time.time;
    }

    private bool PlayerNoAlcance()
    {
        return Vector3.Distance(transform.position, player.position) < distanciaPatrulhar;
    }

    private void AtirarNoJogador()
    {
        Instantiate(bala, pontaArma);
    }

    #endregion ATIRAR

    #region RECARREGAR

    private void Recarregar()
    {
        estadoAtual = Estados.RECARREGAR;
        tempoComecouRecarregar = Time.time;
    }

    private void RecarregouBalas()
    {
        estadoAtual = Estados.PATRULHAR;
    }

    private bool RecarregouTempoSuficiente()
    {
        return tempoRecarregar + tempoComecouRecarregar < Time.time;
    }

    #endregion RECARREGAR
}