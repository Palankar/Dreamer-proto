using System.Collections.Generic;
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

        public bool isResources = true;

        public StreamingAssetsLoader streamingAssetsLoader;
        public ResourcesLoader resourcesLoader;

        public List<GameObject> LoadTiles()
        {
            if (isResources)
            {
                return resourcesLoader.LoadTiles(configPath, modelsPath, materialsPath, iconsPath);
            }
            return streamingAssetsLoader.LoadTiles(configPath, modelsPath, materialsPath, iconsPath, objectsPath);
        }
    }

    //TODO: Перенести MonoBehaviour логику сюда, избавив streamingAssetsLoader и resourcesLoader от нее.

}
