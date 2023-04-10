using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

struct LowestEntropyInfo
{
    public Cell cell;
    public int gridX;
    public int gridY;

    public LowestEntropyInfo(Cell cell, int x, int y)
    {
        this.cell = cell;
        this.gridX = x;
        this.gridY = y;
    }
}

[System.Serializable]
public class Tile
{
    public GameObject tile;
    public List<int> edges;

    public List<int> up = new List<int>();
    public List<int> right = new List<int>();
    public List<int> down = new List<int>();
    public List<int> left = new List<int>();

    public Tile(GameObject tile, List<int> edges)
    {
        this.tile = tile;
        this.edges = edges;
    }

    public static void GenerateTileRules(List<Tile> allTiles)
    {
        for (int i = 0; i < allTiles.Count; i++)
        {
            Tile tile1 = allTiles[i];

            for (int j = 0; j < allTiles.Count; j++)
            {
                Tile tile2 = allTiles[j];

                if (tile1.edges[0] == tile2.edges[2])
                {
                    tile1.up.Add(j);
                }

                if (tile1.edges[1] == tile2.edges[3])
                {
                    tile1.right.Add(j);
                }

                if (tile1.edges[2] == tile2.edges[0])
                {
                    tile1.down.Add(j);
                }

                if (tile1.edges[3] == tile2.edges[1])
                {
                    tile1.left.Add(j);
                }
            }
        }
    }

    public static List<int> ReturnValidOptions(List<int> cellOptions, List<int> constraintOptions)
    {
        List<int> newValidOptions = new List<int>();

        for (int i = 0; i < cellOptions.Count; i++)
        {
            bool found = false;
            for (int j = 0; j < constraintOptions.Count; j++)
            {
                if (cellOptions[i] == constraintOptions[j])
                {
                    found = true;
                    break;
                }
            }

            if (found)
            {
                newValidOptions.Add(cellOptions[i]);
            }
        }
        return newValidOptions;
    }
}

public class GridCreator : MonoBehaviour
{
    [SerializeField] GameObject Block0;

    [SerializeField] GameObject Block1;
    [SerializeField] GameObject Block11;
    [SerializeField] GameObject Block111;
    [SerializeField] GameObject Block1111;

    [SerializeField] GameObject Block2;
    [SerializeField] GameObject Block22;
    [SerializeField] GameObject Block222;
    [SerializeField] GameObject Block2222;

    [SerializeField] GameObject Block3;
    [SerializeField] GameObject Block33;

    [SerializeField] GameObject Block4;
    [SerializeField] GameObject Block44;
    [SerializeField] GameObject Block444;
    [SerializeField] GameObject Block4444;

    [SerializeField] GameObject Block5;
    [SerializeField] GameObject Block55;
    [SerializeField] GameObject Block555;
    [SerializeField] GameObject Block5555;
    

    // list of materials that can be swap out incase of a match
    //[SerializeField] List<Material> quadmaterials = new List<Material>();

    [SerializeField] List<Tile> tiles = new List<Tile>();

    int ROW = 20;
    int COL = 20;

    List<List<Cell>> grid = new List<List<Cell>>();
    List<List<GameObject>> gridGO = new List<List<GameObject>> ();
    List<GameObject> tilesGO;

