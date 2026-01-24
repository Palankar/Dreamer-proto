using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using Test.Configs;
using Test.Utilities.Entities;
using UnityEngine;

namespace Test
{
    /**
     * Загрузчик тайлов.
     */
    public class TileLoader : MonoBehaviour
    {
        public LoaderManager loaderManager;

        private void Awake()
        {
            if (loaderManager == null) Debug.LogError("LoaderManager не назначен!");
        }

        public List<GameObject> LoadTiles()
        {
            string fullPath = Path.Combine(Application.streamingAssetsPath, PathConfig.ConfigPath);

            if (!File.Exists(fullPath))
            {
                Debug.LogError($"Файл конфигурации не найден: {fullPath}");
                return new List<GameObject>();
            }

            string jsonContent = File.ReadAllText(fullPath);
            List<TileConfig> tileConfigs = JsonConvert.DeserializeObject<List<TileConfig>>(jsonContent);
            if (tileConfigs != null)
            {
                tileConfigs.ForEach(config => config.Validate());

                List<GameObject> loadedTiles = new List<GameObject>();
                Dictionary<string, GameObject> loadedModels = loaderManager.GetLoader()
                    .LoadAllModelsWithName(PathConfig.TilesPath);
                
                foreach (GameObject loadedTile in loadedModels.Values)
                {
                    GameObject tile = loadedTile;
                    tile.transform.position = new Vector3(100, 0, 100);

                    SetBaseTileComponents(loaderManager.GetLoader(), ref tile);

                    loadedTiles.Add(tile);
                }

                return loadedTiles;
            }
            Debug.LogError("Конфигурации тайлов не были десериализованы!");
            return null;
        }


        private void SetBaseTileComponents(LoaderInt loader, ref GameObject tile)
        {
            tile.AddComponent<MeshCollider>();

            Material material = loader.LoadMaterial("base");

            MeshRenderer meshRenderer = tile.AddComponent<MeshRenderer>();
            meshRenderer.material = material;

            HexTile hexTile = tile.AddComponent<HexTile>();

            GameObject spawnPosition = new GameObject("SpawnPosition");
            spawnPosition.transform.parent = tile.transform;
            spawnPosition.transform.localPosition = new Vector3(
                0,
                15,
                0
            );

            hexTile.spawnPosition = spawnPosition;
        }
        
    }
}