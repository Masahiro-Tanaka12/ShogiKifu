using UnityEngine;

public class BoardGenerator : MonoBehaviour
{
    public GameObject squarePrefab;
    public float cellSize;
    public Vector3 startPos;
    public float interval;

    void Start()
    {
        GenerateBoard();
    }
    void GenerateBoard()
    {
        for (int x = 0; x < 9; x++)
        {
            for (int y = 0; y < 9; y++)
            {
                GameObject square = Instantiate(squarePrefab);

                float posX = startPos.x + (x * interval);
                float posY = startPos.y + (y * interval);

                square.transform.position = new Vector3(posX, posY, 0);
                square.transform.SetParent(transform);
            }
        }
    }
}
    
    
