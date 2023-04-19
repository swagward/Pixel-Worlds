using UnityEngine;

namespace PixelWorlds.Runtime.Player
{
    public class PlayerController : MonoBehaviour
    {
        [Header("Player Control")] 
        [SerializeField] private float speed;
        public Vector2Int mousePos;
        public int playerReach;

        [Header("Jump/Ground Detection")] 
        [SerializeField] private Transform groundCheck;
        [SerializeField] private LayerMask groundLayer;
        [SerializeField] private float jumpForce;
        private const KeyCode JumpKey = KeyCode.Space;
        
        //Misc
        private Rigidbody2D _rb2;
        private bool _facingRight;
        private float _horizontal;
        private Camera _mainCam;
        
        public void Spawn(int x, int y)
        {
            transform.position = new Vector2(x, y + 3);
            _mainCam = Camera.main;

            _rb2 = GetComponent<Rigidbody2D>();
            _rb2.freezeRotation = true;
        }

        private void Update()
        {
            if (PauseControl.IsPaused) return;

            _horizontal = Input.GetAxisRaw("Horizontal");

            //Get tiles at specific mouse position
            var worldPos = _mainCam.ScreenToWorldPoint(Input.mousePosition);
            mousePos.x = Mathf.RoundToInt(worldPos.x - .5f);
            mousePos.y = Mathf.RoundToInt(worldPos.y - .5f);
        }

        private void FixedUpdate()
            => _rb2.velocity = new Vector2(_horizontal * speed, _rb2.velocity.y);
    }
}
