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
    private TextMeshProUGUI opponentName;
    [SerializeField]
    private TextMeshProUGUI opponentEnergy;
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

    private void OnEnable()
    {
        settingsButton.onClick.AddListener(OpenSettings);
        closeSettingsButton.onClick.AddListener(CloseSettings);
        disconnectButton.onClick.AddListener(DisconnectFromGame);
        // endTurnButton.onClick.AddListener(EndTurn);
    }

    private void OnDisable()
    {
        settingsButton.onClick.RemoveListener(OpenSettings);
        closeSettingsButton.onClick.RemoveListener(CloseSettings);
        disconnectButton.onClick.RemoveListener(DisconnectFromGame);
        // endTurnButton.onClick.RemoveListener(EndTurn);
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

    private void EndTurn()
    {
        
    }
}