    bool progress = true;
    void Start()
    {
        // just checking this code should have different tileset than the default tileset.
        // we have total of 5 tiles currently
 
        tiles.Add(new Tile(Block0, new List<int>() { 0, 0, 0, 0}));

        tiles.Add(new Tile(Block1, new List<int>() { 1, 1, 0, 1}));
        tiles.Add(new Tile(Block11, new List<int>() { 1, 1, 1, 0}));
        tiles.Add(new Tile(Block111, new List<int>() { 0, 1, 1, 1}));
        tiles.Add(new Tile(Block1111, new List<int>() { 1, 0, 1, 1}));

        tiles.Add(new Tile(Block2, new List<int>() { 0, 0, 1, 0}));
        tiles.Add(new Tile(Block22, new List<int>() { 0, 0, 0, 1}));
        tiles.Add(new Tile(Block222, new List<int>() { 1, 0, 0, 0}));
        tiles.Add(new Tile(Block2222, new List<int>() { 0, 1, 0, 0}));

        tiles.Add(new Tile(Block3, new List<int>() { 0, 1, 0, 1}));
        tiles.Add(new Tile(Block33, new List<int>() { 1, 0, 1, 0}));
     

        tiles.Add(new Tile(Block4, new List<int>() { 1, 2, 1, 1}));
        tiles.Add(new Tile(Block44, new List<int>() { 1, 1, 2, 1}));
        tiles.Add(new Tile(Block444, new List<int>() { 1, 1, 1, 2}));
        tiles.Add(new Tile(Block4444, new List<int>() { 2, 1, 1, 1}));

        tiles.Add(new Tile(Block5, new List<int>() { 0, 0, 1, 1}));
        tiles.Add(new Tile(Block55, new List<int>() { 1, 0, 0, 1}));
        tiles.Add(new Tile(Block555, new List<int>() { 1, 1, 0, 0}));
        tiles.Add(new Tile(Block5555, new List<int>() { 0, 1, 1, 0}));
      

        tilesGO = new List<GameObject>() { Block0, Block1, Block11, Block111, Block1111, Block2, Block22, Block222, Block2222, Block3, Block33, Block4, Block44, Block444, Block4444, Block5, Block55, Block555, Block5555};

        Tile.GenerateTileRules(tiles);

        GenerateGrid();

        //Camera.main.transform.position = new Vector3(((float)ROW - 1)/2, 10, -7);

        // initialize an empty ROW X COL grid
        for (int i = 0; i < ROW; i++)
        {
            List<Cell> row = new List<Cell>();
            for (int j = 0; j < COL; j++)
            {
                row.Add(new Cell());
            }
            grid.Add(row);
        }

        grid[2][2].options = new List<int>() {3};
    }

    void Update()
    {
        int lowestEntropy = FindCellWithLowestEntropy();

        if (lowestEntropy < 0 || lowestEntropy > 30)
        {
           return;
        }
        // find all such cells that have the lowest entropy
        List<LowestEntropyInfo> lowestEntropyCells = FindAllCellWithLowestEntropy(lowestEntropy);

        ChooseRandomCellAndCollapse(lowestEntropyCells);        
    }

    private int FindCellWithLowestEntropy()
    {
        float lowestEntropy = Mathf.Infinity;

        for (int i = 0; i < ROW; i++)
        {
            for (int j = 0; j < COL; j++)
            {
                Cell cell = grid[i][j];

                if (!cell.collapsed)
                {
                    if (cell.options.Count < lowestEntropy)
                    {
                        lowestEntropy = cell.options.Count;
                    }
                }
            }
        }

        return (int)lowestEntropy;
    }

    private List<LowestEntropyInfo> FindAllCellWithLowestEntropy(int entropy)
    {
        List<LowestEntropyInfo> lowestEntropyCells = new List<LowestEntropyInfo>();

        for (int i = 0; i < ROW; i++)
        {
            for (int j = 0; j < COL; j++)
            {
                Cell cell = grid[i][j];

                if (!cell.collapsed)
                {
                    if (cell.options.Count == entropy) { 
                        lowestEntropyCells.Add(new LowestEntropyInfo(cell, i, j));
                    }
                }
            }
        }
        return lowestEntropyCells;
    }

    private void PrintList(List<int> list)
    {
        foreach (int i in list)
        {
            Debug.Log(i);
        }

        Debug.Log("---------");
    }

