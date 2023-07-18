using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class PanelChange : MonoBehaviour
{
    [SerializeField] GameObject panel1;
    [SerializeField] GameObject panel2;
    private Button button;

    private void Awake()
    {
        button = GetComponent<Button>();
        button.onClick.AddListener(ChangePanel);
    }

    private void Start()
    {
        panel1.SetActive(true);
        panel2.SetActive(false);
    }

    private void ChangePanel()
    {
        panel1.SetActive(false);
        panel2.SetActive(true);
    }
}
