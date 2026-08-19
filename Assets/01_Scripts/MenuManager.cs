using UnityEngine;
using TMPro;

public class MenuManager : MonoBehaviour
{
    public TMP_InputField[] players;
    [Header("UI References")]
    public TMP_Text currentPlayerText;
    public int playerCount = 2;
    public TMP_Text ResumeText;
    private void Start()
    {
        UpdateUI();
    }
    public void SetPlayerName()
    {
        string[] names = new string[playerCount];

        for (int i = 0; i < playerCount; i++)
        {
            string currentName = players[i].text.Trim();

            if (string.IsNullOrEmpty(currentName))
            {
                currentName = "Player " + (i + 1);
            }

            for (int j = 0; j < i; j++)
            {
                if (string.Equals(currentName, names[j], System.StringComparison.OrdinalIgnoreCase))
                {
                    Debug.Log("Nombre repetido: " + currentName);
                    return;    
                }
            }

            names[i] = currentName;
        }
        GameManager.Instance.SavePlayers(playerCount, names);
    
    }
    public void StartGame()
    {
       

       
        GameManager.Instance.StartGame();
    }
    private void UpdateUI()
    {
        currentPlayerText.text = playerCount.ToString();

        for (int i = 0; i < players.Length; i++)
        {
            if (i < playerCount)
            {
                players[i].gameObject.SetActive(true);
            }
            else
            {
                players[i].gameObject.SetActive(false);
            }
        }
    }
    public void IncreasePlayers()
    {
        if (playerCount < 5)
        {
            playerCount++;
            UpdateUI();
        }
    }

    public void DecreasePlayers()
    {
        if (playerCount > 2)
        {
            playerCount--;
            UpdateUI();
        }
    }
    public void fillResume()
    {
        string resume = "Players:\n";
        for (int i = 0; i < playerCount; i++)
        {
            resume += GameManager.Instance.playerNames[i] + "\n";
        }
        ResumeText.gameObject.SetActive(true);
        ResumeText.text = resume;
    }
}