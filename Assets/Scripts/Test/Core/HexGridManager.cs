using UnityEngine;

namespace Test
{
    public class HexGridManager : MonoBehaviour
    {
        public GameObject hexGrid;
        public GameObject hexPrefab; // Префаб гекса
        public int gridRadius = 5; // Радиус сетки
        public float hexSize = 1f; // Размер гекса

        void Start()
        {
            GenerateGrid();
        }

        void GenerateGrid()
        {
            for (int q = -gridRadius; q <= gridRadius; q++)
            {
                for (int r = Mathf.Max(-gridRadius, -q - gridRadius); r <= Mathf.Min(gridRadius, -q + gridRadius); r++)
                {
                    CreateHex(q, r);
                }
            }
        }

        void CreateHex(int q, int r)
        {
            Vector3 position = HexToWorld(q, r, hexSize);
            GameObject hex = Instantiate(hexPrefab, position, Quaternion.identity, transform);
            hex.GetComponent<HexTile>().Q = q;
            hex.GetComponent<HexTile>().R = r;
            hex.transform.SetParent(hexGrid.transform);
        }

        Vector3 HexToWorld(int q, int r, float hexSize)
        {
            float x = hexSize * Mathf.Sqrt(3) * (q + r / 2f);
            float z = hexSize * 3f / 2f * r;
            return new Vector3(x, 0, z);
        }
    }
}