    private void ChooseRandomCellAndCollapse(List<LowestEntropyInfo> allLowestEntropy)
    {
        int randomIndex = UnityEngine.Random.Range(0, allLowestEntropy.Count);

        // choose a random cell from the cells of lowest entropy
        LowestEntropyInfo lowestEntropy = allLowestEntropy[randomIndex];

        // collapse that cell
        lowestEntropy.cell.collapsed = true;

        // since that cell is collapsed it only has one option
        if (lowestEntropy.cell.options.Count == 0) return;
        //Debug.Log(lowestEntropy.cell.options.Count);

        lowestEntropy.cell.options = new List<int>() { lowestEntropy.cell.options[UnityEngine.Random.Range(0, lowestEntropy.cell.options.Count)]};

        // since it is collapsed make it visible
        //gridGO[lowestEntropy.gridX][lowestEntropy.gridY].SetActive(true);

        // change its material to the option that it has chosen
        SwapQuadMat swapQuadMatScript = gridGO[lowestEntropy.gridX][lowestEntropy.gridY].GetComponent<SwapQuadMat>();
        //swapQuadMatScript.SwapMat(quadmaterials[lowestEntropy.cell.options[0]]);
        Destroy(gridGO[lowestEntropy.gridX][lowestEntropy.gridY]);

        GameObject tileGameObject = Instantiate(tilesGO[lowestEntropy.cell.options[0]], new Vector3(2 * lowestEntropy.gridY, 0, -2 * lowestEntropy.gridX), Quaternion.identity);
        tileGameObject.transform.SetParent(transform);
        gridGO[lowestEntropy.gridX][lowestEntropy.gridY] = tileGameObject;



        // now decrease the entropy of the nearby surrounding it.
        int randomRow = lowestEntropy.gridX;
        int randomCol = lowestEntropy.gridY;

        // decrease the entropy of the cells above the chosen cell
        if (randomRow > 0)
        {
            Cell cellAbove = grid[randomRow - 1][randomCol];

            if (!cellAbove.collapsed)
            {
                List<int> cellOptions = cellAbove.options;
                List<int> constraintOptions = tiles[lowestEntropy.cell.options[0]].up;
                cellAbove.options = Tile.ReturnValidOptions(cellOptions, constraintOptions);
            }
        }

        if (randomRow < ROW - 1)
        {
            Cell cellDown = grid[randomRow + 1][randomCol];


            if (!cellDown.collapsed)
            {
                List<int> cellOptions = cellDown.options;
                List<int> constraintOptions = tiles[lowestEntropy.cell.options[0]].down;

                cellDown.options = Tile.ReturnValidOptions(cellOptions, constraintOptions);
            }
        }
        if (randomCol > 0)
        {
            Cell cellLeft = grid[randomRow][randomCol - 1];

            if (!cellLeft.collapsed)
            {
                List<int> cellOptions = cellLeft.options;
                List<int> constraintOptions = tiles[lowestEntropy.cell.options[0]].left;

                cellLeft.options = Tile.ReturnValidOptions(cellOptions, constraintOptions);
            }
        }
        if (randomCol < COL - 1)
        {
            Cell cellRight = grid[randomRow][randomCol + 1];

            if (!cellRight.collapsed)
            {
                List<int> cellOptions = cellRight.options;
                List<int> constraintOptions = tiles[lowestEntropy.cell.options[0]].right;

                cellRight.options = Tile.ReturnValidOptions(cellOptions, constraintOptions);
            }
        }
    }

    private void GenerateGrid()
    {
        for (int i = 0; i < ROW; i++)
        {
            List<GameObject> gridGORow = new List<GameObject>();
            for (int j = 0; j < COL; j++)
            {
                //GameObject tile = null;

                GameObject tile = Instantiate(Block0, new Vector3(j, 0, -i), Quaternion.identity);
                tile.SetActive(false);
                tile.transform.SetParent(transform);

                gridGORow.Add(tile);
            }
            gridGO.Add(gridGORow);
        }
    }
}
