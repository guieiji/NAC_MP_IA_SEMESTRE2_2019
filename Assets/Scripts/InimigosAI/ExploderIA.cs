using System;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class ExploderIA : MonoBehaviour
{
    public enum Estados
    {
        Esperar,
        Perseguir,
        Retornar,
        Explodir
    }

    private Estados estadoAtual;

    private Transform alvo;

    private NavMeshAgent navMeshAgent;

    // Estado: Esperar
    [Header("Estados:Esperar")]
    public float tempoEsperar = 2f;

    private float tempoEsperando = 0f;
    private float distanciaDetectar = 10f;

    [Header("Estado:Perseguir")]
    public Transform player;

    [Header("Estado:Retornar")]
    public Transform posicaoInicial;

    private float distanciaMinimaPontoInicial = 2f;

    private float distanciaPontoInicial;

    [Header("Estado:Explodir")]
    private float distanciaExplosao = 3f;

    private bool explodindo;
    public GameObject explosionFx;
    public GameObject wispModelo;
    public float forcaExplosao = 1000f;

    private void Awake()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
    }

    // Start is called before the first frame update
    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        Esperar();
    }

    // Update is called once per frame
    private void Update()
    {
        ChecarEstados();
    }

    private void ChecarEstados()
    {
        distanciaPontoInicial = Vector3.Distance(posicaoInicial.position, transform.position);
        print("Achou jogador:" + DetectouJogador());

        if (DetectouJogador() && estadoAtual != Estados.Perseguir && estadoAtual != Estados.Explodir)
        {
            Perseguir();
        }

        switch (estadoAtual)
        {
            case Estados.Esperar:
                if (DetectouJogador())
                {
                    Perseguir();
                }

                alvo = transform;

                break;

            case Estados.Perseguir:
                if (!DetectouJogador())
                {
                    Retornar();
                }
                else
                {
                    alvo = player;
                    if (JogadorDentroExplosao())
                    {
                        Explodir();
                    }
                }

                break;

            case Estados.Retornar:
                if (PertoPontoInicial())
                {
                    Esperar();
                }
                else
                {
                    alvo = posicaoInicial;
                }
                break;

            case Estados.Explodir:
                alvo = transform;
                if (!explodindo)
                {
                    explodindo = true;
                    GerarExplosao();
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

    #endregion Esperar

    #region Perseguir

    private void Perseguir()
    {
        estadoAtual = Estados.Perseguir;
    }

    private bool DetectouJogador()
    {
        return Vector3.Distance(transform.position, player.position) < distanciaDetectar;
    }

    #endregion Perseguir

    #region Retornar

    private void Retornar()
    {
        estadoAtual = Estados.Retornar;
    }

    private bool PertoPontoInicial()
    {
        return Vector3.Distance(transform.position, posicaoInicial.position) <= distanciaMinimaPontoInicial;
    }

    #endregion Retornar

    #region Explodir

    private void Explodir()
    {
        estadoAtual = Estados.Explodir;
    }

    private void GerarExplosao()
    {
        // usar animaçao de explosão
        explodindo = true;
        StartCoroutine(EsperarExplosao());
    }

    private IEnumerator EsperarExplosao()
    {
        yield return new WaitForSeconds(2f);
        Instantiate(explosionFx, wispModelo.transform.position, transform.rotation);

        if (JogadorDentroExplosao())
        {
            Debug.Log("explodiuPlayer");

            PlayerStats.playerVida -= 2;
            player.GetComponent<Rigidbody>().AddForce((player.position - transform.position).normalized * forcaExplosao,ForceMode.Impulse);
        }
        Destroy(wispModelo);
        Destroy(gameObject);
    }

    private bool JogadorDentroExplosao()
    {
        return Vector3.Distance(transform.position, alvo.position) < distanciaExplosao;
    }

    #endregion Explodir
}