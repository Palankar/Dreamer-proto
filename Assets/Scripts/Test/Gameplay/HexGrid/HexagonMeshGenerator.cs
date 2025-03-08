using UnityEngine;

namespace Test
{
    /**
     * Генератор гексагона для ячейки координатной сетки.
     */
    [RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
    public class HexagonMeshGenerator : MonoBehaviour
    {
        public float outerRadius  = 1f; // Радиус гексагона (расстояние от центра до угла)
        public float innerRadius = 0.8f; // Внутренний радиус (пустая часть)
        public Material hexMaterial; // Материал гексагона

        private void Start()
        {
            GenerateHexagon();

            if (hexMaterial != null)
            {
                GetComponent<MeshRenderer>().material = hexMaterial;
            }
        }

        void GenerateHexagon()
        {
            // Создаем новый Mesh
            Mesh mesh = new Mesh();
            GetComponent<MeshFilter>().mesh = mesh;

            // Параметры гексагона
            int verticesCount = 12; // 6 внешних + 6 внутренних

            // Вершины
            Vector3[] vertices = new Vector3[verticesCount];
            vertices[0] = Vector3.zero; // Центр гексагона

            // Внешние вершины
            for (int i = 0; i < 6; i++)
            {
                float angle = Mathf.Deg2Rad * (60 * i - 30); // Смещение на -30° для выравнивания
                vertices[i] = new Vector3(
                    Mathf.Cos(angle) * outerRadius,
                    0,
                    Mathf.Sin(angle) * outerRadius
                );
            }

            // Внутренние вершины
            for (int i = 0; i < 6; i++)
            {
                float angle = Mathf.Deg2Rad * (60 * i - 30); // Те же углы
                vertices[i + 6] = new Vector3(
                    Mathf.Cos(angle) * innerRadius,
                    0,
                    Mathf.Sin(angle) * innerRadius
                );
            }

            // Треугольники
            int[] triangles = new int[36]; // 6 внешних + 6 внутренних треугольников
            for (int i = 0; i < 6; i++)
            {
                // Внешние треугольники
                triangles[i * 6] = i;
                triangles[i * 6 + 1] = i + 6;
                triangles[i * 6 + 2] = (i + 1) % 6;

                // Внутренние треугольники
                triangles[i * 6 + 3] = i + 6;
                triangles[i * 6 + 4] = (i + 1) % 6 + 6;
                triangles[i * 6 + 5] = (i + 1) % 6;
            }

            // UV-координаты (для текстурирования)
            Vector2[] uv = new Vector2[verticesCount];
            for (int i = 0; i < 6; i++)
            {
                uv[i] = new Vector2((vertices[i].x / outerRadius + 1) / 2, (vertices[i].z / outerRadius + 1) / 2);
                uv[i + 6] = new Vector2((vertices[i + 6].x / outerRadius + 1) / 2, (vertices[i + 6].z / outerRadius + 1) / 2);
            }

            // Присваиваем данные Mesh
            mesh.vertices = vertices;
            mesh.triangles = triangles;
            mesh.uv = uv;

            // Пересчет нормалей
            mesh.RecalculateNormals();

            // Опционально: добавляем коллайдер
            MeshCollider collider = gameObject.AddComponent<MeshCollider>();
            collider.sharedMesh = mesh;
        }
    }
}
//TODO: вернемся к полному гексу, без пустого центра. Но дадим ему цветную границу шейдером - обводку. При выборе можно менять цвет обводки.