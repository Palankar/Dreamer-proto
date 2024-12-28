using UnityEngine;
using UnityEngine.UI;

namespace Test
{
    public class TileSelectionUIManager : MonoBehaviour
    {
        public GameObject[] tilePrefabs; // Список доступных префабов тайлов
        public Transform buttonContainer; // Контейнер для кнопок (например, Panel)
        public Button buttonPrefab; // Префаб кнопки

        private int selectedTileIndex = -1; // Индекс выбранного тайла (-1, если не выбран)

        void Start()
        {
            PopulateTileButtons();
        }

        void PopulateTileButtons()
        {
            int width = tilePrefabs.Length / 2;
            for (int i = 0; i < tilePrefabs.Length; i++)
            {
                GameObject tile = tilePrefabs[i];
                
                // Создаем новую кнопку
                Button newButton = Instantiate(buttonPrefab, buttonContainer);
                
                // Задаем позицию кнопки
                newButton.gameObject.transform.localPosition = new Vector2((width * -200) + (160 * i + 40 * i), 0);
                
                // Настраиваем изображение кнопки
                Image buttonImage = newButton.GetComponent<Image>();
                Sprite tileSprite = tile.GetComponent<HexTile>().Sprite;
                if (tileSprite != null)
                {
                    buttonImage.sprite = tileSprite;
                }

                // Добавляем обработчик клика
                int index = i; // Копируем индекс в локальную переменную, чтобы избежать замыканий
                newButton.onClick.AddListener(() => SelectTile(index));
            }
        }

        public void SelectTile(int index)
        {
            selectedTileIndex = index;
            Debug.Log($"Выбранный тайл: {tilePrefabs[selectedTileIndex].name}");
        }

        public GameObject GetSelectedTilePrefab()
        {
            if (selectedTileIndex >= 0 && selectedTileIndex < tilePrefabs.Length)
            {
                return tilePrefabs[selectedTileIndex];
            }
            return null;
        }
    }
}
