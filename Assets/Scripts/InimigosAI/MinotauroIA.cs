﻿using System;
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
        Atordoado
    }

    private Estados estadoAtual;

    private Transform alvo;

    public Transform player;

    private NavMeshAgent navMeshAgent;
    private float raySize = 3f;
    private RaycastHit hit;

    private float aceleracaoBase;
    private float velMaxBase;
    private float velAngularBase;

    // Estado: Esperar
    [Header("Estados:Esperar")]
    public float tempoEsperar = 2f;

    private float tempoEsperando = 0f;
    private float distanciaDetectar = 15f;

    [Header("Estado:Patrulhar")]
    public Transform[] waypoints;

    public Transform waypointAtual;
    private int waypointIndex;
    public float distanciaMinimaWaypoint = 1f;

    [Header("Estado: Perseguir")]
    private float distanciaVisao = 10f;

    [Header("Estado: Atropelar")]
    private float aceleracaoAtropelar = 16f;

    private float velMaxAtropelar = 20f;
    private float velAngularAtropelar = 0f;

    public Transform alvoAtropelar;
    private float distanciaColisao = 2f;

    [Header("Estado: Atordoado")]
    private float tempoAtordoado = 5f;

    private float tempoInicioAtordoado;

    private void Awake()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
    }

    // Start is called before the first frame update
    private void Start()
    {
        aceleracaoBase = navMeshAgent.acceleration;
        velMaxBase = navMeshAgent.speed;
        velAngularBase = navMeshAgent.angularSpeed;

        Esperar();
    }

    // Update is called once per frame
    private void Update()
    {
        Debug.DrawRay(transform.position, transform.forward * raySize, Color.red, 0.5f);
        ChecarEstados();
    }

    private void ChecarEstados()
    {
        if (estadoAtual != Estados.Atordoado && estadoAtual != Estados.Atropelar && estadoAtual != Estados.Perseguir && DetectouJogador())
        {
            estadoAtual = Estados.Perseguir;
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
                AtropelarJogador();

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

    private bool EsperouSuficiente()
    {
        return tempoEsperar + tempoEsperando < Time.time;
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
    }

    private bool VendoJogador()
    {
        if (Physics.Raycast(transform.position, transform.forward, out hit, distanciaVisao))
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
        alvo = alvoAtropelar;
    }

    private void AtropelarJogador()
    {
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
                estadoAtual = Estados.Atordoado;
                navMeshAgent.velocity = Vector3.zero;
                ResetarNavMesh();
            }
        }

        //Debug.Log("acelerou");
    }

    //private void OnCollisionEnter(Collision col)
    //{
    //    Debug.Log("Colisao");

    //    if (col.collider.CompareTag("Parede"))
    //    {
    //        Debug.Log("AcertouParede");
    //        estadoAtual = Estados.Esperar;
    //        alvo = transform;
    //        ResetarNavMesh();
    //    }

    //    if (col.collider.CompareTag("Obstaculo"))
    //    {
    //    }

    //    if (col.collider.CompareTag("Player"))
    //    {
    //    }
    //}

    private void ResetarNavMesh()
    {
        navMeshAgent.acceleration = aceleracaoBase;
        navMeshAgent.speed = velMaxBase;
        navMeshAgent.angularSpeed = velAngularBase;
    }

    #endregion Atropelar

    #region Atordoado

    private void Atordoado()
    {
        estadoAtual = Estados.Atordoado;
        tempoInicioAtordoado = Time.time;
    }

    private bool EsperouAtordoado()
    {
        return tempoInicioAtordoado + tempoAtordoado < Time.time;
    }

    #endregion Atordoado
}