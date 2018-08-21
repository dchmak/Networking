/*
 * Created by Brackeys - https://github.com/Brackeys/2D-Character-Controller
 * Modified by Daniel Mak
 */

using UnityEngine;
using UnityEngine.Events;

namespace Daniel.Character2D {
    [RequireComponent(typeof(Rigidbody2D))]
    public class MovementController : MonoBehaviour {

        [Header("Jumping")]
        [SerializeField] [Tooltip("Whether the player have the ability to jump.")] private bool canJump = true;
        [SerializeField] [Tooltip("The maximum height the player can jump.")] [ConditionalHide("canJump", true)] [Min(0)] private float jumpHeight = 10f;
        [SerializeField] [Tooltip("The time needed to reach the maximum height.")] [ConditionalHide("canJump", true)] [Min(0)] private float jumpDuration = 1f;
        [SerializeField] [Tooltip("Gravity multiplier when falling.")] [ConditionalHide("canJump", true)] [Min(0)] private float fallMultiplier = 2.5f;
        [SerializeField] [Tooltip("Gravity multiplier when falling.")] [ConditionalHide("canJump", true)] [Min(0)] private float lowJumpMultiplier = 2f;
        [SerializeField] [Tooltip("Whether or not a player can steer while jumping.")] [ConditionalHide("canJump", true)] private bool airControl = false;
        [Range(0, 2)] [SerializeField] [Tooltip("Percentage of the max speed applied to movement while airborne. 1 = 100%")] [ConditionalHide("canJump", true)] [Min(0)] private float airborneSpeedPenalty = .36f;
        [SerializeField] [Tooltip("A mask determining what is ground to the character")] [ConditionalHide("canJump", true)] private LayerMask whatIsGround;
        [SerializeField] [Tooltip("A position marking where to check if the player is grounded.")] [ConditionalHide("canJump", true)] private Transform groundCheck;
        [SerializeField] [Tooltip("A window when the player leaving the ground but still can jump.")] [ConditionalHide("canJump", true)] [Min(0)] private float leaveGroundLeniency = 0.05f;
        [SerializeField] [Tooltip("A window when the player press jump early but still can jump.")] [ConditionalHide("canJump", true)] [Min(0)] private float earlyJumpLeniency = 0.05f;


        [Header("Crouching")]
        [SerializeField] [Tooltip("Whether the player have the ability to crouch.")] private bool canCrouch = true;
        [Range(0, 2)] [SerializeField] [Tooltip("Percentage of the max speed applied to crouching movement. 1 = 100%")] [ConditionalHide("canCrouch", true)] private float crouchSpeedPenalty = .36f;
        [SerializeField] [Tooltip("A position marking where to check for ceilings")] [ConditionalHide("canCrouch", true)] private Transform ceilingCheck;
        [SerializeField] [Tooltip("A collider that will be disabled when crouching")] [ConditionalHide("canCrouch", true)] private Collider2D crouchDisableCollider;

        [Header("Speed")]
        [SerializeField] [Tooltip("Horizontal speed of the player.")] private float speed = 1f;

        [Header("Smoothing")]
        [Range(0, .3f)] [SerializeField] [Tooltip("How much to smooth out the movement.")] private float movementSmoothing = .05f;

        [Header("Events")]
        [Space]

        public UnityEvent OnJumpEvent;
        public UnityEvent OnLandingEvent;

        [System.Serializable]
        public class BoolEvent : UnityEvent<bool> { }

        public BoolEvent OnCrouchEvent;

        private float defaultGravity = 1f;
        private bool wasCrouching = false;
        private const float groundedRadius = .2f;   // Radius of the overlap circle to determine if grounded
        private bool grounded;                      // Whether or not the player is grounded.
        private const float ceilingRadius = .2f;    // Radius of the overlap circle to determine if the player can stand up
        private Rigidbody2D rb;
        private bool facingRight = true;            // For determining which way the player is currently facing.
        private Vector3 velocity = Vector3.zero;
        private float lastPressJumpTime;
        private float lastGroundedTime;

