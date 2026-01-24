using System.Collections.Generic;
using System.IO;
using System.Linq;
using Test.Configs;
using Test.Utilities.Entities;
using UnityEngine;

namespace Test
{
    /**
     * Загрузчик объектов из ресурсов.
     */
    public class ResourcesLoader : LoaderInt
    {
        public GameObject LoadModel(string modelsPath, string modelFile)
        {
            string modelPath = Path.Combine(modelsPath, modelFile);
            return Resources.Load<GameObject>(modelPath);
        }

        public List<GameObject> LoadAllModels(string modelsPath)
        {
            List<GameObject> models = new List<GameObject>();
            List<string> modelsNames = Directory
                .GetFiles(modelsPath)
                .Select(Path.GetFileNameWithoutExtension)
                .ToList();

            foreach (var modelName in modelsNames)
            {
                models.Add(LoadModel(modelsPath,  modelName));
            }

            return models;
        }

        public Dictionary<string, GameObject> LoadAllModelsWithName(string modelsPath)
        {
            Dictionary<string, GameObject> modelsWithName = new Dictionary<string, GameObject>();
            
            List<string> modelsNames = Directory
                .GetFiles(modelsPath)
                .Select(Path.GetFileNameWithoutExtension)
                .ToList();

            foreach (string modelName in modelsNames)
            {
                modelsWithName.Add(modelName, LoadModel(modelsPath, modelName));
            }
            return modelsWithName;
        }

        public Material LoadMaterial(string materialFile)
        {
            string materialPath = Path.Combine(PathConfig.MaterialsPath, materialFile);
            return Resources.Load<Material>(materialPath);
        }

        public Sprite LoadIcon(string iconFile)
        {
            string iconPath = Path.Combine(PathConfig.IconsPath, iconFile);
            return Resources.Load<Sprite>(iconPath);
        }

        public void LoadEnvironment(GameObject parent, TileConfig.EnvObject[] envObjects,
            EnvironmentLoader environmentLoader)
        {
            
        }

        public string GetPath()
        {
            return Application.dataPath;
        }
    }
}
