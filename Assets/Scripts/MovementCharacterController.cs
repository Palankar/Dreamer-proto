using UnityEngine;

namespace Test
{
    [RequireComponent(typeof(CharacterController))]
    public class MovementCharacterController : MonoBehaviour
    {
        public float moveSpeed = 0.5f; // Скорость движения
        public float gravity = -9.81f; // Гравитация
        
        private Vector3 _velocity;      // Скорость падения
        private CharacterController _characterController;
        
        [SerializeField] private GameModeManager gameModeManager;

        void Start()
        {
            _characterController = GetComponent<CharacterController>();
        }

        void Update()
        {
            if (gameModeManager.GetCurrentMode() != GameModeManager.GameMode.CharacterControl)
                return;
            
            // Применяем гравитацию
            if (!_characterController.isGrounded)
            {
                _velocity.y += gravity * Time.fixedDeltaTime;
            }
            else
            {
                _velocity.y = 0f; // Обнуляем скорость падения на земле
            }
            
            // Получаем входные данные
            float horizontal = Input.GetAxis("Horizontal");
            float vertical = Input.GetAxis("Vertical");

            Vector3 direction = new Vector3(horizontal, 0, vertical);

            if (direction.magnitude >= 0.1f)
            {
                // Направление движения
                Vector3 move = direction * moveSpeed;

                // Двигаем персонажа с учетом гравитации
                _characterController.Move((move + _velocity) * Time.fixedDeltaTime);
            }
        }
    }
}
