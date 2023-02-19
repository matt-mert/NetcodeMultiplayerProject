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

    private TestRelay testRelay;

    private void Awake()
    {
        testRelay = FindObjectOfType<TestRelay>();

        createRelayButton.onClick.AddListener(() =>
        {
            testRelay.CreateRelay();
        });
        joinRelayButton.onClick.AddListener(() =>
        {
            string str = inputField.text;
            testRelay.JoinRelay(str);
        });
    }

    private void OnEnable()
    {
        testRelay.OnHostStarted += ApplyCodeToText;
    }

    private void OnDisable()
    {
        testRelay.OnHostStarted -= ApplyCodeToText;
    }

    private void ApplyCodeToText()
    {
        codeText.text = testRelay.joinCode;
    }
}
