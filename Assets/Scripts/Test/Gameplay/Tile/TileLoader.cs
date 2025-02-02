using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using Test.Utilities.Entities;
using UnityEngine;

namespace Test
{
    public class TileLoader : MonoBehaviour
    {
        public string configPath = "Configs/tiles_config.json"; // Путь к файлу конфигурации
        public string modelsPath = "Tiles/Obj/";                // Путь к 3D-моделям
        public string materialsPath = "Materials/";             // Путь к материалам
        public string iconsPath = "UI/";                        // Путь к изображениям
        public string objectsPath = "Objects/";                 // Путь к объектам окружения

        public bool isResources;

        private StreamingAssetsLoader _streamingAssetsLoader;
        private ResourcesLoader _resourcesLoader;

        private void Start()
        {
            _streamingAssetsLoader = new StreamingAssetsLoader();
            _resourcesLoader = new ResourcesLoader();
        }

        public List<GameObject> LoadTiles()
        {
            if (isResources)
            {
                return LoadTiles(_resourcesLoader);
            }
            return LoadTiles(_streamingAssetsLoader);
        }

        private List<GameObject> LoadTiles(LoaderInt loader)
        {
            string fullPath = Path.Combine(Application.streamingAssetsPath, configPath);

            if (!File.Exists(fullPath))
            {
                Debug.LogError($"Файл конфигурации не найден: {fullPath}");
                return new List<GameObject>();
            }

            string jsonContent = File.ReadAllText(fullPath);
            List<TileConfig> tileConfigs = JsonConvert.DeserializeObject<List<TileConfig>>(jsonContent);

            List<GameObject> loadedTiles = new List<GameObject>();
            foreach (TileConfig tileConfig in tileConfigs)
            {
                GameObject tile = _streamingAssetsLoader.LoadModel(modelsPath, tileConfig.modelFile);

                SetTileComponents(loader, ref tile, tileConfig);
                
                loadedTiles.Add(tile);
            }

            return loadedTiles;
        }


        private void SetTileComponents(LoaderInt loader, ref GameObject tile, TileConfig tileConfig)
        {
            tile.AddComponent<MeshCollider>();

            Material material = loader.LoadMaterial(materialsPath, tileConfig.materialFile);

            MeshRenderer meshRenderer = tile.AddComponent<MeshRenderer>();
            meshRenderer.material = material;

            HexTile hexTile = tile.AddComponent<HexTile>();
            hexTile.Sprite = loader.LoadIcon(iconsPath, tileConfig.iconFile);

            GameObject spawnPosition = new GameObject("SpawnPosition");
            spawnPosition.transform.parent = tile.transform;
            spawnPosition.transform.localPosition = new Vector3(
                tileConfig.spawnPosition[0],
                tileConfig.spawnPosition[1],
                tileConfig.spawnPosition[2]
            );

            hexTile.spawnPosition = spawnPosition;

            //TODO: Иерархия parent объектов: Environment -> Trees, Stones, etc
            // Загрузка объектов окружения - камней
            GameObject stonesParent = new GameObject("Stones");
            stonesParent.transform.parent = tile.transform;
            stonesParent.transform.localPosition = Vector3.zero;
            foreach (var stone in tileConfig.stones)
            {
                loader.LoadEnvironment(objectsPath, stone.modelFile, materialsPath, stone.materialFile, stonesParent,
                    stone.position, 0.2f);
            }

            // Загрузка объектов окружения - деревьев
            GameObject treesParent = new GameObject("Trees");
            treesParent.transform.parent = tile.transform;
            treesParent.transform.localPosition = Vector3.zero;
            foreach (var tree in tileConfig.trees)
            {
                loader.LoadEnvironment(objectsPath, tree.modelFile, materialsPath, tree.materialFile, treesParent,
                    tree.position, 2.0f);
            }
        }
    }
}