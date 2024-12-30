using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Test
{
    public class TileSelectionUIManager : MonoBehaviour
    {
        public Transform buttonContainer; // Контейнер для кнопок (например, Panel)
        public Button buttonPrefab; // Префаб кнопки
        public GameObject tileLoaderManager;
        
        private List<GameObject> loadedTiles = new(); // Динамический список тайлов
        private TileLoader _tileLoader;

        private int _selectedTileIndex = -1; // Индекс выбранного тайла (-1, если не выбран)

        void Start()
        {
            _tileLoader = tileLoaderManager.GetComponent<TileLoader>();
            loadedTiles = _tileLoader.LoadTiles();
            PopulateTileButtons();
        }

        void PopulateTileButtons()
        {
            int width = loadedTiles.Count / 2;
            for (int i = 0; i < loadedTiles.Count; i++)
            {
                GameObject tile = loadedTiles[i];
                
                // Создаем новую кнопку
                Button newButton = Instantiate(buttonPrefab, buttonContainer);
                
                // Задаем позицию кнопки
                newButton.gameObject.transform.localPosition = new Vector2((width * -200) + (160 * i + 40 * i), 0);
                
                // Настраиваем изображение кнопки
                Image buttonImage = newButton.GetComponent<Image>();
                Sprite tileSprite = tile.GetComponent<HexTile>().Sprite;
                buttonImage.sprite = tileSprite;
                
                // Копируем индекс в локальную переменную, чтобы избежать замыканий
                int index = i; 

                // Добавляем обработчик клика
                newButton.onClick.AddListener(() => SelectTile(index));
            }
        }

        public void SelectTile(int index)
        {
            _selectedTileIndex = index;
            Debug.Log($"Выбранный тайл: {loadedTiles[_selectedTileIndex].name}");
        }

        public GameObject GetSelectedTilePrefab()
        {
            if (_selectedTileIndex >= 0 && _selectedTileIndex < loadedTiles.Count)
            {
                return loadedTiles[_selectedTileIndex];
            }
            return null;
        }
    }
}
