using UnityEngine;

namespace Test
{
    /**
     * Базовый контроллер управления персонажем.
     */
    [RequireComponent(typeof(CharacterController))]
    public class SimpleMovementCharacterController : MonoBehaviour
    {
        public float speed = 0.5f;
        public float rotationSpeed = 15f;

        private CharacterController _controller;
        private Transform _cameraTransform;
        
        [SerializeField] private GameModeManager gameModeManager;

        void Awake()
        {
            if (gameModeManager == null) Debug.LogError("GameModeManager не назначен!");
            if (_cameraTransform == null && Camera.main != null) _cameraTransform = Camera.main.transform;
            if (_cameraTransform == null) Debug.LogError("Camera Transform не назначен!");
        }
        
        void Start()
        {
            _controller = GetComponent<CharacterController>();
        }

        void Update()
        {
            if (gameModeManager.GetCurrentMode() != GameModeManager.GameMode.CharacterControl)
                return;
            
            MoveCharacter();
        }
        
        private void MoveCharacter()
        {
            if (Input.GetMouseButton(1))
            {
                float mouseX = Input.GetAxis("Mouse X");
                _cameraTransform.RotateAround(transform.position, Vector3.up, mouseX * rotationSpeed);
            }
            
            // Получение ввода с клавиатуры
            float horizontal = Input.GetAxis("Horizontal");
            float vertical = Input.GetAxis("Vertical");

            // Направление движения
            Vector3 moveInput = new Vector3(horizontal, 0, vertical);
            Vector3 moveDirection = _cameraTransform.TransformDirection(moveInput);
            moveDirection.y = 0;
            moveDirection = transform.TransformDirection(moveDirection);

            // Простое движение
            _controller.SimpleMove(moveDirection * speed);
        }
    }
}
