using System.IO;
using UnityEngine;

namespace Test
{
    public static class MaterialLoader
    {
        public static Material LoadMaterialFromConfig(string configFilePath)
        {
            string fullPath = Path.Combine(Application.streamingAssetsPath, configFilePath);

            if (!File.Exists(fullPath))
            {
                Debug.LogError($"Material config file not found: {fullPath}");
                return null;
            }

            // Чтение конфигурации
            string jsonContent = File.ReadAllText(fullPath);
            MaterialConfig config = JsonUtility.FromJson<MaterialConfig>(jsonContent);

            // Создание материала
            Shader shader = Shader.Find(config.shader);
            if (shader == null)
            {
                Debug.LogError($"Shader not found: {config.shader}");
                return null;
            }

            Material material = new Material(shader);

            // Установка цвета
            Color color = new Color(config.color[0], config.color[1], config.color[2], config.color[3]);
            material.color = color;

            // Загрузка текстуры
            if (!string.IsNullOrEmpty(config.texture))
            {
                string texturePath = Path.Combine(Application.streamingAssetsPath, config.texture);
                if (File.Exists(texturePath))
                {
                    byte[] textureData = File.ReadAllBytes(texturePath);
                    Texture2D texture = new Texture2D(2, 2); // Создаем пустую текстуру
                    texture.LoadImage(textureData); // Загружаем изображение
                    material.mainTexture = texture;
                }
                else
                {
                    Debug.LogWarning($"Texture file not found: {texturePath}");
                }
            }

            return material;
        }

        [System.Serializable]
        private class MaterialConfig
        {
            public string shader;
            public float[] color; // RGBA
            public string texture;
        }
    }
}