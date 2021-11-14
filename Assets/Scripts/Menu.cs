using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Menu : MonoBehaviour
{
    [SerializeField] private GameObject StartButton;
    [SerializeField] private GameObject BeginButton;
    [SerializeField] private GameObject CreditsButton;
    [SerializeField] private GameObject EndScreen;
     private GameObject CurrentScreen;
    public Text ScoreText;
    [SerializeField] private Player player;
    // Start is called before the first frame update
    void Start()
    {
        Time.timeScale = 0f;
        player = GameObject.Find("Player").GetComponent<Player>();

    }

    public void StartGame()
    {
        RestartGame();
        Time.timeScale = 0f;
        StartButton.SetActive(false);
        BeginButton.SetActive(true);
    }

    public void BeginMatch()
    {
        BeginButton.SetActive(false);
        Time.timeScale = 1f;
        player.gameStart = true;
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void RestartGame()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Zombie");
        foreach (GameObject enemy in enemies)
        {
            Destroy(enemy);
        }
        Time.timeScale = 1f;
        player.ammo = player.maxAmmo;
        player.health = player.maxhealth;
        player.Score = 0;
        player.ScoreText.text = "Score: " + player.Score;
        player.healthText.text = player.health + " ";
        EndScreen.SetActive(false);
    }

    public void Return2Menu()
    {
        CreditsButton.SetActive(false);
        StartButton.SetActive(true);
    }

    public void CreditsMenu()
    {
        StartButton.SetActive(false);
        EndScreen.SetActive(false);
        CreditsButton.SetActive(true);
    }
}
