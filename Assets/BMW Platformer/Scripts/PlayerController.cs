using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using M2MqttUnity;
using uPLibrary.Networking.M2Mqtt.Messages;

namespace Platformer
{
    public class PlayerController : M2MqttUnityClient
    {
        public float movingSpeed;
        public float jumpForce;
        public float dashForce;
        public float dashTime;
        public float dashCooldown;

        private float moveInput;

        private bool facingRight = false;
        [HideInInspector]
        public bool deathState = false;

        private bool isGrounded;
        private bool isHuggingWall;
        public Transform groundCheck;
        public Transform wallCheck;

        private new Rigidbody2D rigidbody;
        private Animator animator;
        private GameManager gameManager;
        public GameObject minusPrefab;

        private bool isDashing = false;
        private bool canDash = true;
        private bool canDoubleJump = true;

        [SerializeField] private GameObject DashAnimation;
        [SerializeField] private GameObject DoubleJumpAnimation;

        protected override void Start()
        {
            base.Start(); 
            InitializeAndConnectMQTT();

            rigidbody = GetComponent<Rigidbody2D>();
            animator = GetComponent<Animator>();
            gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        }

        protected override void Update()
        {
            base.Update();

            CheckGround(); 

            if (Input.GetKeyDown(KeyCode.Space) && !isDashing && canDash)
            {
                StartCoroutine(Dash());
            }
            else
            {
                HandleMovementAndAnimations();
            }
        }

        private void HandleMovementAndAnimations()
        {
            rigidbody.velocity = new Vector2(rigidbody.velocity.x * Mathf.Pow(0.1f, Time.deltaTime), rigidbody.velocity.y);

            if (Input.GetButton("Horizontal") && !isDashing)
            {
                moveInput = Input.GetAxis("Horizontal");
                rigidbody.velocity = new Vector2(movingSpeed * moveInput, rigidbody.velocity.y);
                animator.SetInteger("playerState", 1); 
            }
            else
            {
                if (isGrounded && !isDashing)
                {
                    animator.SetInteger("playerState", 0);
                }
            }

            if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow))
            {
                HandleJump();
            }

            if (facingRight == false && moveInput > 0 && !isDashing)
            {
                Flip();
            }
            else if (facingRight == true && moveInput < 0 && !isDashing)
            {
                Flip();
            }
        }

        private void HandleJump()
        {
            if (isGrounded)
            {
                StartCoroutine(Jump());
            }
            else if (canDoubleJump)
            {
                StartCoroutine(Jump());
            }
        }

        private IEnumerator Jump()
        {
            if (isGrounded)
            {
                rigidbody.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
                canDoubleJump = true; 
            }
            else if (canDoubleJump)
            {
                DoubleJumpAnimation.SetActive(true);
                rigidbody.velocity = new Vector2(rigidbody.velocity.x, 0); 
                rigidbody.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
                canDoubleJump = false; 
                yield return new WaitForSeconds(0.5f);
                DoubleJumpAnimation.SetActive(false);
            }
        }

        private IEnumerator Dash()
        {
            if (canDash)
            {
                DashAnimation.SetActive(true);
                isDashing = true;
                canDash = false;

                float originalGravity = rigidbody.gravityScale;
                rigidbody.gravityScale = 0;

                Vector2 dashDirection = facingRight ? Vector2.right : Vector2.left;
                rigidbody.velocity = dashDirection * dashForce;

                yield return new WaitForSeconds(dashTime);
                DashAnimation.SetActive(false);
                rigidbody.gravityScale = originalGravity;
                rigidbody.velocity = Vector2.zero;
                isDashing = false;

                yield return new WaitForSeconds(dashCooldown);
                canDash = true;
            }
        }

        private void Flip()
        {
            Debug.Log("Flipping!"); 
            facingRight = !facingRight; 
            Vector3 scaler = transform.localScale;
            scaler.x *= -1;
            transform.localScale = scaler;
        }


        private void CheckGround()
        {
            Collider2D[] colliders = Physics2D.OverlapCircleAll(groundCheck.transform.position, 0.2f);
            isGrounded = colliders.Length > 1;

            if (isGrounded)
            {
                canDoubleJump = true;
            }
        }

        private void CheckWall()
        {
            Collider2D[] colliders = Physics2D.OverlapCircleAll(wallCheck.transform.position, 0.2f);
            isHuggingWall = colliders.Length > 1;
        }

        private void OnCollisionEnter2D(Collision2D other)
        {
            if (other.gameObject.tag == "Enemy")
            {
                deathState = true;
            }
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.gameObject.tag == "Coin")
            {
                Instantiate(minusPrefab, transform.position, Quaternion.identity);
            }
        }

        // MQTT Methods

        protected override void OnConnected()
        {
            base.OnConnected();
            SubscribeToTopic("Unity/ControllerBtn");
        }

        protected override void DecodeMessage(string topic, byte[] message)
        {
            string msg = System.Text.Encoding.UTF8.GetString(message);
            Debug.Log("Received message: " + msg + " from topic: " + topic);

            if (topic == "Unity/ControllerBtn")
            {
                HandleButtonPress(msg);
            }
        }

        private void HandleButtonPress(string button)
        {
            switch (button)
            {
                case "Left":
                    if (facingRight)
                    {
                        Flip();
                    }
                    rigidbody.velocity = new Vector2(-movingSpeed, rigidbody.velocity.y);
                    break;
                case "Right":
                    if (!facingRight)
                    {
                        Flip();
                    }
                    rigidbody.velocity = new Vector2(movingSpeed, rigidbody.velocity.y);
                    break;
                case "Up":
                    HandleJump(); 
                    break;
                case "A":
                    if (!isDashing && canDash)
                    {
                        StartCoroutine(Dash());
                    }
                    break;
                default:
                    Debug.LogWarning("Unhandled button: " + button);
                    break;
            }
        }


        protected void SubscribeToTopic(string topic)
        {
            if (client != null && client.IsConnected)
            {
                client.Subscribe(new string[] { topic }, new byte[] { MqttMsgBase.QOS_LEVEL_AT_LEAST_ONCE });
                Debug.Log("Subscribed to topic: " + topic);
            }
            else
            {
                Debug.LogWarning("Could not subscribe to topic, client is not connected.");
            }
        }

        private void InitializeAndConnectMQTT()
        {
            if (client == null)
            {
                Debug.LogError("MQTT client is not initialized.");
            }
            else
            {
                ConnectToBroker();
            }
        }

        private void ConnectToBroker()
        {
            if (client != null)
            {
                Debug.Log("Connecting to MQTT broker...");
                Connect();
            }
            else
            {
                Debug.LogError("MQTT client is not initialized.");
            }
        }

        private void DisconnectFromBroker()
        {
            if (client != null)
            {
                Disconnect();
            }
            else
            {
                Debug.LogError("MQTT client is not initialized.");
            }
        }
    }
}
