using System.Collections.Generic;
using UnityEngine;

namespace Test
{
    public class TileLoader : MonoBehaviour
    {
        public string configPath = "Configs/tiles_config.json"; // Путь к файлу конфигурации
        public string modelsPath = "Tiles/Obj/";                   // Путь к 3D-моделям
        public string materialsPath = "Materials/";            // Путь к материалам
        public string iconsPath = "UI/";                       // Путь к изображениям

        public bool isResources = true;

        public OBJLoader objLoader;
        public ResourcesLoader resourcesLoader;

        public List<GameObject> LoadTiles()
        {
            if (isResources)
            {
                return resourcesLoader.LoadResources(configPath, modelsPath, materialsPath, iconsPath);
            }
            return objLoader.LoadOBJ(configPath, modelsPath, materialsPath, iconsPath);
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
    
    //TODO: по-хорошему, тут нужно будет все загрузки реализовать с выбором.
    //А в отдельных классах раскидать подгрузки с ресурсов или из файлов.
    //Класс для моделей, класс для иконок, класс для матегориалов. Без MonoBehaviour причем.

}
