using UnityEngine;

public class Tile : MonoBehaviour
{
    //The serializeFields in this class is for debug reasons!
    [SerializeField] private int tileRow;
    [SerializeField] private int tileColumn;
    //Used to check if the tile is holding a X or O
    [SerializeField] private bool isHoldingAPiece = false;
    [SerializeField] private GameObject holdingPiece;
    public GameObject PieceData { get => holdingPiece; }
    public bool IsHoldingAPiece { get => isHoldingAPiece; set => isHoldingAPiece = value; } 

    public void SetTilePos(int r, int c)
    {
        tileRow = r;
        tileColumn = c;
    }


    public void SetHoldingPiece(GameObject holdingPiece)
    {
        this.holdingPiece = holdingPiece;
    }
}
