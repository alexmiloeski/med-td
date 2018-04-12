using UnityEngine;
//using System.Collections.Generic;

//public enum Direction { Up, Right, Down, Left };

public class GenerateTiles : MonoBehaviour
{
    public Transform tilePrefab;

    public int boardSizeX;
    public int boardSizeY;

    private bool[,] boardMap;
    private Transform[,] board;

    private void Start()
    {
        // initialize the map to all false
        boardMap = new bool[boardSizeX, boardSizeY];
        for (int i = 0; i < boardSizeX; i++)
        {
            for (int j = 0; j < boardSizeY; j++)
            {
                boardMap[i, j] = false;
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

        // set the coordinates manually
        boardMap[9, 10] = true;
        boardMap[9, 9] = true;
        boardMap[9, 8] = true;
        boardMap[9, 7] = true;

        boardMap[8, 7] = true;
        boardMap[7, 7] = true;
        boardMap[6, 7] = true;

        boardMap[6, 6] = true;
        boardMap[6, 5] = true;
        boardMap[6, 4] = true;

        boardMap[5, 4] = true;
        boardMap[4, 4] = true;
        boardMap[3, 4] = true;
        
        boardMap[3, 3] = true;
        boardMap[3, 2] = true;
        boardMap[3, 1] = true;

        ///////////////////////////////////////

        // create the tiles using the map
        GameObject boardContainer = new GameObject() { name = "NewBoard" };
        // i = 0 ~ x = i + 0.5 - (boardSizeX/2) // todo: don't need this 0.5 if it's an odd number!
        // i = 0 ~ x = -9.5
        // i = 1 ~ x = -8.5
        // i = 2 ~ x = -7.5
        // i = 9 ~ x = -0.5
        // i = 19 ~ x = 9.5

        // j = 0 ~ y = j + 0.5 - ((boardSizeY-1)/2)
        // j = 0 ~ y = -4.5
        // j = 1 ~ y = -3.5
        // j = 2 ~ y = -2.5
        // j = 3 ~ y = -1.5
        // j = 4 ~ y = -0.5
        // j = 5 ~ y = 0.5
        // j = 10 ~ y = 5.5
        for (int i = 0; i < boardSizeX; i++)
        {
            for (int j = 0; j < boardSizeY; j++)
            {
                if (boardMap[i, j])
                {
                    float x = i + 0.5f - (boardSizeX / 2f);
                    float y = j + 0.5f - ((boardSizeY - 1f) / 2f);
                    Transform tile = Instantiate(tilePrefab, new Vector3(x, y, 0), Quaternion.identity);
                    tile.name = "Tile_" + i + " _ " + j;
                    tile.parent = boardContainer.transform;
                    board[i, j] = tile;

                    LinkedNode ln = tile.GetComponent<LinkedNode>();

                    // left neighbor
                    if (i > 0 && board[i - 1, j] != null)
                    {
                        LinkedNode leftLN = board[i - 1, j].GetComponent<LinkedNode>();
                        ln.SetLeft(leftLN);
                        leftLN.SetRight(ln);
                    }
                    // right neighbor
                    if (i < boardSizeX-1 && board[i + 1, j] != null)
                    {
                        LinkedNode rightLN = board[i + 1, j].GetComponent<LinkedNode>();
                        ln.SetRight(rightLN);
                        rightLN.SetLeft(ln);
                    }
                    // above neighbor
                    if (j < boardSizeY - 1 && board[i, j + 1] != null)
                    {
                        LinkedNode aboveLN = board[i, j + 1].GetComponent<LinkedNode>();
                        ln.SetAbove(aboveLN);
                        aboveLN.SetBelow(ln);
                    }
                    // above neighbor
                    if (j > 0 && board[i, j - 1] != null)
                    {
                        LinkedNode belowLN = board[i, j - 1].GetComponent<LinkedNode>();
                        ln.SetBelow(belowLN);
                        belowLN.SetAbove(ln);
                    }
                }
            }
        }

        // link neighbors
        for (int i = 0; i < boardSizeX; i++)
        {
            for (int j = 0; j < boardSizeY; j++)
            {
                
            }
        }
    }
}
//public class GenerateTiles : MonoBehaviour
//{
//    public Transform tilePrefab;

//    //public int boardSizeX;
//    //public int boardSizeY;

//    //public Vector3 firstTilePos = new Vector3(-0.5f, 5.5f, 0f);
//    ////public List<Direction> directions;
//    ////public Direction[] directions = new Direction[] { Direction.Down, Direction.Down, Direction.Left };
//    //public int[] directions;
//    //public const int up = 1;
//    //public const int right = 2;
//    //public const int down = 3;
//    //public const int left = 4;
    
//    //private void Start()
//    //{
//    //    GameObject board = new GameObject() { name = "NewBoard" };

//    //    Transform tile = Instantiate(tilePrefab, firstTilePos, Quaternion.identity);
//    //    tile.name = "StartTile";
//    //    tile.parent = board.transform;


//    //    Debug.Log("directions.Length = " + directions.Length);
//    //    if (directions.Length == 0)
//    //    {
//    //        directions = new int[] { down, down, left, left, down, down, right, right };
//    //    }

//    //    // the size of each square is 1x1 Unity units
//    //    //float width = 1;
//    //    //float height = 1;

//    //    Vector3 prevTilePos = firstTilePos;
//    //    for (int i = 0; i < directions.Length; i++)
//    //    {
//    //        int dir = directions[i];

//    //        float x, y, z;
//    //        switch (dir)
//    //        {
//    //            case up:
//    //                x = prevTilePos.x;
//    //                y = prevTilePos.y + 1;
//    //                z = firstTilePos.z;
//    //                break;

//    //            case right:
//    //                x = prevTilePos.x + 1;
//    //                y = prevTilePos.y;
//    //                z = firstTilePos.z;
//    //                break;

//    //            case down:
//    //                x = prevTilePos.x;
//    //                y = prevTilePos.y - 1;
//    //                z = firstTilePos.z;
//    //                break;

//    //            case left:
//    //            default:
//    //                x = prevTilePos.x - 1;
//    //                y = prevTilePos.y;
//    //                z = firstTilePos.z;
//    //                break;
//    //        }
//    //        Vector3 nextTilePos = new Vector3(x, y, z);


//    //        tile = Instantiate(tilePrefab, nextTilePos, Quaternion.identity);
//    //        tile.name = "Tile_" + (i + 1);
//    //        tile.parent = board.transform;
//    //        //LinkedNode ln = tile.GetComponent<LinkedNode>();
//    //        //if (ln != null)
//    //        //{
//    //        //    ln.Set..
//    //        //}


//    //        prevTilePos = nextTilePos;
//    //    }
//    //}

//    //void Start()
//    //{
//    //    //Renderer rend = tilePrefab.GetComponent<Renderer>();
//    //    //Vector3 size = rend.bounds.size;
//    //    BoxCollider2D coll = tilePrefab.GetComponent<BoxCollider2D>();
//    //    Vector3 size = coll.bounds.size;
//    //    float width = 1;// size.x;
//    //    float height = 1;//size.y;
//    //    Debug.Log(width + " x " + height);

//    //    int halfBoardWidth = boardSizeX / 2;
//    //    int halfBoardHeight = boardSizeY / 2;

//    //    GameObject board = new GameObject();
//    //    board.name = "NewBoard";
//    //    for (int x = -halfBoardWidth; x < halfBoardWidth; x++)
//    //    {
//    //        for (int y = -halfBoardHeight; y < halfBoardHeight; y++)
//    //        {
//    //            Transform tile = (Transform)Instantiate(tilePrefab, new Vector3(x * width, y * height, 0), Quaternion.identity);
//    //            tile.name = "Tile_" + (x + halfBoardWidth) + "_" + (y + halfBoardHeight);
//    //            tile.parent = board.transform;
//    //        }
//    //    }
//    //}
//}
