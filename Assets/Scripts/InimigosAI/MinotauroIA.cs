using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class MinotauroIA : MonoBehaviour
{
    public enum Estados
    {
        Esperar,
        Patrulhar,
        Perseguir,
        Atropelar,
        Atordoado,
        Atacar
    }

    private int MinotauroHP;
    private Estados estadoAtual;

    private Transform alvo;

    public Transform player;

    private NavMeshAgent navMeshAgent;
    public Animator anim;
    private float raySize = 3f;
    private RaycastHit hit;

    private float aceleracaoBase;
    private float velMaxBase;
    private float velAngularBase;
    public float velRot = 5f;

    float distanciaPlayer;
    // Estado: Esperar
    [Header("Estados:Esperar")]
    public float tempoEsperar = 2f;

    private float tempoEsperando = 0f;
    private float distanciaDetectar = 25f;

    [Header("Estado:Patrulhar")]
    public Transform[] waypoints;

    public Transform waypointAtual;
    private int waypointIndex;
    public float distanciaMinimaWaypoint = 1f;

    [Header("Estado: Perseguir")]
    private float distanciaVisao = 20f;

    [Header("Estado: Atropelar")]
    private float aceleracaoAtropelar = 16f;

    private float velMaxAtropelar = 20f;
    private float velAngularAtropelar = 0f;

    public Transform alvoAtropelar;
    private float distanciaColisao = 5f;
    private int danoAtropelar = 2;

    [Header("Estado: Atordoado")]
    private float tempoAtordoado = 5f;

    private float tempoInicioAtordoado;
    private float tempoInicioAtropelar;
    private float tempoAtropelarJogador = 2f;
    private Quaternion salvaRot;

    [Header("Estado: Atacar")]
    float tempoInicioAtaque;
    float tempoAtacando = 2f;
    float alcanceAtaque = 3f;
    private float distanciaAtaque = 8f;

    private void Awake()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
        player = GameObject.FindGameObjectWithTag("Player").transform;
        
    }

    // Start is called before the first frame update
    private void Start()
    {
        MinotauroHP = 3;
        aceleracaoBase = navMeshAgent.acceleration;
        velMaxBase = navMeshAgent.speed;
        velAngularBase = navMeshAgent.angularSpeed;
        waypointAtual = waypoints[0];
        Esperar();
    }

    // Update is called once per frame
    private void Update()
    {
        distanciaPlayer = Vector3.Distance(transform.position, player.position);
        Debug.DrawRay(transform.position, transform.forward * raySize, Color.red, 0.5f);

        if(MinotauroHP <= 0)
        {
            Destroy(gameObject);
        }

        ChecarEstados();
    }

    private void ChecarEstados()
    {

        if (estadoAtual != Estados.Atordoado && estadoAtual != Estados.Atropelar && estadoAtual != Estados.Perseguir && estadoAtual != Estados.Atacar && DetectouJogador())
        {
            Perseguir();
        }

        switch (estadoAtual)
        {
            case Estados.Esperar:
                if (EsperouSuficiente())
                {
                    
                    Patrulhar();
                }

                alvo = transform;

                break;

            case Estados.Perseguir:
                Debug.Log(VendoJogador());
                if (VendoJogador())
                {
                    Atropelar();
                }
                else if (!DetectouJogador())
                {
                    Patrulhar();
                }
                else if (PlayerNoAlcanceAtaque())
                {
                    Atacar();
                }

                else
                {
                    alvo = player;
                }
                break;

            case Estados.Patrulhar:
                if (PertoDoWaypoint())
                {
                    MudarWaypoint();
                }
                else
                {
                    alvo = waypointAtual;
                }
                break;

            case Estados.Atropelar:
                if (PreparandoAtropelar())
                {
                    AtropelarJogador();
                }
                else
                {
                    transform.rotation = salvaRot;
                }
               

                break;

            case Estados.Atordoado:

                if (EsperouAtordoado())
                {
                    Patrulhar();
                }
                else
                {
                    alvo = transform;
                }

                break;

            case Estados.Atacar:
                alvo = transform;
                if (AcabouAtaque())
                {
                    if (PlayerNoAlcanceAtaque())
                    {
                        PlayerStats.playerVida--;
                    }
                        Esperar();
                }

                transform.rotation = Quaternion.Slerp(transform.rotation, new Quaternion(transform.rotation.x, Quaternion.LookRotation(player.position - transform.position).y, transform.rotation.z, transform.rotation.w), Time.deltaTime * velRot);

                break;
        }

        if (alvo != null)
            navMeshAgent.destination = alvo.position;

        //transform.rotation = Quaternion.Slerp(transform.rotation, new Quaternion(transform.rotation.x, Quaternion.LookRotation(alvo.position - transform.position).y, transform.rotation.z, transform.rotation.w), Time.deltaTime * velRot);

    }



    #region Esperar

    private void Esperar()
    {
        estadoAtual = Estados.Esperar;
        tempoEsperando = Time.time;
        anim.Play("Idle");
    }

    private bool EsperouSuficiente()
    {
        return tempoEsperar + tempoEsperando < Time.time;
    }

    #endregion Esperar

    #region Patrulhar

    private void Patrulhar()
    {
        estadoAtual = Estados.Patrulhar;
        anim.Play("Walk");
    }

    private void MudarWaypoint()
    {
        waypointIndex++;
        if (waypointIndex == waypoints.Length)
        {
            waypointIndex = 0;
        }
        waypointAtual = waypoints[waypointIndex];
    }

    private bool PertoDoWaypoint()
    {
        return Vector3.Distance(transform.position, waypointAtual.position) < distanciaMinimaWaypoint;
    }

    private bool DetectouJogador()
    {
        return Vector3.Distance(transform.position, player.position) < distanciaDetectar;
    }

    #endregion Patrulhar

    #region Perseguir

    private void Perseguir()
    {
        estadoAtual = Estados.Perseguir;
        anim.Play("Walk");
    }

    private bool VendoJogador()
    {
        if (Physics.Raycast(transform.position + Vector3.up * 0.5f, transform.forward, out hit, distanciaVisao))
        {
            //Debug.Log(hit.collider.tag);
            if (hit.collider.tag == "Player")
            {
                return true;
            }
        }
        return false;
    }

    #endregion Perseguir

    #region Atropelar

    private void Atropelar()
    {
        estadoAtual = Estados.Atropelar;
        alvoAtropelar.position = hit.point;
        alvo = transform;
        tempoInicioAtropelar = Time.time;
        salvaRot = transform.rotation;
        anim.Play("PrepareCharge");
    }

    bool PreparandoAtropelar()
    {
        return tempoInicioAtropelar + tempoAtropelarJogador < Time.time;
    }

    private void AtropelarJogador()
    {
        alvo = alvoAtropelar;
        // usar animaçao de atropelar
        // aumentar velocidade
        navMeshAgent.acceleration = aceleracaoAtropelar;
        navMeshAgent.speed = velMaxAtropelar;
        navMeshAgent.angularSpeed = velAngularAtropelar;

        if (Physics.Raycast(transform.position, transform.forward, out hit, distanciaColisao))
        {
            if (hit.collider.tag == "Parede")
            {
                Debug.Log("AcertouParede");
                Atordoado();
                navMeshAgent.velocity = Vector3.zero;
                ResetarNavMesh();
            }
            if (hit.collider.tag == "Obstaculo")
            {
                Debug.Log("AcertouObstaculo");
                Atordoado();
                navMeshAgent.velocity = Vector3.zero;
                ResetarNavMesh();
                Destroy(hit.collider.gameObject);
                MinotauroHP--;
            }
        }

        //Debug.Log("acelerou");
    }

    private void OnCollisionEnter(Collision col)
    {
        Debug.Log("Colisao");

        if (col.collider.CompareTag("Parede"))
        {

        }

        if (col.collider.CompareTag("Obstaculo"))
        {
        }

        if (col.collider.CompareTag("Player"))
        {
            Debug.Log("AcertouPlayer");
            if(estadoAtual == Estados.Atropelar)
            {
                PlayerStats.playerVida -= danoAtropelar;
                Atordoado();
            }
        }
    }

    private void ResetarNavMesh()
    {
        navMeshAgent.acceleration = aceleracaoBase * (2 - PlayerStats.forcaLuz/1023); // mais aceleraçao com menos luz
        navMeshAgent.speed = velMaxBase;
        navMeshAgent.angularSpeed = velAngularBase;
    }

    #endregion Atropelar

    #region Atordoado

    private void Atordoado()
    {
        estadoAtual = Estados.Atordoado;
        tempoInicioAtordoado = Time.time;
        anim.Play("Stun");
        ResetarNavMesh();
    }

    private bool EsperouAtordoado()
    {
        return tempoInicioAtordoado + tempoAtordoado < Time.time;
    }

    #endregion Atordoado

    #region Atacar
    private void Atacar()
    {
        anim.Play("Attack");
        tempoInicioAtaque = Time.time;
        estadoAtual = Estados.Atacar;
    }

    private bool PlayerNoAlcanceAtaque()
    {
        return Vector3.Distance(transform.position, player.position) < distanciaAtaque;
    }
    #endregion Atacar

    bool AcabouAtaque()
    {
        return tempoInicioAtaque + tempoAtacando < Time.time;
    }
}