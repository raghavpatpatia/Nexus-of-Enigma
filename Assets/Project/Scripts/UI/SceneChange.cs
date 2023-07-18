using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(Button))]
public class SceneChange : MonoBehaviour
{
    private Button button;
    [SerializeField] int sceneBuildIndex;

    private void Start()
    {
        button = GetComponent<Button>();
        button.onClick.AddListener(ChangeScene);
    }

    private void ChangeScene()
    {
        SceneManager.LoadScene(sceneBuildIndex);
    }
}
