namespace FuzzPhyte.UI.Camera
{
    using UnityEngine;

#if ENABLE_INPUT_SYSTEM
    using UnityEngine.InputSystem;
#endif

    public class FPUI_CameraControl : MonoBehaviour
    {
        [Header("Should be the parent item we are controlling")]
        public Transform LocalTransform;
        public float movementSpeed = 10f;
        public float rotationSpeed = 15f;
        public float dragSpeed = 0.1f;

        private bool isTouching = false;
        private Vector2 initialTouchPos;

        public Camera mainCamera;
        [SerializeField] private bool setup;
        [SerializeField] private bool useTouch;

        public virtual void Setup(Camera userSpecifiedCamera, bool client)
        {
            if (!client)
            {
                Destroy(this);
                return;
            }

            mainCamera = userSpecifiedCamera;
            mainCamera.enabled = true;

            if (Application.platform == RuntimePlatform.Android ||
                Application.platform == RuntimePlatform.IPhonePlayer)
            {
                useTouch = true;
            }
            else
            {
                useTouch = false;
            }

            setup = true;
        }

        public virtual void Update()
        {
            if (!setup) return;

            if (useTouch)
                HandleTouchInput();
            else
                HandleMouseKeyboardInput();
        }

        #region TOUCH INPUT

        public virtual void HandleTouchInput()
        {
#if ENABLE_INPUT_SYSTEM
            HandleTouchInput_NewSystem();
#elif ENABLE_LEGACY_INPUT_MANAGER
            HandleTouchInput_Legacy();
#endif
        }

        #region NEW_INPUT_SYSTEM

#if ENABLE_INPUT_SYSTEM
        private void HandleTouchInput_NewSystem()
        {
            if (Touchscreen.current == null) return;

            var touches = Touchscreen.current.touches;
            int activeTouches = 0;

            foreach (var touch in touches)
            {
                if (touch.press.isPressed)
                    activeTouches++;
            }

            if (activeTouches == 0)
            {
                isTouching = false;
                return;
            }

            var primaryTouch = Touchscreen.current.primaryTouch;

            if (primaryTouch.press.wasPressedThisFrame)
            {
                isTouching = true;
                initialTouchPos = primaryTouch.position.ReadValue();
            }

            if (primaryTouch.press.isPressed && isTouching)
            {
                Vector2 currentPos = primaryTouch.position.ReadValue();
                Vector2 delta = currentPos - initialTouchPos;

                RotateCamera(delta.x, 0);
                initialTouchPos = currentPos;
            }

            if (activeTouches == 2)
                MoveCamera(Vector3.forward);

            if (activeTouches == 3)
                MoveCamera(Vector3.back);
        }
#endif

        #endregion

        #region LEGACY_INPUT_SYSTEM

#if ENABLE_LEGACY_INPUT_MANAGER
        private void HandleTouchInput_Legacy()
        {
            if (Input.touchCount <= 0) return;

            Touch touch = Input.GetTouch(0);

            if (touch.phase == UnityEngine.TouchPhase.Began)
            {
                isTouching = true;
                initialTouchPos = touch.position;
            }

            if (touch.phase == UnityEngine.TouchPhase.Moved && isTouching)
            {
                Vector2 delta = touch.position - initialTouchPos;
                RotateCamera(delta.x, 0);
                initialTouchPos = touch.position;
            }

            if (Input.touchCount == 2 && touch.phase == UnityEngine.TouchPhase.Stationary)
                MoveCamera(Vector3.forward);

            if (Input.touchCount == 3 && touch.phase == UnityEngine.TouchPhase.Stationary)
                MoveCamera(Vector3.back);

            if (touch.phase == UnityEngine.TouchPhase.Ended || touch.phase == UnityEngine.TouchPhase.Canceled)
                isTouching = false;
        }
#endif

        #endregion

        #endregion

        #region MOUSE + KEYBOARD

        public virtual void HandleMouseKeyboardInput()
        {
#if ENABLE_INPUT_SYSTEM
            HandleMouseKeyboard_NewSystem();
#elif ENABLE_LEGACY_INPUT_MANAGER
            HandleMouseKeyboard_Legacy();
#endif
        }

        #region NEW_INPUT_SYSTEM

#if ENABLE_INPUT_SYSTEM
        private void HandleMouseKeyboard_NewSystem()
        {
            if (Keyboard.current == null || Mouse.current == null) return;

            Vector2 moveInput = Vector2.zero;

            if (Keyboard.current.wKey.isPressed) moveInput.y += 1;
            if (Keyboard.current.sKey.isPressed) moveInput.y -= 1;
            if (Keyboard.current.aKey.isPressed) moveInput.x -= 1;
            if (Keyboard.current.dKey.isPressed) moveInput.x += 1;

            MoveCamera(new Vector3(moveInput.x, 0, moveInput.y));

            if (Mouse.current.rightButton.isPressed)
            {
                Vector2 delta = Mouse.current.delta.ReadValue();
                RotateCamera(delta.x, delta.y);
            }
        }
#endif

        #endregion

        #region LEGACY_INPUT_SYSTEM

#if ENABLE_LEGACY_INPUT_MANAGER
        private void HandleMouseKeyboard_Legacy()
        {
            Vector3 move = new Vector3(
                Input.GetAxis("Horizontal"),
                0,
                Input.GetAxis("Vertical"));

            MoveCamera(move);

            if (Input.GetMouseButton(1))
            {
                float mouseX = Input.GetAxis("Mouse X");
                float mouseY = Input.GetAxis("Mouse Y");
                RotateCamera(mouseX, mouseY);
            }
        }
#endif

        #endregion

        #endregion

        #region CAMERA_MOVEMENT

        public virtual void MoveCamera(Vector3 direction)
        {
            LocalTransform.Translate(direction * movementSpeed * Time.deltaTime);
        }

        public virtual void RotateCamera(float deltaX, float deltaY)
        {
            float rotationX = deltaX * rotationSpeed * Time.deltaTime;
            float rotationY = -deltaY * rotationSpeed * Time.deltaTime;

            LocalTransform.Rotate(0, rotationX, 0);
            LocalTransform.Rotate(rotationY, 0, 0);
        }

        #endregion

        public virtual void LateUpdate() { }
    }
}