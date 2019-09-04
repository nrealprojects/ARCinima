using UnityEngine;
using System.Collections.Generic;

namespace NREAL.AR
{
    public class CircleGrid : MonoBehaviour
    {
        public float r = 1.0f;
        public float deltaAngle = 15f;

        [ContextMenu("Rank")]
        private void Rank()
        {
            // create positions
            float current = 0;
            List<Vector3> pos = new List<Vector3>();
            while (current < 360 - deltaAngle + 1)
            {
                Vector3 v = new Vector3(r * Mathf.Sin(Mathf.PI * current / 180), 0, r * Mathf.Cos(Mathf.PI * current / 180));
                pos.Add(v);
                current += deltaAngle;
            }

            int count = this.transform.childCount;
            for (int i = 0; i < count && i < pos.Count; i++)
            {
                Transform tran = this.transform.GetChild(i);
                tran.localPosition = pos[i];
            }
        }
    }
}