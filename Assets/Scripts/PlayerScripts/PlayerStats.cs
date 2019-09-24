using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerStats : MonoBehaviour
{
    


    public static int playerVida;
    public static float forcaLuz;
    public static int ambiente_player;
    

    public Text vida;
    public Text GameOver;
    // Start is called before the first frame update
    void Start()
    {
        playerVida = 3;
        ambiente_player = 1;
        forcaLuz = 0f;
        GameOver.enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        print(forcaLuz);
        print("ambiente:" + ambiente_player);



        if(playerVida < 0)
        {
            playerVida = 0;

        }


        vida.text = playerVida.ToString();

        if(playerVida == 0)
        {
            IniciarGameOver();
        }

    }

    private void IniciarGameOver()
    {
        Time.timeScale = Time.timeScale != 0 ? 0 : 1;

        GameOver.enabled = true;

    }
}
