using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class threeDGrid : MonoBehaviour
{
    [SerializeField] GameObject blankCube;

    List<List<List<GameObject>>> grid3D;

    private void Start()
    {
        grid3D = new List<List<List<GameObject>>> ();

        for (int i = 0; i < 10; i++)
        {
            List<List<GameObject>> grid = new List<List<GameObject>> ();
            for (int j = 0; j < 10; j++)
            {
                List<GameObject> singleRow = new List<GameObject> ();
                for (int k = 0; k < 10; k++)
                {
                    GameObject block = Instantiate(blankCube, new Vector3(k, -i, -j), Quaternion.identity);
                    singleRow.Add (block);
                }

                grid.Add (singleRow);
            }

            grid3D.Add(grid);
        }
    }
}
