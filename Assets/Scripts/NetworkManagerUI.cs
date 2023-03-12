using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;
using Cysharp.Threading.Tasks;
using NotEnoughCheddar.Networking;

// Written by https://github.com/matt-mert

public class NetworkManagerUI : MonoBehaviour
{
    [SerializeField]
    private Button createRelayButton;
    [SerializeField]
    private Button joinRelayButton;
    [SerializeField]
    private TMP_InputField inputField;
    [SerializeField]
    private TextMeshProUGUI codeText;

    private void Awake()
    {
        createRelayButton.onClick.AddListener(() =>
        {
            CreateRelayAsyncButton().Forget();
        });
        joinRelayButton.onClick.AddListener(() =>
        {
            JoinRelayAsyncButton().Forget();
        });

        NetworkManager.Singleton.OnServerStarted += ApplyCodeToText;
    }

    private async UniTaskVoid CreateRelayAsyncButton()
    {
        await UniTask.Delay(2000);
        await ConnectionManager.Instance.CreateRelayAsync();
    }

    private async UniTaskVoid JoinRelayAsyncButton()
    {
        await UniTask.Delay(2000);
        string str = inputField.text;
        await ConnectionManager.Instance.JoinRelayAsync(str);
    }

    private void ApplyCodeToText()
    {
        codeText.text = ConnectionManager.Instance.joinCode;
    }
}
