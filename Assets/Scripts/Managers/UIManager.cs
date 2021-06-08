using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{

    [SerializeField] private Image playerHealthImage;
    [SerializeField] private Player player;

    private float playerStartHealth;

    void Start()
    {
        playerStartHealth = player.Health;
    }

    // Update is called once per frame
    void Update()
    {
        playerHealthImage.fillAmount = player.Health / playerStartHealth;
    }
}
