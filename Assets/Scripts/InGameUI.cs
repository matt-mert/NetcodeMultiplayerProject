using TMPro;
using UnityEngine;
using UnityEngine.UI;

// Written by https://github.com/matt-mert

public class InGameUI : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI playerName;
    [SerializeField]
    private TextMeshProUGUI playerEnergy;
    [SerializeField]
    private TextMeshProUGUI playerTimer;
    [SerializeField]
    private TextMeshProUGUI opponentName;
    [SerializeField]
    private TextMeshProUGUI opponentEnergy;
    [SerializeField]
    private TextMeshProUGUI opponentTimer;
    [SerializeField]
    private Button endTurnButton;
    [SerializeField]
    private Button settingsButton;
    [SerializeField]
    private Button disconnectButton;
    [SerializeField]
    private Button closeSettingsButton;
    [SerializeField]
    private GameObject panel;

    private void Awake()
    {
        FlowManager.Instance.OnTurnCounterUpdate += UpdateTurnCounter;
        FlowManager.Instance.OnPlayerEnergyUpdate += UpdatePlayerEnergy;
        FlowManager.Instance.OnPlayerTimerUpdate += UpdatePlayerTimer;
        FlowManager.Instance.OnOpponentEnergyUpdate += UpdateOpponentEnergy;
        FlowManager.Instance.OnOpponentTimerUpdate += UpdateOpponentTimer;
    }

    private void OnEnable()
    {
        settingsButton.onClick.AddListener(OpenSettings);
        closeSettingsButton.onClick.AddListener(CloseSettings);
        disconnectButton.onClick.AddListener(DisconnectFromGame);
    }

    private void OnDisable()
    {
        settingsButton.onClick.RemoveListener(OpenSettings);
        closeSettingsButton.onClick.RemoveListener(CloseSettings);
        disconnectButton.onClick.RemoveListener(DisconnectFromGame);
    }

    private void UpdateTurnCounter(int prev, int next)
    {
        Debug.Log("Turn counter is not yet implemented.");
    }

    private void UpdatePlayerEnergy(int prev, int next)
    {
        playerEnergy.text = "E-" + next.ToString();
    }

    private void UpdateOpponentEnergy(int prev, int next)
    {
        opponentEnergy.text = "E-" + next.ToString();
    }

    private void UpdatePlayerTimer(int prev, int next)
    {
        playerTimer.text = "T-" + next.ToString();
    }

    private void UpdateOpponentTimer(int prev, int next)
    {
        opponentTimer.text = "T-" + next.ToString();
    }

    private void OpenSettings()
    {
        panel.SetActive(true);
    }

    private void CloseSettings()
    {
        panel.SetActive(false);
    }

    private void DisconnectFromGame()
    {
        
    }
}
