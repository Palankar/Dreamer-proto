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
        
        public float moveSpeedFlying = 7.5f;
        public float zoomSpeedFlying = 2f;
        public float minZoomFlying = 0f;
        public float maxZoomFlying = 20f;

        [SerializeField] private GameModeManager gameModeManager;

        void Update()
        {
            var currentMode = gameModeManager.GetCurrentMode();
            
            if (currentMode == GameModeManager.GameMode.CharacterControl)
                return;
            
            // WASD движение камеры
            float horizontal = Input.GetAxis("Horizontal"); // A и D
            float vertical = Input.GetAxis("Vertical"); // W и S
            
            Vector3 moveInput = new Vector3(horizontal, 0, vertical);
            Vector3 movement;
            
            if (currentMode == GameModeManager.GameMode.Building)
            {
                movement = new Vector3(horizontal, 0, vertical) * moveSpeed * Time.deltaTime;
            }
            else
            {
                if (Input.GetMouseButton(1))
                {
                    float mouseX = Input.GetAxis("Mouse X");
                    transform.Rotate(Vector3.up, mouseX * moveSpeed, Space.World);
                }
                movement = transform.TransformDirection(moveInput) * moveSpeedFlying * Time.deltaTime;
                movement.y = 0;
            }
            
            transform.Translate(movement, Space.World);

            // Колесико мыши для приближения/удаления
            float scroll = Input.GetAxis("Mouse ScrollWheel");
            Vector3 position = transform.position;
            if (currentMode == GameModeManager.GameMode.Building)
            {
                position.y -= scroll * zoomSpeed;
                position.y = Mathf.Clamp(position.y, minZoom, maxZoom);
            }
            else
            {
                position.y -= scroll * zoomSpeedFlying;
                position.y = Mathf.Clamp(position.y, minZoomFlying, maxZoomFlying);
            }
            transform.position = position;
        }
    }
}
