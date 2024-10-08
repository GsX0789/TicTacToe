using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuManager : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI creditsText;
    [SerializeField] Image xImage;
    private const int GAME_INDEX = 1;

    public void LoadGameScene()
    {
        SceneManager.LoadScene(GAME_INDEX);
    }

    public void ShowCredits()
    {
        if (creditsText != null)
        {
            creditsText.gameObject.SetActive(!creditsText.gameObject.activeSelf);
            xImage.gameObject.SetActive(!xImage.gameObject.activeSelf);
        }
    }
}
