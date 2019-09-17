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
        Retornar,
        Atropelar
    }

    private Estados estadoAtual;

    private Transform alvo;

    public Transform player;

    private NavMeshAgent navMeshAgent;

    // Estado: Esperar
    [Header("Estados:Esperar")]
    public float tempoEsperar = 2f;

    private float tempoEsperando = 0f;
    private float distanciaDetectar = 5f;

    [Header("Estado:Patrulhar")]
    public Transform[] waypoints;

    private Transform waypointAtual;
    private int waypointIndex;
    private float distanciaMinimaWaypoint;

    [Header("Estado:Retornar")]
    private Vector3 posicaoInicial;

    private float distanciaMinimaPontoInicial = 2f;

    [Header("Estado:Explodir")]
    private float distanciaExplosao = 1f;

    private void Awake()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
    }

    // Start is called before the first frame update
    private void Start()
    {
        posicaoInicial = transform.position;

        Esperar();
    }

    // Update is called once per frame
    private void Update()
    {
        ChecarEstados();
    }

    private void ChecarEstados()
    {
        switch (estadoAtual)
        {
            case Estados.Esperar:
                alvo = transform;

                break;

            case Estados.Patrulhar:
                if (!DetectouJogador())
                {
                    Retornar();
                }
                else
                {
                    alvo = waypointAtual;
                }

                break;

            case Estados.Retornar:
                if (PertoPontoInicial())
                {
                    Esperar();
                }
                else
                {
                    alvo.position = posicaoInicial;
                }
                break;

            case Estados.Atropelar:
                AtropelarJogador();

                break;
        }

        if (alvo != null)
            navMeshAgent.destination = alvo.position;
    }

    #region Esperar

    private void Esperar()
    {
        estadoAtual = Estados.Esperar;
        tempoEsperando = Time.time;
    }

    #endregion Esperar

    #region Patrulhar

    private void Patrulhar()
    {
        estadoAtual = Estados.Patrulhar;
    }

    private void MudarWaypoint()
    {
        waypointIndex++;
        if (waypointIndex == waypoints.Length)
        {
            waypointIndex = 0;
        }
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

    #region Retornar

    private void Retornar()
    {
        estadoAtual = Estados.Retornar;
    }

    private bool PertoPontoInicial()
    {
        return Vector3.Distance(transform.position, posicaoInicial) <= distanciaMinimaPontoInicial;
    }

    #endregion Retornar

    #region Atropelar

    private void Atropelar()
    {
        estadoAtual = Estados.Atropelar;
    }

    private void AtropelarJogador()
    {
        // usar animaçao de explosão
        // aumentar velocidade
    }

    #endregion Atropelar
}