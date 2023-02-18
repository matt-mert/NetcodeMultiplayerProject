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
    private TestRelay testRelay;

    private void Awake()
    {
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
}
