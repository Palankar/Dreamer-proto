using UnityEngine;

namespace Test
{
    /**
     * Объект ячейки координатной сетки.
     */
    public class HexCell : MonoBehaviour
    {
        private Renderer _renderer; // Ссылка на Renderer ячейки
        public Color defaultColor = Color.white; // Цвет по умолчанию
        public Color hoverColor = Color.yellow;  // Цвет при наведении

        void Start()
        {
            _renderer = GetComponent<Renderer>();
            ResetColor(); // Устанавливаем цвет по умолчанию
        }

        public void ShowHoverColor()
        {
            if (_renderer != null)
            {
                _renderer.material.color = hoverColor; // Меняем цвет на цвет при наведении
            }
        }

        public void ResetColor()
        {
            if (_renderer != null)
            {
                _renderer.material.color = defaultColor; // Возвращаем цвет по умолчанию
            }
        }
    }
}
