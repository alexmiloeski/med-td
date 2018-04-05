using UnityEngine;

public class GenerateTiles : MonoBehaviour
{
    public int boardSizeX;
    public int boardSizeY;
    public Transform tilePrefab;

    // Use this for initialization
    void Start()
    {
        //Renderer rend = tilePrefab.GetComponent<Renderer>();
        //Vector3 size = rend.bounds.size;
        BoxCollider2D coll = tilePrefab.GetComponent<BoxCollider2D>();
        Vector3 size = coll.bounds.size;
        float width = 1;// size.x;
        float height = 1;//size.y;
        Debug.Log(width + " x " + height);

        int halfBoardWidth = boardSizeX / 2;
        int halfBoardHeight = boardSizeY / 2;

        GameObject board = new GameObject();
        board.name = "NewBoard";
        for (int x = -halfBoardWidth; x < halfBoardWidth; x++)
        {
            for (int y = -halfBoardHeight; y < halfBoardHeight; y++)
            {
                Transform tile = (Transform)Instantiate(tilePrefab, new Vector3(x*width, y*height, 0), Quaternion.identity);
                tile.name = "Tile_" + (x+halfBoardWidth) + "_" + (y + halfBoardHeight);
                tile.parent = board.transform;
            }
        }
    }
}
