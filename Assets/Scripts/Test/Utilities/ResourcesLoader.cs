using System.Collections.Generic;
using System.IO;
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

        public Material LoadMaterial(string materialsPath, string materialFile)
        {
            string materialPath = Path.Combine(materialsPath, materialFile);
            return Resources.Load<Material>(materialPath);
        }

        public Sprite LoadIcon(string iconsPath, string iconFile)
        {
            string iconPath = Path.Combine(iconsPath, iconFile);
            return Resources.Load<Sprite>(iconPath);
        }

        public void LoadEnvironment(string objectsPath, string modelFile, string materialsPath, string materialFile,
            GameObject parent, float[] position, float scale)
        {
            
        }

        public List<GameObject> LoadEnvironment(string objectsPath, TileConfig config)
        {
            return new List<GameObject>();
        }

        public string GetPath()
        {
            return Application.dataPath;
        }
    }
}
