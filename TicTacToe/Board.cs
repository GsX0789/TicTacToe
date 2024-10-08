using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Board : MonoBehaviour
{
    //width = x,row
    [SerializeField] int boardWidth = 0;
    //height = y,column
    [SerializeField] int boardHeight = 0;

    [Header("Prefabs")]
    [SerializeField] GameObject tilePrefab;
    [SerializeField] GameObject oPiece;
    [SerializeField] GameObject xPiece;
    [SerializeField] AudioClip piecePlaced;
    [SerializeField] AudioClip winSound;
    [SerializeField] ParticleSystem p1_System;
    [SerializeField] ParticleSystem p2_System;

    private AudioSource audioSourceRef;

    private GridLayoutGroup gridlayout;
    private Tile[,] tiles;

    //0 = Player1, 1 = Player2, 2 = Draw!
    //0 = X , 1 = O and 2 = Draw
    //Works more as a game state
    [SerializeField] private int currentTurn = 0;
    private bool isThereAWinner = false;
    private bool draw = false;

    //Events to Update the HUD
    public event EventHandler<PlayerTurnChangedEventArgs> OnPlayerTurnChanged;
    public event EventHandler<PlayerTurnChangedEventArgs> OnGameFinished;

    //Input Reference
    PlayerActions playerActions;

    private void Start()
    {
        gridlayout = GetComponent<GridLayoutGroup>();
        CreateGrid();

        playerActions = new PlayerActions();
        playerActions.MouseActions.Enable();
        playerActions.MouseActions.MouseLeftClick.performed += MouseLeftClick;

        audioSourceRef = GetComponent<AudioSource>();
    }

    private void OnDisable()
    {
        playerActions.MouseActions.Disable();
        playerActions.MouseActions.MouseLeftClick.performed -= MouseLeftClick;
    }

    protected virtual void OnPlayerTurnChangedHandler(PlayerTurnChangedEventArgs e)
    {
        //Method to send the event(this -> the actual gameBoard, e -> extra parameters
        //here we will use the currentTurn!
        OnPlayerTurnChanged?.Invoke(this,e);
    }

    protected virtual void OnGameEndedHandler(PlayerTurnChangedEventArgs e)
    {
        OnGameFinished?.Invoke(this,e);
    }

    private void MouseLeftClick(InputAction.CallbackContext action)
    {
        //getting the position of the mouse
        Vector2 mousePosition = Camera.main.ScreenToWorldPoint(
            new Vector3(Input.mousePosition.x, Input.mousePosition.y, 
            Camera.main.nearClipPlane));

        RaycastHit2D hit = Physics2D.Raycast(mousePosition, Vector2.zero, 0f);
        //Checking if we are really hitting the board tile
        if(hit.collider != null && hit.collider.GetComponent<Tile>() != null && !isThereAWinner
            && !draw)
        {
            switch (currentTurn)
            {
                case 0:
                    //Player 1 Turn
                    //Checking if the tile is not holding any piece.
                    if (!hit.collider.GetComponent<Tile>().IsHoldingAPiece)
                    {
                        //if not instantiate one, and set its data!
                        GameObject xNewPiece = Instantiate(xPiece, hit.transform);
                        hit.collider.GetComponent<Tile>().IsHoldingAPiece = true;
                        hit.collider.GetComponent<Tile>().SetHoldingPiece(xNewPiece);
                        CheckForWinner();
                        if (!isThereAWinner)
                        {
                            currentTurn = 1;
                            ChangePlayerTurn(currentTurn);
                        }
                    }
                    break;
                case 1:
                    //Player 2 Turn
                    if (!hit.collider.GetComponent<Tile>().IsHoldingAPiece)
                    {
                        GameObject oNewPiece = Instantiate(oPiece, hit.transform);
                        hit.collider.GetComponent<Tile>().IsHoldingAPiece = true;
                        hit.collider.GetComponent<Tile>().SetHoldingPiece(oNewPiece);
                        CheckForWinner();
                        //ChangeTurn to player 1
                        if (!isThereAWinner)
                        {
                            currentTurn = 0;
                            ChangePlayerTurn(currentTurn);
                        }
                    }
                    break;
            }
            audioSourceRef.PlayOneShot(piecePlaced);
            CheckForDraw();
            //Debug.Log($"Hit object {hit.collider.gameObject.name}");
        }
        

    }

    //Here we are sending the event, when the turn changes
    private void ChangePlayerTurn(int playerTurn)
    {
        PlayerTurnChangedEventArgs args = new PlayerTurnChangedEventArgs();
        args.NewPlayerTurn = playerTurn;
        OnPlayerTurnChangedHandler(args);
    }

    private void OnGameOver(int playerTurn)
    {
        PlayerTurnChangedEventArgs args = new PlayerTurnChangedEventArgs();
        args.NewPlayerTurn = playerTurn;
        OnGameEndedHandler(args);
    }

    private void CreateGrid()
    {
        //Instanciating the tiles array
        //here the lenght is 9(3x3)
        tiles = new Tile[boardWidth, boardHeight];

        for(int x = 0; x < boardWidth; x++)
        {
            for(int y = 0; y < boardHeight; y++)
            {
                GameObject newTile = Instantiate(tilePrefab, transform.position,
                    Quaternion.identity, transform);
                newTile.name = $"x:{x}, y:{y}";
                newTile.transform.position = new Vector2(x, y);

                //Here, we are only setting the reference pos of the tile
                //e.g, the first tile is 0,0, but the array does not know it
                //until we set this line here!
                newTile.GetComponent<Tile>().SetTilePos(x, y);

                //Here we add the currentTile in the 2D Array
                tiles[x,y] = newTile.GetComponent<Tile>();
            }
        }

        gridlayout.constraintCount = 3;
    }

    private void CheckForWinner()
    {

        //Check for rows
        for(int row =0; row < boardWidth; row++)
        {
            if (CheckThreeInARow(tiles[row, 0], tiles[row, 1], tiles[row,2]))
            {
                DeclareWinner();
            }
        }

        //Check for columns
        for(int col =0; col < boardHeight; col++)
        {
            if (CheckThreeInARow(tiles[0, col], tiles[1,col], tiles[2, col]))
            {
                DeclareWinner();
            }
        }

        //Check for diagonal(Bottom-Left to Top-Right)
        if (CheckThreeInARow(tiles[0, 0], tiles[1, 1], tiles[2, 2]))
        {
            DeclareWinner();
        }

        //Check for diagonal(Bottom-Right to Top-Left)
        if (CheckThreeInARow(tiles[0, 2], tiles[1, 1], tiles[2, 0]))
        {
            DeclareWinner();
        }
    }

    private bool CheckThreeInARow(Tile t1, Tile t2, Tile t3)
    {
        return t1.PieceData != null && t2.PieceData != null 
            && t3.PieceData != null && t1.PieceData.tag == t2.PieceData.tag 
            && t2.PieceData.tag == t3.PieceData.tag;
    }

    private void DeclareWinner()
    {
        isThereAWinner = true;
        audioSourceRef.PlayOneShot(winSound);
        OnGameOver(currentTurn);
        switch(currentTurn)
        {
            case 0:
                p1_System.Play();
                break;
            case 1:
                p2_System.Play();
                break;
        }
    }

    private void CheckForDraw()
    {
        //Logic = since we have nine tiles, we check each tile
        //if the tile has a piece adds 1, if the final calc is:
        // 9(tiles with pieces) == total tiles and there is no winner
        // then it's a draw!
        int totalTiles = boardHeight * boardWidth;
        int tilesWithAPiece = 0;
        foreach(Tile tile in tiles)
        {
            if (tile.IsHoldingAPiece)
            {
                tilesWithAPiece++;
                if(tilesWithAPiece == totalTiles && !isThereAWinner)
                {
                    OnGameOver(2);
                    draw = true;
                } 
            }
        }
    }

    public void ResetGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void LoadMainMenu()
    {
        SceneManager.LoadScene(0);
    }
}

public class PlayerTurnChangedEventArgs : EventArgs
{
    //A simple property
    public int NewPlayerTurn { get; set; }
}