using UnityEngine;
using System.Collections.Generic;

public class PieceManager : MonoBehaviour
{
    [Header("Settings")]
    public GameObject piecePrefab;
    public BoardGenerator board;
    public TextAsset kifuFile;

    [Header("Piece Sprites")]
    public List<PieceData> pieceImages;
    private Dictionary<string, Sprite> imageDict = new Dictionary<string, Sprite>();

    // 内部データ
    private GameObject[,] pieceBoard = new GameObject[9, 9];
    private List<MoveData> kifuList = new List<MoveData>();
    private int currentMoveIndex = 0;

    [System.Serializable]
    public struct PieceData
    {
        public string name;
        public Sprite image;
    }

    [System.Serializable]
    public struct MoveData
    {
        public int fromX, fromY, toX, toY;
    }

    [System.Serializable]
    public struct MoveHistory
    {
        public int fromX, fromY;
        public int toX, toY;
        public GameObject takenPiece;
    }

    private List<MoveHistory> historyList = new List<MoveHistory>();

    void Awake()
    {
        InitializeDictionary();
        LoadKifu();
    }

    void Start()
    {
        SpawnInitialLayout();
    }

    // --- 初期化系 ---

    private void InitializeDictionary()
    {
        foreach (var data in pieceImages)
        {
            if (!imageDict.ContainsKey(data.name))
                imageDict.Add(data.name, data.image);
        }
    }

    private void SpawnInitialLayout()
    {
        string[] initialSetup = {
            "0,0,false,KY", "1,0,false,KE", "2,0,false,GN", "3,0,false,KN", "4,0,false,OU", "5,0,false,KN", "6,0,false,GN", "7,0,false,KE", "8,0,false,KY",
            "7,1,false,HI", "1,1,false,KK",
            "0,2,false,FU", "1,2,false,FU", "2,2,false,FU", "3,2,false,FU", "4,2,false,FU", "5,2,false,FU", "6,2,false,FU", "7,2,false,FU", "8,2,false,FU",
            "0,6,true,EFU", "1,6,true,EFU", "2,6,true,EFU", "3,6,true,EFU", "4,6,true,EFU", "5,6,true,EFU", "6,6,true,EFU", "7,6,true,EFU", "8,6,true,EFU",
            "1,7,true,EHI", "7,7,true,EKK",
            "0,8,true,EKY", "1,8,true,EKE", "2,8,true,EGN", "3,8,true,EKN", "4,8,true,EOU", "5,8,true,EKN", "6,8,true,EGN", "7,8,true,EKE", "8,8,true,EKY"
        };

        foreach (string line in initialSetup)
        {
            string[] s = line.Split(',');
            PlacePiece(int.Parse(s[0]), int.Parse(s[1]), bool.Parse(s[2]), s[3]);
        }
    }

    void LoadKifu()
    {
        if (kifuFile == null) return;

        string[] lines = kifuFile.text.Split(new[] { '\n', '\r' }, System.StringSplitOptions.RemoveEmptyEntries);
        foreach (string line in lines)
        {
            string[] s = line.Split(',');
            if (s.Length == 4)
            {
                kifuList.Add(new MoveData
                {
                    fromX = int.Parse(s[0]),
                    fromY = int.Parse(s[1]),
                    toX = int.Parse(s[2]),
                    toY = int.Parse(s[3])
                });
            }
        }
    }

    // --- 駒の操作系 ---

    public void PlacePiece(int x, int y, bool isSente, string type)
    {
        GameObject pObj = Instantiate(piecePrefab);
        Piece p = pObj.GetComponent<Piece>();

        p.isSente = isSente;
        p.type = type;

        if (imageDict.TryGetValue(type, out Sprite sp)) p.SetSprite(sp);

        p.SetPosition(x, y, board.interval, board.startPos);
        pieceBoard[x, y] = pObj;
    }

    public void MovePiece(int fromX, int fromY, int toX, int toY, bool isUndo = false)
    {
        GameObject targetPiece = pieceBoard[fromX, fromY];
        if (targetPiece == null) return;

        GameObject pieceAtToPos = pieceBoard[toX, toY];

        // 「戻る」操作ではない時だけ、履歴を記録する
        if (!isUndo)
        {
            MoveHistory history = new MoveHistory
            {
                fromX = fromX,
                fromY = fromY,
                toX = toX,
                toY = toY,
                takenPiece = pieceAtToPos // 消える駒をメモ！
            };
            historyList.Add(history);

            // 取られた駒を画面から消すのではなく「非表示」にする（後で復活させるため）
            if (pieceAtToPos != null) pieceAtToPos.SetActive(false);
        }

        // 名簿更新
        pieceBoard[toX, toY] = targetPiece;
        pieceBoard[fromX, fromY] = null;

        // 見た目の移動
        targetPiece.GetComponent<Piece>().SetPosition(toX, toY, board.interval, board.startPos);
    }

    public void OnNextButtonClick()
    {
        if (currentMoveIndex >= kifuList.Count) return;

        MoveData move = kifuList[currentMoveIndex];
        MovePiece(move.fromX, move.fromY, move.toX, move.toY);
        currentMoveIndex++;
    }
    public void OnBackButtonClick()
    {
        if (historyList.Count == 0) return; // 履歴がなければ何もしない

        // 1. 最後の履歴を取り出してリストから消す
        int lastIdx = historyList.Count - 1;
        MoveHistory lastMove = historyList[lastIdx];
        historyList.RemoveAt(lastIdx);

        // 2. 駒を逆方向に動かす (toX, toY -> fromX, fromY)
        // 引数に true を渡して、履歴に記録されないようにする
        MovePiece(lastMove.toX, lastMove.toY, lastMove.fromX, lastMove.fromY, true);

        // 3. もし取っていた駒があれば復活させる
        if (lastMove.takenPiece != null)
        {
            lastMove.takenPiece.SetActive(true);
            // 名簿（配列）にも戻してあげる
            pieceBoard[lastMove.toX, lastMove.toY] = lastMove.takenPiece;
        }

        // 4. 手数を戻す
        currentMoveIndex--;
    }
}