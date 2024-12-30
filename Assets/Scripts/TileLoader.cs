using System.Collections.Generic;
using System.IO;
using UnityEngine;
using Newtonsoft.Json;

namespace Test
{
    public class TileLoader : MonoBehaviour
    {
        public string configPath = "Configs/tiles_config.json"; // Путь к файлу конфигурации
        public string modelsPath = "Tiles/";                   // Путь к 3D-моделям
        public string materialsPath = "Materials/";            // Путь к материалам
        public string iconsPath = "UI/";                       // Путь к изображениям

        public List<GameObject> LoadTiles()
        {
            List<GameObject> loadedTiles = new List<GameObject>();
            string fullPath = Path.Combine(Application.dataPath, configPath);

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
            string modelPath = Path.Combine(modelsPath, modelFile);
            return Resources.Load<GameObject>(modelPath);
        }

        private Material LoadMaterial(string materialFile)
        {
            string materialPath = Path.Combine(materialsPath, materialFile);
            return Resources.Load<Material>(materialPath);
        }

        private Sprite LoadIcon(string iconFile)
        {
            string iconPath = Path.Combine(iconsPath, iconFile);
            return Resources.Load<Sprite>(iconPath);
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
