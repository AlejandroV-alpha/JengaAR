using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public int playerCount = 2;
    public string[] playerNames;

    public int currentPlayer = 0;
    public GameObject[] playerInputs;
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

    public void SavePlayers(int count, string[] names)
    {
        playerCount = count;
        playerNames = names;

        currentPlayer = 0;
    }

    public void StartGame()
    {
        SceneManager.LoadScene("SampleScene");
    }

    public string GetCurrentPlayerName()
    {
        return playerNames[currentPlayer];
    }

    public void NextPlayer()
    {
        currentPlayer++;

        if (currentPlayer >= playerCount)
        {
            currentPlayer = 0;
        }
    }

  
}
