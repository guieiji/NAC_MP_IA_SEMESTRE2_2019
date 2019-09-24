using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PatrulhadorAi2 : MonoBehaviour
{
    private float raySize = 5f;
    private RaycastHit hit;

    private NavMeshAgent navAgent;

    public enum Estados
    {
        ESPERAR,

        PATRULHAR,

        PERSEGUIR,

        ATACAR
    }

    private Estados estadoAtual;
    private Transform alvo;
    private Transform player;
    private Animator anim;


    [Header("Estados: ESPERAR")]
    float tempoEsperar;
    float tempoEsperando = 2f;


    [Header("ESTADO:PATRULHAR")]
    private float distanciaPatrulhar = 10f;
    float distanciaAlvo;

    private int indexWaypoint = 0;
    public Transform[] waypoints;
    private float distanciaPertoWaypoint = 3f;
    //public Transform destino;


    [Header("ESTADO:PERSEGUIR")]

    float distanciaPerseguir = 12f;
    private float velMaxPerseguir = 4f;
    private float aceleracaoPerseguir = 10f;

    [Header("ESTADO:ATAQUE")]

    float distanciaAtaque = 3f;
    private bool atacando;

    private void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        estadoAtual = Estados.PATRULHAR;
        navAgent = GetComponent<NavMeshAgent>();
        anim = GetComponent<Animator>();
    }

    // Start is called before the first frame update
    private void Start()
    {
        alvo = waypoints[0];
        atacando = false;
    }

    // Update is called once per frame
    private void Update()
    {
        distanciaAlvo = Vector3.Distance(transform.position, alvo.position);

        Debug.DrawRay(transform.position, transform.forward * raySize, Color.red, 0.5f);


        switch (estadoAtual)
        {
            case Estados.ESPERAR:
                if (EsperouSuficiente())
                {
                    Patrulhar();
                }
                else
                {
                    alvo = transform;
                }
                break;

            case Estados.PATRULHAR:
                if (PlayerPerto())
                {
                    Perseguir();
                }
                else
                {
                    
                    anim.Play("Walk");
                    MudarWaypointIndex();
                    alvo = waypoints[indexWaypoint];
                }
                break;


            case Estados.PERSEGUIR:
                if (PlayerForaArea())
                {
                    Esperar();
                }
                else if (PlayerNaAreaDeAtaque())
                {
                    Atacar();
                }
                else
                {
                    
                    alvo = player;
                }

                break;

            case Estados.ATACAR:
                if (!atacando)
                {
                    IniciarAtaque();
                }

                alvo = transform;
                break;
        }

         navAgent.destination = alvo.position;
        
    }





    #region ESPERAR

    void Esperar()
    {
        estadoAtual = Estados.ESPERAR;
        tempoEsperar = Time.time;
        atacando = false;
        StopAllCoroutines();
        anim.Play("Idle");
        ResetarNavAgent();
    }

    private bool EsperouSuficiente()
    {
        return tempoEsperar + tempoEsperando < Time.time;
    }

    void ResetarNavAgent()
    {
        navAgent.acceleration = 8;
        navAgent.speed = 3.5f;
    }

    #endregion ESPERAR

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
    private bool PlayerPerto()
    {
        return Vector3.Distance(player.position, transform.position) < distanciaPatrulhar;
    }

    private bool EstaPertoWaypoint()
    {
        return Vector3.Distance(transform.position, waypoints[indexWaypoint].position) < distanciaPertoWaypoint;
    }

    #endregion PATRULHAR

   

    #region PERSEGUIR

    private void Perseguir()
    {
        estadoAtual = Estados.PERSEGUIR;
        anim.Play("Run");


        navAgent.acceleration = aceleracaoPerseguir;
        navAgent.speed = velMaxPerseguir;

    }



    private bool PlayerForaArea()
    {
        return Vector3.Distance(player.position, transform.position) > distanciaPerseguir;
    }
    #endregion PERSEGUIR


    #region ATACAR
    void Atacar()
    {
        estadoAtual = Estados.ATACAR;
    }

    private bool PlayerNaAreaDeAtaque()
    {
        return Vector3.Distance(player.position, transform.position) < distanciaAtaque;
    }

    private void IniciarAtaque()
    {
        atacando = true;
        StartCoroutine(AtacandoPlayer());
    }

    IEnumerator AtacandoPlayer()
    {
        anim.Play("Attack");
        yield return new WaitForSeconds(1.5f);
        if(Vector3.Distance(player.position,transform.position) < distanciaAtaque)
        {
            PlayerStats.playerVida--;
        }
        Esperar();
    }

    #endregion ATACAR

}
