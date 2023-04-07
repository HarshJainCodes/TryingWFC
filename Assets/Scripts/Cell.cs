using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Cell : MonoBehaviour
{
    public bool collapsed = false;

    // each cell in the grid starts with an all the possible options.
    public List<int> options = new List<int>() { 0, 1, 2, 3, 4};
}
