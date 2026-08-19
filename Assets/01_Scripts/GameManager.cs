using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public int playerCount = 2;
  
    public int currentPlayer = 0;
    public double turnTime = 15.0;
    public  Player[] players { get; set; }
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void SavePlayers(int count,double turntime, string[] names)
    {
        playerCount = count;
        turnTime = turntime;
        players = new Player[count] ;
        for (int i = 0; i < count; i++)
        {
            players[i] = new Player { Name = names[i], Wins = 0, Losses = 0, times = new double[0] };
        }

        currentPlayer = 0;
    }

    public void StartGame()
    {
        SceneManager.LoadScene("SampleScene");
    }

    public string GetCurrentPlayerName()
    {
        return players[currentPlayer].Name;
    }

    public void NextPlayer()
    {
        currentPlayer++;

        if (currentPlayer >= playerCount)
        {
            currentPlayer = 0;
        }
    }
    public void seePlayers()
    {
        for (int i = 0; i < playerCount; i++)
        {
            Debug.Log("Player " + (i + 1) + ": " + players[i].Name);
        }
    }

}
