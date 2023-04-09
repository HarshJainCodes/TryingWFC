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
    [SerializeField] GameObject Plane1;
    [SerializeField] GameObject Plane2;
    [SerializeField] GameObject Plane3;
    [SerializeField] GameObject Plane4;
    [SerializeField] GameObject Plane5;
    [SerializeField] GameObject Plane6;
    [SerializeField] GameObject Plane7;
    [SerializeField] GameObject Plane8;
    [SerializeField] GameObject Plane9;
    [SerializeField] GameObject Plane10;
    [SerializeField] GameObject Plane11;
    [SerializeField] GameObject Plane12;
    [SerializeField] GameObject Plane13;
    [SerializeField] GameObject Plane14;
    [SerializeField] GameObject Plane15;
    [SerializeField] GameObject Plane16;
    [SerializeField] GameObject Plane17;

    // list of materials that can be swap out incase of a match
    //[SerializeField] List<Material> quadmaterials = new List<Material>();

    [SerializeField] List<Tile> tiles = new List<Tile>();

    int ROW = 30;
    int COL = 30;

    List<List<Cell>> grid = new List<List<Cell>>();
    List<List<GameObject>> gridGO = new List<List<GameObject>> ();
    List<GameObject> tilesGO;

    bool progress = true;
    void Start()
    {
        // just checking this code should have different tileset than the default tileset.
        // we have total of 5 tiles currently
 
        tiles.Add(new Tile(Plane1, new List<int>() { 1, 0, 2, 0}));
        tiles.Add(new Tile(Plane2, new List<int>() { 1, 1, 1, 1 }));
        tiles.Add(new Tile(Plane3, new List<int>() { 0, 1, 0, 2 }));
        tiles.Add(new Tile(Plane4, new List<int>() { 2, 0, 1, 0}));
        tiles.Add(new Tile(Plane5, new List<int>() { 3, 4, 3, 1 }));
        tiles.Add(new Tile(Plane6, new List<int>() { 2, 4, 2, 4 }));
        tiles.Add(new Tile(Plane7, new List<int>() { 2, 5, 2, 5 }));
        tiles.Add(new Tile(Plane8, new List<int>() { 1, 0, 2, 0 }));
        tiles.Add(new Tile(Plane9, new List<int>() { 2, 2, 2, 5 }));
        tiles.Add(new Tile(Plane10, new List<int>() { 2, 2, 5, 2 }));
        tiles.Add(new Tile(Plane11, new List<int>() { 3, 2, 2, 3 }));
        tiles.Add(new Tile(Plane12, new List<int>() { 0, 0, 2, 2 }));
        tiles.Add(new Tile(Plane13, new List<int>() { 2, 3, 0, 2 }));
        tiles.Add(new Tile(Plane14, new List<int>() { 2, 2, 3, 0 }));
        tiles.Add(new Tile(Plane15, new List<int>() { 0, 1, 0, 4 }));
        tiles.Add(new Tile(Plane16, new List<int>() { 4, 3, 1, 3 }));
        tiles.Add(new Tile(Plane17, new List<int>() { 1, 0, 4, 0 }));

        tilesGO = new List<GameObject>() { Plane1, Plane2, Plane3, Plane4, Plane5, Plane6, Plane7, Plane8, Plane9, Plane10, Plane11, Plane12, Plane13, Plane14, Plane15, Plane16, Plane17};  

        Tile.GenerateTileRules(tiles);

        GenerateGrid();

        Camera.main.transform.position = new Vector3(((float)ROW - 1)/2, 10, -7);

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

        grid[2][2].options = new List<int>() {0};
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
        if (lowestEntropy.cell.options.Count == 0) return;
        Debug.Log(lowestEntropy.cell.options.Count);

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

                GameObject tile = Instantiate(Plane1, new Vector3(j, 0, -i), Quaternion.identity);
                tile.SetActive(false);
                tile.transform.SetParent(transform);

                gridGORow.Add(tile);
            }
            gridGO.Add(gridGORow);
        }
    }
}
