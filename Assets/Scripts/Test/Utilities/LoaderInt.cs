using System.Collections.Generic;
using Test.Utilities.Entities;
using UnityEngine;

namespace Test
{
    public interface LoaderInt
    {
        public GameObject LoadModel(string modelsPath, string modelFile);

        public Material LoadMaterial(string materialsPath, string materialFile);

        public Sprite LoadIcon(string iconsPath, string iconFile);

        public List<GameObject> LoadEnvironment(string objectsPath, TileConfig config);
        
        public void LoadEnvironment(string objectsPath, string modelFile, string materialsPath, string materialFile,
            GameObject parent, float[] position, float scale);

        public string GetPath();
    }
}