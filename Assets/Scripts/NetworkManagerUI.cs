using TMPro;
using UnityEngine;
using UnityEngine.UI;

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
            ConnectionManager.Instance.CreateRelay();
        });
        joinRelayButton.onClick.AddListener(() =>
        {
            string str = inputField.text;
            ConnectionManager.Instance.JoinRelay(str);
        });
    }

    private void OnEnable()
    {
        ConnectionManager.Instance.OnHostStarted += ApplyCodeToText;
    }

    private void OnDisable()
    {
        ConnectionManager.Instance.OnHostStarted -= ApplyCodeToText;
    }

    private void ApplyCodeToText()
    {
        codeText.text = ConnectionManager.Instance.joinCode;
    }
}
