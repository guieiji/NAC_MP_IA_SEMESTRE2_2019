using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerStats : MonoBehaviour
{
    


    public static int playerVida;
    public static float forcaLuz;
    float inputLuz;
    float maxLuz;

    public Text vida;
    // Start is called before the first frame update
    void Start()
    {
        playerVida = 3;
        forcaLuz = 0f;
    }

    // Update is called once per frame
    void Update()
    {
        print(forcaLuz);

        forcaLuz = inputLuz / maxLuz;


        if(playerVida < 0)
        {
            playerVida = 0;

        }
        vida.text = playerVida.ToString();
    }
}
