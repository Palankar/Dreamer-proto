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
                foreach (TileConfig tileConfig in tileConfigs)
                {
                    GameObject tile = loaderManager.GetLoader().LoadModel(PathConfig.TilesPath, tileConfig.modelFile);
                    tile.transform.position = new Vector3(100, 0, 100);

                    SetTileComponents(loaderManager.GetLoader(), ref tile, tileConfig);

                    loadedTiles.Add(tile);
                }

                return loadedTiles;
            }
            Debug.LogError("Конфигурации тайлов не были десериализованы!");
            return null;
        }


        private void SetTileComponents(LoaderInt loader, ref GameObject tile, TileConfig tileConfig)
        {
            tile.AddComponent<MeshCollider>();

            Material material = loader.LoadMaterial(tileConfig.materialFile);

            MeshRenderer meshRenderer = tile.AddComponent<MeshRenderer>();
            meshRenderer.material = material;

            HexTile hexTile = tile.AddComponent<HexTile>();
            hexTile.Sprite = loader.LoadIcon(tileConfig.iconFile);
            hexTile.TileConfig = tileConfig;

            GameObject spawnPosition = new GameObject("SpawnPosition");
            spawnPosition.transform.parent = tile.transform;
            spawnPosition.transform.localPosition = new Vector3(
                tileConfig.spawnPosition[0],
                tileConfig.spawnPosition[1],
                tileConfig.spawnPosition[2]
            );

            hexTile.spawnPosition = spawnPosition;
        }

    }
}