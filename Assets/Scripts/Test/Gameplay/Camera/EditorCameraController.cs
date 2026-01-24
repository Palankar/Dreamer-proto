using UnityEngine;

namespace Test
{
    /**
     * Базовый контроллер камеры.
     */
    public class EditorCameraController : MonoBehaviour
    {
        public float moveSpeed = 7.5f; // Скорость движения камеры
        public float zoomSpeed = 2f; // Скорость приближения/удаления
        public float minZoom = 0f; // Минимальная высота камеры
        public float maxZoom = 20f; // Максимальная высота камеры

        void Update()
        {
            // WASD движение камеры
            float horizontal = Input.GetAxis("Horizontal"); // A и D
            float vertical = Input.GetAxis("Vertical"); // W и S
            
            Vector3 moveInput = new Vector3(horizontal, 0, vertical);
            
            if (Input.GetMouseButton(1))
            {
                float mouseX = Input.GetAxis("Mouse X");
                transform.Rotate(Vector3.up, mouseX * moveSpeed, Space.World);
            }
            Vector3 movement = transform.TransformDirection(moveInput) * (moveSpeed * Time.deltaTime);
            movement.y = 0;
            
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
