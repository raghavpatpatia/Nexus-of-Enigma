using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class ResumeGame : MonoBehaviour
{
    private Button button;
    [SerializeField] GameObject pausePanel;

    private void Start()
    {
        button = GetComponent<Button>();
        button.onClick.AddListener(Resume);
    }

    private void Resume()
    {
        SoundManager.Instance.PlayMusic(Sounds.ButtonClick);
        pausePanel.SetActive(false);
        Time.timeScale = 1;
    }
}
