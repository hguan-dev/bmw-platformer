using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Platformer
{
    public class PlayerDeathState : MonoBehaviour
    {
        public float jumpForce;

        private Rigidbody2D rbody;
        void Start()
        {
            rbody = GetComponent<Rigidbody2D>();
            rbody.AddForce(transform.up * jumpForce, ForceMode2D.Impulse);
        }
    }
}

