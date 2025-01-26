using System.Collections.Generic;
using System.Globalization;
using System.IO;
using UnityEngine;

namespace Test
{
    public class ObjParser
    {
        /// <summary>
        /// Преобразует OBJ файл в Unity Mesh.
        /// </summary>
        /// <param name="filePath">Путь до OBJ файла.</param>
        /// <returns>Unity Mesh объект, собранный из OBJ file.</returns>
        public static Mesh Parse(string filePath)
        {
            // Проверка наличия файла.
            if (!File.Exists(filePath))
            {
                Debug.LogError($"File not found: {filePath}");
                return null;
            }

            // Списки для хранения необработанных данных о вершинах, нормалях и UV из файла OBJ.
            List<Vector3> vertices = new List<Vector3>();
            List<Vector3> normals = new List<Vector3>();
            List<Vector2> uv = new List<Vector2>();
            List<int> triangles = new List<int>();

            // Кэш для хранения уникальных комбинаций вершин (vertex, UV, normal).
            Dictionary<string, int> vertexCache = new Dictionary<string, int>();

            // Окончательные списки данных сетки, учитывающие уникальные комбинации вершин.
            List<Vector3> finalVertices = new List<Vector3>();
            List<Vector3> finalNormals = new List<Vector3>();
            List<Vector2> finalUVs = new List<Vector2>();

            try
            {
                string[] lines = File.ReadAllLines(filePath);

                foreach (string line in lines)
                {
                    // Обрезание пробелов и пропуск вустых строк или комментариев.
                    string trimmedLine = line.Trim();

                    if (string.IsNullOrEmpty(trimmedLine) || trimmedLine.StartsWith("#"))
                        continue;

                    // Разделение строки на компоненты.
                    string[] parts = trimmedLine.Split(' ');

                    switch (parts[0])
                    {
                        case "v": // Положение вершин.
                            vertices.Add(ParseVector3(parts));
                            break;
                        case "vt": // UV координаты.
                            uv.Add(ParseVector2(parts));
                            break;
                        case "vn": // Вектора нормалей.
                            normals.Add(ParseVector3(parts));
                            break;
                        case "f": // Грани.
                            ParseFace(parts, triangles, vertices, uv, normals, vertexCache, finalVertices, finalUVs,
                                finalNormals);
                            break;
                    }
                }

                // Создание и заполнение Unity Mesh.
                Mesh mesh = new Mesh
                {
                    vertices = finalVertices.ToArray(),
                    triangles = triangles.ToArray()
                };

                // Подключение UV координат, если доступны.
                if (finalUVs.Count > 0)
                    mesh.uv = finalUVs.ToArray();

                // Подлкючение нормалей, если доступны, иначе их вычисление.
                if (finalNormals.Count > 0)
                    mesh.normals = finalNormals.ToArray();
                else
                    mesh.RecalculateNormals();

                // Вычисление границы сетки для повышения производительности рендеринга.
                mesh.RecalculateBounds();

                return mesh;
            }
            catch (System.Exception ex)
            {
                Debug.LogError($"Error parsing OBJ file: {ex.Message}");
                return null;
            }
        }

        /// <summary>
        /// Парсит vector3 из массива строк.
        /// </summary>
        private static Vector3 ParseVector3(string[] parts)
        {
            float x = float.Parse(parts[1], CultureInfo.InvariantCulture);
            float y = float.Parse(parts[2], CultureInfo.InvariantCulture);
            float z = float.Parse(parts[3], CultureInfo.InvariantCulture);
            return new Vector3(x, y, z);
        }

        /// <summary>
        /// Парсит vector2 из массива строк.
        /// </summary>
        private static Vector2 ParseVector2(string[] parts)
        {
            float x = float.Parse(parts[1], CultureInfo.InvariantCulture);
            float y = float.Parse(parts[2], CultureInfo.InvariantCulture);
            return new Vector2(x, y);
        }

        /// <summary>
        /// Парсит грани и заполняет индексы треугольников.
        /// </summary>
        private static void ParseFace(string[] parts, List<int> triangles, List<Vector3> vertices, List<Vector2> uv,
            List<Vector3> normals,
            Dictionary<string, int> vertexCache, List<Vector3> finalVertices, List<Vector2> finalUVs,
            List<Vector3> finalNormals)
        {
            // Триангуляция грани (предполагается, что грань состоит из треугольников).
            for (int i = 1; i < parts.Length - 2; i++)
            {
                triangles.Add(ParseFaceVertex(parts[1], vertices, uv, normals, vertexCache, finalVertices, finalUVs,
                    finalNormals));
                triangles.Add(ParseFaceVertex(parts[i + 1], vertices, uv, normals, vertexCache, finalVertices, finalUVs,
                    finalNormals));
                triangles.Add(ParseFaceVertex(parts[i + 2], vertices, uv, normals, vertexCache, finalVertices, finalUVs,
                    finalNormals));
            }
        }

        /// <summary>
        /// Парсит вершины грани, включая ее позицию, UV, и индексы нормалей.
        /// </summary>
        private static int ParseFaceVertex(string facePart, List<Vector3> vertices, List<Vector2> uv,
            List<Vector3> normals,
            Dictionary<string, int> vertexCache, List<Vector3> finalVertices, List<Vector2> finalUVs,
            List<Vector3> finalNormals)
        {
            // Проверка, была ли комбинация вершин уже кэширована.
            if (vertexCache.TryGetValue(facePart, out int index))
            {
                return index;
            }

            // Разделение части грани на положение/UV/индекс нормали.
            string[] indices = facePart.Split('/');

            // Парсинг индекса вершины (всегда существует).
            int vertexIndex = int.Parse(indices[0], CultureInfo.InvariantCulture) - 1;
            Vector3 vertex = vertices[vertexIndex];

            // Парсинг индекса UV, если он существует.
            Vector2 uvCoord = Vector2.zero;
            if (indices.Length > 1 && !string.IsNullOrEmpty(indices[1]))
            {
                int uvIndex = int.Parse(indices[1], CultureInfo.InvariantCulture) - 1;
                uvCoord = uv[uvIndex];
            }

            // Парсинг индекса нормали, если он существует.
            Vector3 normal = Vector3.zero;
            if (indices.Length > 2 && !string.IsNullOrEmpty(indices[2]))
            {
                int normalIndex = int.Parse(indices[2], CultureInfo.InvariantCulture) - 1;
                normal = normals[normalIndex];
            }

            // Добавление новой уникальной комбинации вершин в кэш.
            index = finalVertices.Count;
            vertexCache[facePart] = index;
            finalVertices.Add(vertex);
            finalUVs.Add(uvCoord);
            finalNormals.Add(normal);

            return index;
        }
    }
}