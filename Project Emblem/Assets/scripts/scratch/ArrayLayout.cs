using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ArrayLayout
{
    [System.Serializable]
    public struct RowData
    {
        public GameObject[] column;
    }

    public RowData[] rows;
}
