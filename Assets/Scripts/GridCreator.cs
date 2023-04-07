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
    [SerializeField] GameObject BLANK;
    [SerializeField] GameObject UP;
    [SerializeField] GameObject RIGHT;
    [SerializeField] GameObject DOWN;
    [SerializeField] GameObject LEFT;

    // list of materials that can be swap out incase of a match
    [SerializeField] List<Material> quadmaterials = new List<Material>();

    [SerializeField] List<Tile> tiles = new List<Tile>();

    int ROW = 10;
    int COL = 10;

    List<List<Cell>> grid = new List<List<Cell>>();
    List<List<GameObject>> gridGO = new List<List<GameObject>> ();

    bool progress = true;
    void Start()
    {
        // we have total of 5 tiles currently
 
        tiles.Add(new Tile(BLANK, new List<int>() { 0, 0, 0, 0}));
        tiles.Add(new Tile(UP, new List<int>() { 1, 1, 0, 1 }));
        tiles.Add(new Tile(RIGHT, new List<int>() { 1, 1, 1, 0 }));
        tiles.Add(new Tile(DOWN, new List<int>() { 0, 1, 1, 1}));
        tiles.Add(new Tile(LEFT, new List<int>() { 1, 0, 1, 1 }));

        Tile.GenerateTileRules(tiles);

        GenerateGrid();

        Camera.main.transform.position = new Vector3(((float)ROW - 1)/2, 40, -7);

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

        grid[2][2].options = new List<int>() {2};
    }

    void Update()
    {
        int lowestEntropy = FindCellWithLowestEntropy();

        if (lowestEntropy < 0 || lowestEntropy > 5)
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
        lowestEntropy.cell.options = new List<int>() { lowestEntropy.cell.options[UnityEngine.Random.Range(0, lowestEntropy.cell.options.Count)]};

        // since it is collapsed make it visible
        gridGO[lowestEntropy.gridX][lowestEntropy.gridY].SetActive(true);

        // change its material to the option that it has chosen
        SwapQuadMat swapQuadMatScript = gridGO[lowestEntropy.gridX][lowestEntropy.gridY].GetComponent<SwapQuadMat>();
        swapQuadMatScript.SwapMat(quadmaterials[lowestEntropy.cell.options[0]]);

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
                GameObject tile = Instantiate(BLANK, new Vector3(j, 0, -i), Quaternion.identity);
                tile.SetActive(false);
                tile.transform.SetParent(transform);

                gridGORow.Add(tile);
            }
            gridGO.Add(gridGORow);
        }
    }
}
