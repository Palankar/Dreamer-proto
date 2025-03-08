using UnityEngine;

namespace Test.Utilities.Entities
{
    /**
     * Конфигурация материала.
     */
    [System.Serializable]
    public class MaterialConfig
    {
        public string shader;
        public float[] color; // RGBA
        public string texture;
        
        public void Validate()
        {
            if (string.IsNullOrEmpty(shader)) Debug.LogWarning("Шейдер не задан в MaterialConfig!");
            if (color == null || color.Length != 4) Debug.LogWarning("Цвет в MaterialConfig должен содержать 4 значения (RGBA)!");
        }
    }
}