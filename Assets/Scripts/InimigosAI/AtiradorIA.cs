using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AtiradorIA : MonoBehaviour
{
    private float raySize = 5f;
    private RaycastHit hit;

    public enum Estados
    {
        PROCURAR,

        ATIRAR,

        RECARREGAR
    }

    private Estados estadoAtual;
    private Transform alvo;
    private Transform player;
    private float distanciaDoJogador;

    Animator anim;

    [Header("ESTADO:PROCURAR")]
    private float distanciaProcurar = 20f;

    [Header("ESTADO:ATIRAR")]
    private float tempoAtirar = 4f;

    private float tempoComecouAtirar;
    private float velRot = 5f;
    public GameObject bala;
    public Transform pontaArma;
    public float rateOfFire = 1f;
    private bool atirando;

    [Header("ESTADO:RECARREGAR")]
    private float tempoRecarregar = 2f;

    private float tempoComecouRecarregar;

    private void Awake()
    {
        estadoAtual = Estados.PROCURAR;
        anim = GetComponent<Animator>();
    }

    // Start is called before the first frame update
    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    // Update is called once per frame
    private void Update()
    {
        distanciaDoJogador = Vector3.Distance(transform.position, player.position);
        Debug.DrawRay(transform.position, transform.forward * raySize, Color.red, 0.5f);
        switch (estadoAtual)
        {
            case Estados.PROCURAR:
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
                    
                    if (Physics.Raycast(transform.position + Vector3.up * 0.5f, transform.forward, out hit, distanciaProcurar))
                    {
                        Debug.Log(hit.collider.tag);
                        if (hit.collider.tag == "Player")
                        {
                            Debug.Log("atirou");
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
                    Debug.Log("Acabou Reload");
                    Procurar();
                }
                else
                {
                    alvo = player;
                }

                break;
        }
        if (estadoAtual == Estados.ATIRAR)
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, new Quaternion(transform.rotation.x, Quaternion.LookRotation(player.position - transform.position).y, transform.rotation.z, transform.rotation.w), Time.deltaTime * velRot);

        }
    }
    #region PROCURAR

    private void Procurar()
    {
        estadoAtual = Estados.PROCURAR;
    }

    #endregion PROCURAR

    #region ATIRAR

    private void Atirar()
    {
        estadoAtual = Estados.ATIRAR;
        tempoComecouAtirar = Time.time;
    }

    private IEnumerator AtirarContinuo()
    {
        yield return new WaitForSeconds(rateOfFire);
        anim.Play("Shoot");
        AtirarNoJogador();
        StartCoroutine(AtirarContinuo());
    }

    private bool AcabaramBalas()
    {
        return tempoAtirar + tempoComecouAtirar < Time.time;
    }

    private bool PlayerNoAlcance()
    {
        return Vector3.Distance(transform.position, player.position) < distanciaProcurar;
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
        StopAllCoroutines();
    }

    private void RecarregouBalas()
    {
        estadoAtual = Estados.PROCURAR;
    }

    private bool RecarregouTempoSuficiente()
    {
        return tempoRecarregar + tempoComecouRecarregar < Time.time;
    }

    #endregion RECARREGAR
}