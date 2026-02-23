using UnityEngine;
using System.Collections.Generic;
using Unity.VisualScripting;

public class PieceManager : MonoBehaviour
{
    public GameObject piecePrefab; // 駒の設計図
    public BoardGenerator board;    // 盤面情報

    // 駒の名簿
    private GameObject[,] pieceBoard = new GameObject[9, 9];

    [System.Serializable]
    public struct PieceData
    {
        public string name;
        public Sprite image;
    }
    public List<PieceData> pieceImages;


    //　辞書
    private Dictionary<string, Sprite> imageDict = new Dictionary<string, Sprite>();


    // 初期配置用の構造体
    [System.Serializable]
    public struct InitialPiece
    {
        public int x;
        public int y;
        public bool isSente;
        public string type;
    }

    public List<InitialPiece> setupData; 

    void Awake()
    {
        // リストを辞書形式に変換しておく
        foreach (var data in pieceImages)
        {
            imageDict[data.name] = data.image;
        }
    }

    void Start()
    {
        // 文字列で初期配置を定義（x, y, 勢力, 種類）
        string[] initialSetup = {
        // --- 先手（isSente: false） ---
        "0,0,false,KY", "1,0,false,KE", "2,0,false,GI", "3,0,false,KI", "4,0,false,OU", "5,0,false,KI", "6,0,false,GI", "7,0,false,KE", "8,0,false,KY",
        "7,1,false,HI", "1,1,false,KK",
        "0,2,false,FU", "1,2,false,FU", "2,2,false,FU", "3,2,false,FU", "4,2,false,FU", "5,2,false,FU", "6,2,false,FU", "7,2,false,FU", "8,2,false,FU",

        // --- 後手（isSente: true） ---
        "0,6,true,EFU", "1,6,true,EFU", "2,6,true,EFU", "3,6,true,EFU", "4,6,true,EFU", "5,6,true,EFU", "6,6,true,EFU", "7,6,true,EFU", "8,6,true,EFU",
        "1,7,true,EHI", "7,7,true,EKK",
        "0,8,true,EKY", "1,8,true,EKE", "2,8,true,EGN", "3,8,true,EKN", "4,8,true,EOU", "5,8,true,EKN", "6,8,true,EGN", "7,8,true,EKE", "8,8,true,EKY"
    };

        foreach (string line in initialSetup)
        {
            string[] s = line.Split(',');
            int x = int.Parse(s[0]);
            int y = int.Parse(s[1]);
            bool isSente = bool.Parse(s[2]);
            string type = s[3];

            PlacePiece(x, y, isSente, type);
        }
    }

    // 駒を生成して配置する関数
    public void PlacePiece(int x, int y, bool isSente, string type)
    {
        // 生成
        GameObject pObj = Instantiate(piecePrefab);

        // データのセット
        Piece p = pObj.GetComponent<Piece>();
        p.isSente = isSente;
        p.type = type;

        // 名前に対応した画像をセット
        if (imageDict.ContainsKey(type))
        {
            p.SetSprite(imageDict[type]);
        }

        // 見た目の反映
        p.SetPosition(x, y, board.interval, board.startPos);

        // 名簿に登録
        pieceBoard[x, y] = pObj;

        
    }

    public void SetupBoard()
    {
        foreach (var data in setupData)
        {
            PlacePiece(data.x, data.y, data.isSente, data.type);
        }
    }

}