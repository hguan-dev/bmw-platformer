using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Platformer
{
    public class EnemyAI : MonoBehaviour
    {
        public float moveSpeed = 1f; 
        public LayerMask blocks;

        private Rigidbody2D rbody; 
        public Collider2D groundCollider;
        public Collider2D wallCollider;
        
        void Start()
        {
            rbody = GetComponent<Rigidbody2D>();
        }

        void Update()
        {
            rbody.velocity = new Vector2(moveSpeed, rbody.velocity.y);
        }

        void FixedUpdate()
        {
            if(!groundCollider.IsTouchingLayers(blocks) || wallCollider.IsTouchingLayers(blocks))
            {
                Flip();
            }
        }
        
        private void Flip()
        {
            transform.localScale = new Vector2(transform.localScale.x * -1, transform.localScale.y);
            moveSpeed *= -1;
        }
    }
}
