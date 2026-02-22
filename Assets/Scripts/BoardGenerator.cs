using UnityEngine;

// 9x9の盤面を生成するクラス
public class BoardGenerator : MonoBehaviour
{
    public GameObject squarePrefab; // 生成するマスのプレハブ
    public Vector3 startPos;        // 盤面の開始位置
    public float interval;          // マス間の間隔

    void Start()
    {
        GenerateBoard(); // ゲーム開始時に盤面を生成
    }

    // 盤面を生成する関数
    void GenerateBoard()
    {
        for (int x = 0; x < 9; x++)
        {
            for (int y = 0; y < 9; y++)
            {
                GameObject square = Instantiate(squarePrefab);

                // マスの位置を計算
                float posX = startPos.x + (x * interval);
                float posY = startPos.y + (y * interval);

                square.transform.position = new Vector3(posX, posY, 0);
                square.transform.SetParent(transform); // このオブジェクトの子にする
            }
        }
    }
}



