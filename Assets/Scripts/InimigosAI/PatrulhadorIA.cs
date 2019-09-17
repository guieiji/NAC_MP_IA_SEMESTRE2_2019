using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PatrulhadorIA : MonoBehaviour
{
    private float raySize = 5f;
    private RaycastHit hit;

    private NavMeshAgent navAgent;

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

    private int indexWaypoint = 0;
    public Transform[] waypoints;
    private float distanciaPertoWaypoint = 4f;
    //public Transform destino;

    [Header("ESTADO:ATIRAR")]
    private float tempoAtirar = 4f;

    private float tempoComecouAtirar;
    private float velRot = 5f;
    public GameObject bala;
    public Transform pontaArma;
    private bool atirando;
    public float rateOfFire;

    [Header("ESTADO:RECARREGAR")]
    private float tempoRecarregar = 2f;

    private float tempoComecouRecarregar;

    private void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        estadoAtual = Estados.PATRULHAR;
        navAgent = GetComponent<NavMeshAgent>();
    }

    // Start is called before the first frame update
    private void Start()
    {
        alvo = waypoints[0];
    }

    // Update is called once per frame
    private void Update()
    {
        Debug.Log("Player no range? " + PlayerNoAlcance());
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
                    MudarWaypointIndex();
                    alvo = waypoints[indexWaypoint];
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
                        Debug.Log(hit.collider.tag);
                        if (hit.collider.tag == "Player")
                        {
                            //AtirarNoJogador();
                            if (!atirando)
                            {
                                StartCoroutine(AtirarContinuo());
                                atirando = true;
                            }
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
                    StopAllCoroutines();
                    alvo = transform;
                }

                break;
        }

        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(alvo.position - transform.position), Time.deltaTime * velRot);

        if (estadoAtual == Estados.ATIRAR)
        {
            navAgent.destination = transform.position;
        }
        else
        {
            navAgent.destination = alvo.position;
        }
    }

    #region PATRULHAR

    private void Patrulhar()
    {
        estadoAtual = Estados.PATRULHAR;
    }

    private void MudarWaypointIndex()
    {
        if (EstaPertoWaypoint())
        {
            indexWaypoint++;
            if (indexWaypoint == waypoints.Length)
            {
                indexWaypoint = 0;
            }
        }
    }

    private bool EstaPertoWaypoint()
    {
        return Vector3.Distance(transform.position, waypoints[indexWaypoint].position) < distanciaPertoWaypoint;
    }

    #endregion PATRULHAR

    #region ATIRAR

    private void Atirar()
    {
        estadoAtual = Estados.ATIRAR;
        tempoComecouAtirar = Time.time;
    }

    private bool AcabaramBalas()
    {
        return tempoAtirar + tempoComecouAtirar < Time.time;
    }

    private IEnumerator AtirarContinuo()
    {
        yield return new WaitForSeconds(rateOfFire);
        Instantiate(bala, pontaArma.position, pontaArma.rotation);
        StartCoroutine(AtirarContinuo());
    }

    private bool PlayerNoAlcance()
    {
        return Vector3.Distance(transform.position, player.position) < distanciaPatrulhar;
    }

    private void AtirarNoJogador()
    {
        Instantiate(bala, pontaArma.position, pontaArma.rotation);
    }

    #endregion ATIRAR

    #region RECARREGAR

    private void Recarregar()
    {
        estadoAtual = Estados.RECARREGAR;
        tempoComecouRecarregar = Time.time;
        atirando = false;
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