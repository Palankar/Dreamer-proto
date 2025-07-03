using System.Collections.Generic;
using Assimp;
using System.IO;
using System.Linq;
using UnityEngine;
using Newtonsoft.Json;
using Material = UnityEngine.Material;
using Mesh = UnityEngine.Mesh;

namespace Test
{
    public class TileLoader : MonoBehaviour
    {
        public string configPath = "Configs/tiles_config.json"; // Путь к файлу конфигурации
        public string modelsPath = "Tiles/";                   // Путь к FBX-моделям
        public string materialsPath = "Materials/";            // Путь к материалам
        public string iconsPath = "UI/";                       // Путь к изображениям

        public List<GameObject> LoadTiles()
        {
            List<GameObject> loadedTiles = new List<GameObject>();
            string fullPath = Application.streamingAssetsPath + "/" + configPath;
                

            if (!File.Exists(fullPath))
            {
                Debug.LogError($"Файл конфигурации не найден: {fullPath}");
                return loadedTiles;
            }

            string jsonContent = File.ReadAllText(fullPath);
            List<TileConfig> tileConfigs = JsonConvert.DeserializeObject<List<TileConfig>>(jsonContent);

            foreach (var tileConfig in tileConfigs)
            {
                GameObject model = LoadModel(tileConfig.modelFile);
                if (model == null)
                {
                    Debug.LogWarning($"Не удалось загрузить модель {tileConfig.modelFile}");
                    continue;
                }

                model.transform.position = new Vector3(1000, 0, 1000); // Отодвигаем модель за пределы видимости
                model.AddComponent<MeshCollider>(); // Добавляем MeshCollider

                Material material = LoadMaterial(tileConfig.materialFile);
                if (material != null)
                {
                    Renderer renderer = model.GetComponent<Renderer>();
                    if (renderer != null)
                        renderer.material = material;
                }

                Sprite icon = LoadIcon(tileConfig.iconFile);
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

        private GameObject LoadModel(string modelFile)
        {
            string modelPath = Application.streamingAssetsPath + "/" + modelsPath + modelFile;

            if (!File.Exists(modelPath))
            {
                Debug.LogError($"Файл модели не найден: {modelPath}");
                return null;
            }

            AssimpContext importer = new AssimpContext();
            try
            {
                // Импорт модели
                Scene modelScene = importer.ImportFile(modelPath, PostProcessPreset.TargetRealTimeMaximumQuality);

                // Создаём корневой объект
                GameObject rootObject = new GameObject(Path.GetFileNameWithoutExtension(modelFile));

                // Конвертация всех мешей в Unity-модели
                foreach (var mesh in modelScene.Meshes)
                {
                    Mesh unityMesh = new Mesh
                    {
                        vertices = mesh.Vertices.Select(v => new Vector3(v.X, v.Y, v.Z)).ToArray(),
                        normals = mesh.Normals.Select(n => new Vector3(n.X, n.Y, n.Z)).ToArray(),
                        triangles = mesh.GetIndices().ToArray()
                    };

                    GameObject meshObject = new GameObject(mesh.Name);
                    MeshFilter meshFilter = meshObject.AddComponent<MeshFilter>();
                    meshFilter.mesh = unityMesh;

                    MeshRenderer meshRenderer = meshObject.AddComponent<MeshRenderer>();
                    meshRenderer.material = new Material(Shader.Find("Standard")); // Для теста, замените материал позже

                    meshObject.transform.parent = rootObject.transform;
                }
                
                rootObject.AddComponent<MeshRenderer>();
                
                // Установка масштаба
                Bounds bounds = CalculateBounds(rootObject);
                float scaleFactor = 1.0f / Mathf.Max(bounds.size.x, bounds.size.y, bounds.size.z);
                rootObject.transform.localScale = Vector3.one * scaleFactor * 8;

                return rootObject;
            }
            catch (System.Exception ex)
            {
                Debug.LogError($"Ошибка при загрузке FBX: {ex.Message}");
                return null;
            }
        }
        
        private Bounds CalculateBounds(GameObject model)
        {
            Renderer[] renderers = model.GetComponentsInChildren<Renderer>();
            Bounds bounds = new Bounds(model.transform.position, Vector3.zero);
            foreach (var renderer in renderers)
            {
                bounds.Encapsulate(renderer.bounds);
            }
            return bounds;
        }

        private Material LoadMaterial(string materialFile)
        {
            string materialPath = Application.streamingAssetsPath + "/" + materialsPath + materialFile;

            if (!File.Exists(materialPath))
            {
                Debug.LogWarning($"Файл материала не найден: {materialPath}");
                return null;
            }

            Material material = new Material(Shader.Find("Standard"));
            Texture2D texture = new Texture2D(2, 2);
            byte[] textureData = File.ReadAllBytes(materialPath);

            if (texture.LoadImage(textureData))
            {
                material.mainTexture = texture;
            }
            else
            {
                Debug.LogWarning($"Не удалось загрузить текстуру из материала: {materialFile}");
            }

            return material;
        }

        private Sprite LoadIcon(string iconFile)
        {
            string iconPath = Application.streamingAssetsPath + "/" + iconsPath + iconFile;

            if (!File.Exists(iconPath))
            {
                Debug.LogWarning($"Файл иконки не найден: {iconPath}");
                return null;
            }

            Texture2D texture = new Texture2D(2, 2);
            byte[] textureData = File.ReadAllBytes(iconPath);

            if (texture.LoadImage(textureData))
            {
                return Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
            }

            Debug.LogWarning($"Не удалось загрузить иконку: {iconFile}");
            return null;
        }

        private Vector3[] ConvertVector3DArray(IList<Assimp.Vector3D> vectors)
        {
            Vector3[] unityVectors = new Vector3[vectors.Count];
            for (int i = 0; i < vectors.Count; i++)
            {
                unityVectors[i] = new Vector3(vectors[i].X, vectors[i].Y, vectors[i].Z);
            }
            return unityVectors;
        }
    }

    [System.Serializable]
    public class TileConfig
    {
        public string name;
        public string modelFile;
        public string materialFile;
        public string iconFile;
        public float[] spawnPosition;
    }

}
