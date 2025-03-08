using UnityEngine;
using UnityEngine.Serialization;

namespace Test
{
    /**
     * Менеджер загрузки внешних файлов.
     */
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
                default:
                    Debug.LogError($"Неизвестный тип загрузчика: {loaderType}");
                    break;
            }
        }

        public LoaderInt GetLoader()
        {
            return _loader;
        }
    }
}