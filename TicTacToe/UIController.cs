using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{
    [Header("Turn objects!")]
    [SerializeField] TextMeshProUGUI p1_Text;
    [SerializeField] TextMeshProUGUI p2_Text;
    [SerializeField] Image p1_Image;
    [SerializeField] Image p2_Image;
    [SerializeField] GameObject gameOverMenu;
    [SerializeField] GameObject turnMenu;
    [SerializeField] TextMeshProUGUI winnerPlayerText;

    private void Start()
    {
        //Events assignments
        Board gameBoard = FindObjectOfType<Board>();
        gameBoard.OnPlayerTurnChanged += OnPlayerTurnChanged;
        gameBoard.OnGameFinished += GameBoard_OnGameFinished;

        p2_Text.alpha = 0.5f;
        p2_Image.color = new Vector4(p2_Image.color.r, p2_Image.color.g, p2_Image.color.b, 0.5f);
        gameOverMenu.SetActive(false);
    }

    private void GameBoard_OnGameFinished(object sender, PlayerTurnChangedEventArgs e)
    {
        switch (e.NewPlayerTurn)
        {
            case 0:
                winnerPlayerText.text = "player 1 wins!!";
                break;
            case 1:
                winnerPlayerText.text = "player 2 wins!!";
                break;
            case 2:
                winnerPlayerText.text = "draw!!";
                break;
        }
        gameOverMenu.SetActive(true);
        turnMenu.GetComponent<Animator>().SetBool("isGameOver", true);
    }

    private void OnPlayerTurnChanged(object sender, PlayerTurnChangedEventArgs e)
    {
        Debug.Log(e.NewPlayerTurn);
        if(e.NewPlayerTurn == 0)
        {
            p1_Text.alpha = 1f;
            p1_Image.color = new Vector4(p1_Image.color.r, p1_Image.color.g, p1_Image.color.b, 1);
            p2_Text.alpha = 0.5f;
            p2_Image.color = new Vector4(p2_Image.color.r, p2_Image.color.g, p2_Image.color.b, 0.5f);
        }
        else if(e.NewPlayerTurn == 1)
        {
            p1_Text.alpha = 0.5f;
            p1_Image.color = new Vector4(p1_Image.color.r, p1_Image.color.g, p1_Image.color.b, 0.5f);
            p2_Text.alpha = 1f;
            p2_Image.color = new Vector4(p2_Image.color.r, p2_Image.color.g, p2_Image.color.b, 1f);
        }
    }
    
}
