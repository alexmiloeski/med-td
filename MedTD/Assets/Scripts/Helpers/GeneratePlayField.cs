using UnityEngine;

public class GeneratePlayField : MonoBehaviour
{
    public Transform tilePrefab;

    public int boardSizeX;
    public int boardSizeY;

    private bool[,] boardMap;
    private Transform[,] board;

    public Vector2 startTile = new Vector2(9, 10);
    public int[] directions;
    private const int backToFork = -1;
    private const int fork = 0;
    private const int up = 1;
    private const int right = 2;
    private const int down = 3;
    private const int left = 4;

    private void Start()
    {
        // initialize the map to all false
        boardMap = new bool[boardSizeX, boardSizeY];
        for (int i = 0; i < boardSizeX; i++)
        {
            for (int j = 0; j < boardSizeY; j++)
            {
                boardMap[i, j] = true;
            }
        }
        // initialize the board to all null
        board = new Transform[boardSizeX, boardSizeY];
        for (int i = 0; i < boardSizeX; i++)
        {
            for (int j = 0; j < boardSizeY; j++)
            {
                board[i, j] = null;
            }
        }
        
        // set the directions manually
        if (directions.Length == 0)
        {
            directions = new int[] { down, down, down, left, left, down, left, left, left, down, down, down, down,
                left, left, down, down, right, right, right, right, right, up, up, up, fork, up, up, backToFork,
                right, right, right, right, right, fork, down, down, down, right, right, right, right, right, up,
                up, left, left, up, up, up, up, left, left, backToFork, up, up, up, up, left, left, up, up, up
            };
        }

        boardMap[(int)startTile.x, (int)startTile.y] = false;
        Vector2 prevTile = startTile;
        Vector2 forkTile = new Vector2();
        for (int i = 0; i < directions.Length; i++)
        {
            int dir = directions[i];

            float x, y;
            switch (dir)
            {
                case fork:
                default:
                    forkTile.x = prevTile.x;
                    forkTile.y = prevTile.y;
                    x = prevTile.x;
                    y = prevTile.y;
                    break;
                
                case backToFork:
                    x = forkTile.x;
                    y = forkTile.y;
                    break;

                case up:
                    x = prevTile.x;
                    y = prevTile.y + 1;
                    break;
                
                case right:
                    x = prevTile.x + 1;
                    y = prevTile.y;
                    break;
                
                case down:
                    x = prevTile.x;
                    y = prevTile.y - 1;
                    break;
                
                case left:
                    x = prevTile.x - 1;
                    y = prevTile.y;
                    break;
            }
            
            if (dir == backToFork)
            {
                prevTile = new Vector3(x, y);
            }
            else if (dir != fork)
            {
                boardMap[(int)x, (int)y] = false;
                
                prevTile = new Vector3(x, y);
            }
        }


        

        ///////////////////////////////////////

        // create the tiles using the map
        GameObject boardContainer = new GameObject() { name = "NewBoard" };
        for (int i = 0; i < boardSizeX; i++)
        {
            for (int j = 0; j < boardSizeY; j++)
            {
                if (boardMap[i, j])
                {
                    float x = i + 0.5f - (boardSizeX / 2f);
                    float y = j + 0.0f - ((boardSizeY - 1f) / 2f);
                    Transform tile = Instantiate(tilePrefab, new Vector3(x, y, 0), Quaternion.identity);
                    tile.name = "SSPoint_" + i + "_" + j;
                    tile.parent = boardContainer.transform;
                    board[i, j] = tile;
                }
            }
        }
    }
}