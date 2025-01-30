using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using Test.Utilities.Entities;
using UnityEngine;

namespace Test
{
    public class ResourcesLoader : MonoBehaviour
    {
        public List<GameObject> LoadTiles(string configPath, string modelsPath, string materialsPath, string iconsPath)
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

                Material material = LoadMaterial(materialsPath, tileConfig.materialFile);
                if (material != null)
                {
                    MeshRenderer renderer = model.GetComponentInChildren<MeshRenderer>();
                    if (renderer != null)
                        renderer.material = material;
                }

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

        private GameObject LoadModel(string modelsPath, string modelFile)
        {
            string modelPath = Path.Combine(modelsPath, modelFile);
            return Resources.Load<GameObject>(modelPath);
        }

        private Material LoadMaterial(string materialsPath, string materialFile)
        {
            string materialPath = Path.Combine(materialsPath, materialFile);
            return Resources.Load<Material>(materialPath);
        }

        private Sprite LoadIcon(string iconsPath, string iconFile)
        {
            string iconPath = Path.Combine(iconsPath, iconFile);
            return Resources.Load<Sprite>(iconPath);
        }
    }
}
