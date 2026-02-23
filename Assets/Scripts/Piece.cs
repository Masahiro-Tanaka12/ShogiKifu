using UnityEngine;

public class Piece : MonoBehaviour
{
    public int x; // 0~8
    public int y; // 0~8
    public bool isSente; // 先手ならtrue, 後手ならfalse
    public string type;  // 駒の種類

    public void SetSprite(Sprite newSprite)
    {
        // 自分の SpriteRenderer を捕まえて画像を差し替える
        GetComponent<SpriteRenderer>().sprite = newSprite;
    }

    // 指定した座標に移動するメソッド
    public void SetPosition(int gridX, int gridY, float interval, Vector3 startPos)
    {
        x = gridX;
        y = gridY;

        float posX = startPos.x + (x * interval);
        float posY = startPos.y + (y * interval);

        transform.position = new Vector3(posX, posY, -1); // 盤面より手前に出すため
    }
}