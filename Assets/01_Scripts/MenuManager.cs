using UnityEngine;
using TMPro;
using System.Runtime.CompilerServices;
using UnityEngine.Rendering.Universal;
using Unity.VectorGraphics;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    public static bool openSettingsOnLoad = false;

    public TMP_InputField[] players;

    [Header("UI References")]
    public TMP_Text currentPlayerText;
    public int playerCount = 2;
    public TMP_Text ResumeText;
    public TMP_Text errorText;

    public bool isResume = false;
    public bool isGameResume = false;

    public GameObject settingsPanel;
    public GameObject resumePanel;
    public GameObject mainPanel;


    public double turnTime = 15.0;
    public TMP_Text currentTurnTimeText;

    private void Start()
    {
        if (openSettingsOnLoad)
        {
            settingsPanel.SetActive(true);
            resumePanel.SetActive(false);
                        mainPanel.SetActive(false);
            UpdateUI();

            openSettingsOnLoad = false;
        }
        else
        {
            if (isResume)
            {
                fillResume();
            }
            else if (isGameResume)
            {
                fillGameResume();
            }
            else
            {
                if(ResumeText!=null)
                UpdateUI();
            }

        }
    }

    private void OnEnable()
    {
        if (isResume)
        {
            fillResume();
        }
        else if (isGameResume)
        {
            fillGameResume();
        }
        else
        {
            UpdateUI();
        }
    }

    public void SetPlayerName()
    {
        string[] names = new string[playerCount];

        errorText.gameObject.SetActive(false);

        for (int i = 0; i < playerCount; i++)
        {
            string currentName = players[i].text.Trim();

            if (string.IsNullOrEmpty(currentName))
            {
                currentName = "Player " + (i + 1);
            }

            for (int j = 0; j < i; j++)
            {
                if (string.Equals(
                    currentName,
                    names[j],
                    System.StringComparison.OrdinalIgnoreCase))
                {
                    errorText.text = "El nombre '" + currentName + "' ya está repetido.";
                    errorText.gameObject.SetActive(true);

                    resumePanel.SetActive(false);
                    settingsPanel.SetActive(true);

                    return;
                }
            }

            names[i] = currentName;
        }

        GameManager.Instance.SavePlayers(playerCount, turnTime, names);

        resumePanel.SetActive(true);
        settingsPanel.SetActive(false);
    }

    public void StartGame()
    {
        GameManager.Instance.StartGame();
    }

    private void UpdateUI()
    {
        currentPlayerText.text = playerCount.ToString();
        currentTurnTimeText.text = turnTime.ToString() + "s";

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

    public void IncreaseTurnTime()
    {
        if (turnTime < 60)
        {
            turnTime++;
            UpdateUI();
        }
    }

    public void DecreaseTurnTime()
    {
        if (turnTime > 5)
        {
            turnTime--;
            UpdateUI();
        }
    }

    public void fillResume()
    {
        string resume =
            $"Tiempo de turno: {GameManager.Instance.turnTime}s\n" +
            $"Cantidad de jugadores {GameManager.Instance.playerCount}:\n" +
            $"Nombres de los jugadores:";

        for (int i = 0; i < GameManager.Instance.playerCount; i++)
        {
            resume += GameManager.Instance.players[i].Name + "\n";
        }

        ResumeText.text = resume;
    }

    public void fillGameResume()
    {
        if(GameManager.Instance == null || GameManager.Instance.players == null )
            return;
        string resume =
            $"Cantidad de jugadores {GameManager.Instance.playerCount}:\n" +
            $"Nombres de los jugadores:";

        double promedio = 0;

        for (int i = 0; i < GameManager.Instance.playerCount; i++)
        {
            resume +=
                GameManager.Instance.players[i].Name +
                " - ganadas: " +
                GameManager.Instance.players[i].Wins +
                " - Perdidas: " +
                GameManager.Instance.players[i].Losses;

            if (GameManager.Instance.players[i].lost)
            {
                resume += " - Perdedor";
            }
            else
            {
                resume += " +1 Victoria";
            }

            promedio = 0;

            for (int j = 0; j < GameManager.Instance.players[i].times.Length; j++)
            {
                promedio += GameManager.Instance.players[i].times[j];
            }

            promedio /= GameManager.Instance.players[i].times.Length;

            resume +=
                " - Promedio de tiempo de accion: " +
                promedio +
                "s";

            resume += "\n";
        }

        ResumeText.text = resume;
    }

    public void restartSettings()
    {
        GameManager.Instance.playerCount = 2;
        GameManager.Instance.turnTime = 15.0;
        GameManager.Instance.players = null;
        GameManager.Instance.currentPlayer = 0;
    }

    public void restartScores()
    {
        if(GameManager.Instance == null || GameManager.Instance.players == null)
            return;
        for (int i = 0; i < GameManager.Instance.playerCount; i++)
        {
            GameManager.Instance.players[i].Wins = 0;
            GameManager.Instance.players[i].Losses = 0;
            GameManager.Instance.players[i].times = new double[0];
            GameManager.Instance.players[i].lost = false;
        }
        fillGameResume()
;    }

    public void ResetGame()
    {
        restartSettings();
        restartScores();

        isResume = false;
        openSettingsOnLoad = false;

        SceneManager.LoadScene("MainMenu");
    }

    public void realoadGame()
    {
        for (int i = 0; i < GameManager.Instance.playerCount; i++)
        {
            GameManager.Instance.players[i].times = new double[0];
            GameManager.Instance.players[i].lost = false;
        }

        SceneManager.LoadScene("SampleScene");
    }

    public void GoToSettingsFromFinish()
    {
        openSettingsOnLoad = true;

        SceneManager.LoadScene("MainMenu");
    }
}