        /// <summary>
        /// Move the game object.
        /// </summary>
        /// <param name="horizontalInput">The horizontal speed of the game object</param>
        /// <param name="crouch">Whether the game object is crouching.</param>
        /// <param name="jump">Whether the game object is jumping.</param>
        public void Move(float horizontalInput, bool crouch, bool jump) {
            if (!canCrouch) crouch = false;
            if (!canJump) jump = false;

            if (jump) lastPressJumpTime = earlyJumpLeniency;

            // If crouching, check to see if the character can stand up
            if (wasCrouching && !crouch) {
                // If the character has a ceiling preventing them from standing up, keep them crouching
                if (Physics2D.OverlapCircle(ceilingCheck.position, ceilingRadius, whatIsGround)) {
                    crouch = true;
                }
            }

            //only control the player if grounded or airControl is turned on
            if (grounded || airControl) {

                horizontalInput *= speed;

                // If crouching
                if (crouch) {
                    if (!wasCrouching) {
                        wasCrouching = true;
                        OnCrouchEvent.Invoke(true);
                    }

                    // Reduce the speed
                    horizontalInput *= crouchSpeedPenalty;

                    // Disable one of the colliders when crouching
                    if (crouchDisableCollider != null)
                        crouchDisableCollider.enabled = false;
                } else {
                    // Enable the collider when not crouching
                    if (crouchDisableCollider != null)
                        crouchDisableCollider.enabled = true;

                    if (wasCrouching) {
                        wasCrouching = false;
                        OnCrouchEvent.Invoke(false);
                    }
                }

                // If airborne
                if (!grounded) {
                    // Reduce the speed
                    horizontalInput *= airborneSpeedPenalty;
                }

                // Move the character by finding the target velocity
                Vector3 targetVelocity = new Vector2(horizontalInput * 10f, rb.velocity.y);
                // And then smoothing it out and applying it to the character
                rb.velocity = Vector3.SmoothDamp(rb.velocity, targetVelocity, ref velocity, movementSmoothing);

                // If the input is moving the player right and the player is facing left or if the input is moving the player left and the player is facing right...
                if ((horizontalInput > 0 && !facingRight) || (horizontalInput < 0 && facingRight)) {
                    // ... flip the player.
                    Flip();
                }
            }

            // If the player should jump...
            if (lastPressJumpTime > 0 && lastGroundedTime > 0) {
                lastPressJumpTime = 0;
                lastGroundedTime = 0;

                // Add a vertical force to the player.
                grounded = false;
                rb.velocity = new Vector2(rb.velocity.x, 2 * jumpHeight / jumpDuration);

                OnJumpEvent.Invoke();
            }

            // Alter gravity to give snappier movement
            if (rb.velocity.y < 0) rb.gravityScale = defaultGravity * fallMultiplier;
            else if (!jump) rb.gravityScale = defaultGravity * lowJumpMultiplier;
            else rb.gravityScale = defaultGravity;

            if (lastPressJumpTime > 0) lastPressJumpTime -= Time.deltaTime;
            if (lastGroundedTime > 0) lastGroundedTime -= Time.deltaTime;
        }

        private void Awake() {
            rb = GetComponent<Rigidbody2D>();

            if (OnLandingEvent == null)
                OnLandingEvent = new UnityEvent();

            if (OnCrouchEvent == null)
                OnCrouchEvent = new BoolEvent();
        }

        private void Update() {
            defaultGravity = (2 * jumpHeight) / (jumpDuration * jumpDuration);
        }

        private void FixedUpdate() {
            bool wasGrounded = grounded;
            grounded = false;

            // The player is grounded if a circlecast to the groundcheck position hits anything designated as ground
            // This can be done using layers instead but Sample Assets will not overwrite your project settings.
            Collider2D[] colliders = Physics2D.OverlapCircleAll(groundCheck.position, groundedRadius, whatIsGround);
            for (int i = 0; i < colliders.Length; i++) {
                if (colliders[i].gameObject != gameObject) {
                    grounded = true;
                    lastGroundedTime = leaveGroundLeniency;

                    if (!wasGrounded) {
                        OnLandingEvent.Invoke();
                    }
                }
            }
        }

        private void Flip() {
            // Switch the way the player is labelled as facing.
            facingRight = !facingRight;

            // Multiply the player's x local scale by -1.
            Vector3 theScale = transform.localScale;
            theScale.x *= -1;
            transform.localScale = theScale;
        }

        private void OnValidate() {
            if (groundCheck != null) groundCheck.name = "Ground Check";
            if (ceilingCheck != null) ceilingCheck.name = "Ceiling Check";
        }
    }
}