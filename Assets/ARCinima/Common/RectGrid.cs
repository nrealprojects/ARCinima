using System.Collections.Generic;
using UnityEngine;

public class RectGrid : MonoBehaviour
{
    public float Width = 10;
    public float Height = 10;
    public int Row = 2;
    public int Col = 3;


    [ContextMenu("Rank")]
    public void Rank()
    {
        // create positions
        int current = 0;
        Vector2 center = new Vector2((Col - 1) * Width * 0.5f, (Row - 1) * Height * 0.5f);
        List<Vector3> pos = new List<Vector3>();
        int num = Row * Col;
        while (current < num)
        {
            int r = current / Col;
            int c = current % Col;
            Vector3 v = new Vector3(c * Width - center.x, r * Height - center.y, 0);
            pos.Add(v);
            current++;
        }

        int count = this.transform.childCount;
        for (int i = 0; i < count && i < pos.Count; i++)
        {
            Transform tran = this.transform.GetChild(i);
            tran.localPosition = pos[count - i - 1];
        }
    }
}
