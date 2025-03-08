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

        private CharacterController _controller;
        
        [SerializeField] private GameModeManager gameModeManager;

        void Awake()
        {
            if (gameModeManager == null) Debug.LogError("GameModeManager не назначен!");
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
            // Получение ввода с клавиатуры
            float horizontal = Input.GetAxis("Horizontal");
            float vertical = Input.GetAxis("Vertical");

            // Направление движения
            Vector3 moveDirection = new Vector3(horizontal, 0, vertical);
            moveDirection = transform.TransformDirection(moveDirection);

            // Простое движение
            _controller.SimpleMove(moveDirection * speed);
        }
    }
}
