using UnityEngine;
using System.Collections.Generic;
using Unity.VisualScripting;

public class PieceManager : MonoBehaviour
{
    public GameObject piecePrefab; // 駒の設計図
    public BoardGenerator board;    // 盤面情報

    // 駒の名簿
    private GameObject[,] pieceBoard = new GameObject[9, 9];

    // 駒を生成して配置する関数
    public void PlacePiece(int x, int y, bool isSente, string type)
    {
        // 生成
        GameObject pObj = Instantiate(piecePrefab);

        // データのセット
        Piece p = pObj.GetComponent<Piece>();
        p.isSente = isSente;
        p.type = type;

        // 見た目の反映
        p.SetPosition(x, y, board.interval, board.startPos);

        // 名簿（配列）に登録
        pieceBoard[x, y] = pObj;

        
    }
    void Start()
    {
        PlacePiece(4, 4, true, "FU");
    }
}