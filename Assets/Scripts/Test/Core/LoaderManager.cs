using UnityEngine;
using UnityEngine.Serialization;

namespace Test
{
    public class LoaderManager : MonoBehaviour
    {
        private enum LoaderType
        {
            StreamingAssets,
            Resources
        }
        
        [SerializeField] private LoaderType loaderType;

        private LoaderInt _loader;

        void Awake()
        {
            switch (loaderType)
            {
                case LoaderType.StreamingAssets:
                    _loader = new StreamingAssetsLoader();
                    break;
                case LoaderType.Resources:
                    _loader = new ResourcesLoader();
                    break;
            }
        }

        public LoaderInt GetLoader()
        {
            return _loader;
        }
    }
}