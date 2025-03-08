using UnityEngine;

namespace Test
{
    /**
     * Базовый контроллер камеры.
     */
    public class CameraController : MonoBehaviour
    {
        public float moveSpeed = 15f; // Скорость движения камеры
        public float zoomSpeed = 10f; // Скорость приближения/удаления
        public float minZoom = 5f; // Минимальная высота камеры
        public float maxZoom = 60f; // Максимальная высота камеры

        [SerializeField] private GameModeManager gameModeManager;

        void Update()
        {
            if (gameModeManager.GetCurrentMode() != GameModeManager.GameMode.CharacterControl)
            {
                // WASD движение камеры
                float horizontal = Input.GetAxis("Horizontal"); // A и D
                float vertical = Input.GetAxis("Vertical"); // W и S

                Vector3 movement = new Vector3(horizontal, 0, vertical) * moveSpeed * Time.deltaTime;
                transform.Translate(movement, Space.World);

                // Колесико мыши для приближения/удаления
                float scroll = Input.GetAxis("Mouse ScrollWheel");
                Vector3 position = transform.position;
                position.y -= scroll * zoomSpeed;
                position.y = Mathf.Clamp(position.y, minZoom, maxZoom);
                transform.position = position;
            }
        }
    }
}
