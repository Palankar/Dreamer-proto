using System.Collections.Generic;
using System.Globalization;
using System.IO;
using UnityEngine;

namespace Test
{
    public static class FilesLoader
    {
        public static Material LoadMaterialFromConfig(string configFilePath)
        {
            string fullPath = Path.Combine(Application.streamingAssetsPath, configFilePath);

            if (!File.Exists(fullPath))
            {
                Debug.LogError($"Material config file not found: {fullPath}");
                return null;
            }

            // Чтение конфигурации
            string jsonContent = File.ReadAllText(fullPath);
            MaterialConfig config = JsonUtility.FromJson<MaterialConfig>(jsonContent);

            // Создание материала
            Shader shader = Shader.Find(config.shader);
            if (shader == null)
            {
                Debug.LogError($"Shader not found: {config.shader}");
                return null;
            }

            Material material = new Material(shader);

            // Установка цвета
            Color color = new Color(config.color[0], config.color[1], config.color[2], config.color[3]);
            material.color = color;

            // Загрузка текстуры
            if (!string.IsNullOrEmpty(config.texture))
            {
                string texturePath = Path.Combine(Application.streamingAssetsPath, config.texture);
                if (File.Exists(texturePath))
                {
                    byte[] textureData = File.ReadAllBytes(texturePath);
                    Texture2D texture = new Texture2D(2, 2); // Создаем пустую текстуру
                    texture.LoadImage(textureData); // Загружаем изображение
                    material.mainTexture = texture;
                }
                else
                {
                    Debug.LogWarning($"Texture file not found: {texturePath}");
                }
            }

            return material;
        }

        public static Sprite LoadSpriteFromPath(string path)
        {
            if (File.Exists(path))
            {
                byte[] textureData = File.ReadAllBytes(path);
                Texture2D texture = new Texture2D(2, 2); // Создаем пустую текстуру
                texture.LoadImage(textureData); // Загружаем изображение
                return Sprite.Create(texture, new Rect(0.0f, 0.0f, texture.width, texture.height), Vector2.zero);
            }
            Debug.LogWarning($"Sprite file not found: {path}");
            return null;
        }

        public static Mesh LoadObjMeshFromPath(string path)
        {
            if (File.Exists(path))
            {
                // Списки для хранения данных из .obj
                List<Vector3> vertices = new List<Vector3>();
                List<Vector3> normals = new List<Vector3>();
                List<Vector2> uv = new List<Vector2>();
                List<int> triangles = new List<int>();

                // Списки для корректной сборки меша
                List<Vector3> tempVertices = new List<Vector3>();
                List<Vector3> tempNormals = new List<Vector3>();
                List<Vector2> tempUV = new List<Vector2>();

                // Чтение .obj файла построчно
                string[] lines = File.ReadAllLines(path);

                foreach (string line in lines)
                {
                    string[] tokens = line.Split(' ', System.StringSplitOptions.RemoveEmptyEntries);
                    if (tokens.Length == 0 || tokens[0].StartsWith("#"))
                        continue; // Пропустить комментарии и пустые строки

                    switch (tokens[0])
                    {
                        case "v": // Вершины
                            tempVertices.Add(new Vector3(
                                float.Parse(tokens[1], CultureInfo.InvariantCulture.NumberFormat),
                                float.Parse(tokens[2], CultureInfo.InvariantCulture.NumberFormat),
                                float.Parse(tokens[3], CultureInfo.InvariantCulture.NumberFormat)
                            ));
                            break;

                        case "vn": // Нормали
                            tempNormals.Add(new Vector3(
                                float.Parse(tokens[1], CultureInfo.InvariantCulture.NumberFormat),
                                float.Parse(tokens[2], CultureInfo.InvariantCulture.NumberFormat),
                                float.Parse(tokens[3], CultureInfo.InvariantCulture.NumberFormat)
                            ));
                            break;

                        case "vt": // UV-координаты
                            tempUV.Add(new Vector2(
                                float.Parse(tokens[1], CultureInfo.InvariantCulture.NumberFormat),
                                float.Parse(tokens[2], CultureInfo.InvariantCulture.NumberFormat)
                            ));
                            break;

                        case "f": // Грани
                            for (int i = 1; i <= 3; i++) // Обрабатываем треугольники
                            {
                                if (tokens.Length < 4)
                                {
                                    Debug.LogError("Incorrect model, face with 2 vertexes: " + line);
                                }

                                string[] vertexData = tokens[i].Split('/');
                                int vertexIndex = int.Parse(vertexData[0]) - 1; // Индексы в .obj начинаются с 1
                                vertices.Add(tempVertices[vertexIndex]);

                                if (vertexData.Length > 1 && vertexData[1] != "")
                                {
                                    int uvIndex = int.Parse(vertexData[1]) - 1;
                                    uv.Add(tempUV[uvIndex]);
                                }

                                if (vertexData.Length > 2 && vertexData[2] != "")
                                {
                                    int normalIndex = int.Parse(vertexData[2]) - 1;
                                    normals.Add(tempNormals[normalIndex]);
                                }

                                triangles.Add(vertices.Count - 1);
                            }

                            break;
                    }
                }

                // Создание и настройка Mesh
                Mesh mesh = new Mesh
                {
                    vertices = vertices.ToArray(),
                    triangles = triangles.ToArray(),
                    uv = uv.Count > 0 ? uv.ToArray() : null,
                    normals = normals.Count > 0 ? normals.ToArray() : null
                };

                if (normals.Count == 0) mesh.RecalculateNormals();
                mesh.RecalculateBounds();

                return mesh;
            }
            Debug.LogWarning($"Model file not found: {path}");
            return null;
        }

        [System.Serializable]
        private class MaterialConfig
        {
            public string shader;
            public float[] color; // RGBA
            public string texture;
        }
    }
}