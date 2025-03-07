using System.IO;
using Test.Configs;
using Test.Utilities.Entities;
using UnityEngine;

namespace Test
{
    public class ResourcesLoader : LoaderInt
    {
        public GameObject LoadModel(string modelsPath, string modelFile)
        {
            string modelPath = Path.Combine(modelsPath, modelFile);
            return Resources.Load<GameObject>(modelPath);
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
