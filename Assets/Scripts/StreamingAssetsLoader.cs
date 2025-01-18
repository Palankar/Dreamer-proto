using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using UnityEngine;

namespace Test
{
    public class StreamingAssetsLoader : MonoBehaviour
    {
        public List<GameObject> LoadTiles(string configPath, string modelsPath, string materialsPath, string iconsPath)
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

                Material material = FilesLoader.LoadMaterialFromConfig(Path.Combine(materialsPath, tileConfig.materialFile + ".json"));

                MeshRenderer meshRenderer = model.AddComponent<MeshRenderer>();
                meshRenderer.material = material;
                
                string iconPath = Path.Combine(Application.streamingAssetsPath, iconsPath, tileConfig.iconFile);
                Sprite icon = FilesLoader.LoadSpriteFromPath(iconPath);
                
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
            string modelPath = Path.Combine(Application.streamingAssetsPath, modelsPath, modelFile);
            Mesh mesh = FilesLoader.LoadObjMeshFromPath(modelPath);
            
            GameObject model = new GameObject();

            // Привязка Mesh к объекту
            MeshFilter meshFilter = model.AddComponent<MeshFilter>();
            meshFilter.mesh = mesh;
            
            return model;
        }
    }
}


