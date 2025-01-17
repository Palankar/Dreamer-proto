using System.Collections.Generic;
using System.Globalization;
using System.IO;
using Newtonsoft.Json;
using UnityEngine;

namespace Test
{
    public class OBJLoader : MonoBehaviour
    {
        public List<GameObject> LoadOBJ(string configPath, string modelsPath, string materialsPath, string iconsPath)
        {
            List<GameObject> loadedTiles = new List<GameObject>();
            string fullPath = Path.Combine(Application.streamingAssetsPath, configPath);
            
            if (!File.Exists(fullPath))
            {
                Debug.LogError($"Файл конфигурации не найден: {fullPath}");
                return loadedTiles;
            }
            
            string jsonContent = File.ReadAllText(fullPath);
            List<TileConfig> tileConfigs = JsonConvert.DeserializeObject<List<TileConfig>>(jsonContent);
            
            foreach (var tileConfig in tileConfigs)
            {
                GameObject model = LoadModel(modelsPath, tileConfig.modelFile);
                if (model != null)
                {
                    model = Instantiate(model, new Vector3(1000, 0, 1000), Quaternion.identity);
                }
                else
                {
                    Debug.LogWarning($"Не удалось загрузить модель {tileConfig.modelFile}");
                    continue;
                }

                model.AddComponent<MeshCollider>();

                Material material = MaterialLoader.LoadMaterialFromConfig(Path.Combine(materialsPath, tileConfig.materialFile + ".json"));

                MeshRenderer meshRenderer = model.AddComponent<MeshRenderer>();
                meshRenderer.material = material;
                
                Sprite icon = LoadIcon(iconsPath, tileConfig.iconFile);
                HexTile hexTile = model.AddComponent<HexTile>();
                hexTile.Sprite = icon;
                
                GameObject spawnPosition = new GameObject("SpawnPosition");
                spawnPosition.transform.parent = model.transform;
                spawnPosition.transform.localPosition = new Vector3(
                    tileConfig.spawnPosition[0],
                    tileConfig.spawnPosition[1],
                    tileConfig.spawnPosition[2]
                );

                hexTile.spawnPosition = spawnPosition;

                loadedTiles.Add(model);
            }

            return loadedTiles;
        }

        private Sprite LoadIcon(string iconsPath, string iconFile)
        {
            string iconPath = Path.Combine(Application.streamingAssetsPath, iconsPath, iconFile);
            if (File.Exists(iconPath))
            {
                byte[] textureData = File.ReadAllBytes(iconPath);
                Texture2D texture = new Texture2D(2, 2); // Создаем пустую текстуру
                texture.LoadImage(textureData); // Загружаем изображение
                return Sprite.Create(texture, new Rect(0.0f, 0.0f, texture.width, texture.height), Vector2.zero);
            }
            Debug.LogWarning($"Icon file not found: {iconPath}");
            return null;
        }

        private GameObject LoadModel(string modelsPath, string modelFile)
        {
            string modelPath = Path.Combine(Application.streamingAssetsPath, modelsPath, modelFile);
            if (File.Exists(modelPath))
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
                string[] lines = File.ReadAllLines(modelPath);

                foreach (string line in lines)
                {
                    string[] tokens = line.Split(' ', System.StringSplitOptions.RemoveEmptyEntries);
                    if (tokens.Length == 0 || tokens[0].StartsWith("#")) continue; // Пропустить комментарии и пустые строки

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

                GameObject model = new GameObject();

                // Привязка Mesh к объекту
                MeshFilter meshFilter = model.AddComponent<MeshFilter>();
                meshFilter.mesh = mesh;
                
                Debug.Log($"OBJ файл {modelFile} успешно загружен!");

                return model;
            }
            
            Debug.LogError($"Файл {modelFile} не найден!");
            return null;
        }
    }
}


