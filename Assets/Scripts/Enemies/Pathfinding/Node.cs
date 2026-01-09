using System.Collections.Generic;
using UnityEditor.ShaderGraph.Internal;
using UnityEngine;

public class Node
{
    public int X;
    public int Y;

    public bool T;
    public float C;
    public float F;
    public float G;
    public float H;

    public Node[] Neighbors;
    public Vector2Int previous;
    public bool InOpenSet;
    public bool InClosedSet;
}