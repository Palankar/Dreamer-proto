using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using UnityEngine;

namespace Test
{
    public class StreamingAssetsLoader : MonoBehaviour
    {
        public List<GameObject> LoadTiles(string configPath, string modelsPath, string materialsPath, string iconsPath, string objectsPath)
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
                GameObject tile = LoadModel(modelsPath, tileConfig.modelFile);
                if (tile != null)
                {
                    tile = Instantiate(tile, new Vector3(1000, 0, 1000), Quaternion.identity);
                }
                else
                {
                    Debug.LogWarning($"Не удалось загрузить модель {tileConfig.modelFile}");
                    continue;
                }

                tile.AddComponent<MeshCollider>();

                Material material = FilesLoader.LoadMaterialFromConfig(Path.Combine(materialsPath, tileConfig.materialFile + ".json"));

                MeshRenderer meshRenderer = tile.AddComponent<MeshRenderer>();
                meshRenderer.material = material;
                
                string iconPath = Path.Combine(Application.streamingAssetsPath, iconsPath, tileConfig.iconFile);
                Sprite icon = FilesLoader.LoadSpriteFromPath(iconPath);
                
                HexTile hexTile = tile.AddComponent<HexTile>();
                hexTile.Sprite = icon;
                
                GameObject spawnPosition = new GameObject("SpawnPosition");
                spawnPosition.transform.parent = tile.transform;
                spawnPosition.transform.localPosition = new Vector3(
                    tileConfig.spawnPosition[0],
                    tileConfig.spawnPosition[1],
                    tileConfig.spawnPosition[2]
                );

                hexTile.spawnPosition = spawnPosition;
                
                // Загрузка объектов окружения - камней
                GameObject stonesParent = new GameObject("Stones");
                stonesParent.transform.parent = tile.transform;
                stonesParent.transform.localPosition = Vector3.zero;
                foreach (var stone in tileConfig.stones)
                {
                    LoadEnvironment(objectsPath, stone.modelFile, materialsPath, stone.materialFile, stonesParent, stone.position, 0.2f);
                }
                
                // Загрузка объектов окружения - деревьев
                GameObject treesParent = new GameObject("Trees");
                treesParent.transform.parent = tile.transform;
                treesParent.transform.localPosition = Vector3.zero;
                foreach (var tree in tileConfig.trees)
                {
                    LoadEnvironment(objectsPath, tree.modelFile, materialsPath, tree.materialFile, treesParent, tree.position, 2.0f);
                }

                loadedTiles.Add(tile);
            }

            return loadedTiles;
        }

        private GameObject LoadModel(string modelsPath, string modelFile)
        {
            string modelPath = Path.Combine(Application.streamingAssetsPath, modelsPath, modelFile);
            Mesh mesh = FilesLoader.LoadObjMeshFromPath(modelPath);
            
            GameObject model = new GameObject(modelFile.Substring(0, modelFile.LastIndexOf('.')));

            // Привязка Mesh к объекту
            MeshFilter meshFilter = model.AddComponent<MeshFilter>();
            meshFilter.mesh = mesh;
            
            return model;
        }

        private void LoadEnvironment(string objectsPath, string modelFile, string materialsPath, string materialFile, 
            GameObject parent, float[] position, float scale)
        {
            GameObject envObject = LoadModel(objectsPath, modelFile);
            
            envObject.transform.parent = parent.transform;
            envObject.transform.localPosition = new Vector3(
                position[0],
                position[1],
                position[2]
            );

            envObject.transform.localScale = new Vector3(scale, scale, scale);
            
            envObject.AddComponent<MeshCollider>();

            Material material = FilesLoader.LoadMaterialFromConfig(Path.Combine(materialsPath, materialFile + ".json"));

            MeshRenderer meshRenderer = envObject.AddComponent<MeshRenderer>();
            meshRenderer.material = material;
        }
    }
}


