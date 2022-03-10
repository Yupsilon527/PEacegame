using System;
using System.Collections;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;

namespace UnityStandardAssets.Characters.FirstPerson
{
    [RequireComponent(typeof(Rigidbody))]
    [RequireComponent(typeof(CapsuleCollider))]
    public class RigidbodyFirstPersonController : MonoBehaviour
    {
        [Serializable]
        public class MovementSettings
        {
            public float ForwardSpeed = 8.0f;   // Speed when walking forward
            public float BackwardSpeed = 4.0f;  // Speed when walking backwards
            public float StrafeSpeed = 4.0f;    // Speed when walking sideways
            public float AirControlSpeed = 2.0f;    // Speed when flying in the air (added during the ConBITi Games Jam)
            public float RunMultiplier = 2.0f;   // Speed when sprinting
            public KeyCode RunKey= KeyCode.LeftShift;
            public float JumpForce = 50f;
            public AnimationCurve SlopeCurveModifier = new AnimationCurve(new Keyframe(-90.0f, 1.0f), new Keyframe(0.0f, 1.0f), new Keyframe(90.0f, 0.0f));
            [HideInInspector] public float CurrentTargetSpeed = 8f;

#if !MOBILE_INPUT
            private bool m_Running;
#endif

            public void UpdateDesiredTargetSpeed(Vector2 input)
            {
                if (input == Vector2.zero) return;
                if (input.x > 0 || input.x < 0)
                {
                    //strafe
                    CurrentTargetSpeed = StrafeSpeed;
                }
                if (input.y < 0)
                {
                    //backwards
                    CurrentTargetSpeed = BackwardSpeed;
                }
                if (input.y > 0)
                {
                    //forwards
                    //handled last as if strafing and moving forward at the same time forwards speed should take precedence
                    CurrentTargetSpeed = ForwardSpeed;
                }
#if !MOBILE_INPUT
                if (Input.GetKey(RunKey))
                {
                    CurrentTargetSpeed *= RunMultiplier;
                    m_Running = true;
                }
                else
                {
                    m_Running = false;
                }
#endif
            }

#if !MOBILE_INPUT
            public bool Running
            {
                get { return m_Running; }
            }
#endif
        }


        [Serializable]
        public class AdvancedSettings
        {
            public float groundCheckDistance = 0.01f; // distance for checking if the controller is grounded ( 0.01f seems to work best for this )
            public float stickToGroundHelperDistance = 0.5f; // stops the character
            public float slowDownRate = 20f; // rate at which the controller comes to a stop when there is no input
            public bool airControl; // can the user control the direction that is being moved in the air
            [Tooltip("set it to 0.1 or more if you get stuck in wall")]
            public float shellOffset; //reduce the radius by that ratio to avoid getting stuck in wall (a value of 0.1f is nice)
        }


        public Camera cam;
        public MovementSettings movementSettings = new MovementSettings();
        public MouseLook mouseLook = new MouseLook();
        public AdvancedSettings advancedSettings = new AdvancedSettings();


        private Rigidbody m_RigidBody;
        private CapsuleCollider m_Capsule;
        private float m_YRotation;
        private Vector3 m_GroundContactNormal;
        /*[HideInInspector]*/ public bool m_Flying; // Flying state toggle (added during the ConBITi Games Jam)
        private bool m_Jump, m_PreviouslyGrounded, m_Jumping, m_IsGrounded;


        public Vector3 Velocity
        {
            get { return m_RigidBody.velocity; }
        }

        public bool Grounded
        {
            get { return m_IsGrounded; }
        }

        public bool Jumping
        {
            get { return m_Jumping; }
        }

        public bool Running
        {
            get
            {
#if !MOBILE_INPUT
                return movementSettings.Running;
#else
	            return false;
#endif
            }
        }


        private void Start()
        {
            m_RigidBody = GetComponent<Rigidbody>();
            m_Capsule = GetComponent<CapsuleCollider>();
            mouseLook.Init(transform, cam.transform);
        }


        private void Update()
        {
            RotateView();

            if (CrossPlatformInputManager.GetButtonDown("Jump") && !m_Jump)
            {
                m_Jump = true;
            }
        }

        public float localVelocityX;
        public float localVelocityY;
        public float localVelocityZ;

        public float forceZ;
        public float forceX;

        private void FixedUpdate()
        {
            GroundCheck();

            // If the player is not jumping, changes the player's state to 'Flying' (added during the ConBITi Games Jam)
            if (!m_IsGrounded && !m_Jumping)
                m_Flying = true;

            Vector2 input = GetInput();

            // Player movement control when flying in the air (added during the ConBITi Games Jam)
            if ((Mathf.Abs(input.x) > float.Epsilon || Mathf.Abs(input.y) > float.Epsilon) && advancedSettings.airControl && !m_IsGrounded/* && m_Flying*/)
            {
                // always move along the camera forward as it is the direction that it being aimed at
                Vector3 desiredMove = cam.transform.forward * input.y + cam.transform.right * input.x;
                desiredMove = Vector3.ProjectOnPlane(desiredMove, m_GroundContactNormal).normalized;

                if (!(m_RigidBody.velocity.x > -movementSettings.AirControlSpeed && desiredMove.x < 0) ||
                    !(m_RigidBody.velocity.x < movementSettings.AirControlSpeed && desiredMove.x > 0))
                {
                    m_RigidBody.AddForce(new Vector3(desiredMove.x * movementSettings.AirControlSpeed, 0f, 0f), ForceMode.Impulse);
                }
                if (!(m_RigidBody.velocity.z > -movementSettings.AirControlSpeed && desiredMove.z < 0) ||
                    !(m_RigidBody.velocity.z < movementSettings.AirControlSpeed && desiredMove.z > 0))
                {
                    m_RigidBody.AddForce(new Vector3(0f, 0f, desiredMove.z * movementSettings.AirControlSpeed), ForceMode.Impulse);
                }
            }

            #region Draft code
            //if (Mathf.Abs(input.y) > float.Epsilon && (Math.Abs(localVelocityZ) < movementSettings.CurrentTargetSpeed || (localVelocityZ <= float.Epsilon && input.y > float.Epsilon) || (localVelocityZ >= float.Epsilon && input.y < float.Epsilon)))
            //{                    
            //    desiredMove.z = desiredMove.z * movementSettings.CurrentTargetSpeed;
            //    forceZ = desiredMove.z;
            //    forceX = desiredMove.x;
            //}
            //if (Mathf.Abs(input.x) > float.Epsilon && (Math.Abs(localVelocityX) < movementSettings.CurrentTargetSpeed || (localVelocityX <= float.Epsilon && input.x > float.Epsilon) || (localVelocityX >= float.Epsilon && input.x < float.Epsilon)))
            //{
            //    desiredMove.x = desiredMove.x * movementSettings.CurrentTargetSpeed;
            //    forceZ = desiredMove.z;
            //    forceX = desiredMove.x;
            //}
            //if (m_RigidBody.velocity.sqrMagnitude <
            //(movementSettings.CurrentTargetSpeed * movementSettings.CurrentTargetSpeed))
            //{
            //    m_RigidBody.AddForce(desiredMove * SlopeMultiplier(), ForceMode.Impulse);
            //}
            #endregion
            // Player movement control in all other cases (some tweaks of the initial code during the ConBITi Games Jam)
            else if ((Mathf.Abs(input.x) > float.Epsilon || Mathf.Abs(input.y) > float.Epsilon) && (m_IsGrounded || m_Jumping)) // Changed during the ConBITi Games Jam. Initial bool was '(m_IsGrounded || advancedSettings.airControl)' 
            {
                // always move along the camera forward as it is the direction that it being aimed at
                Vector3 desiredMove = cam.transform.forward * input.y + cam.transform.right * input.x;
                desiredMove = Vector3.ProjectOnPlane(desiredMove, m_GroundContactNormal).normalized;

                //if (Input.GetKey(movementSettings.RunKey) && !m_IsGrounded)
                //    movementSettings.CurrentTargetSpeed /= movementSettings.RunMultiplier;

                desiredMove.x = desiredMove.x * movementSettings.CurrentTargetSpeed;
                desiredMove.z = desiredMove.z * movementSettings.CurrentTargetSpeed;
                desiredMove.y = desiredMove.y * movementSettings.CurrentTargetSpeed;
                if (m_RigidBody.velocity.sqrMagnitude <
                    (movementSettings.CurrentTargetSpeed * movementSettings.CurrentTargetSpeed))
                {
                    m_RigidBody.AddForce(desiredMove * SlopeMultiplier(), ForceMode.Impulse);
                }
            }

            if (m_IsGrounded)
            {
                m_RigidBody.drag = 5f;

                if (m_Jump)
                {
                    m_RigidBody.drag = 0f;
                    m_RigidBody.velocity = new Vector3(m_RigidBody.velocity.x, 0f, m_RigidBody.velocity.z);
                    m_RigidBody.AddForce(new Vector3(0f, movementSettings.JumpForce, 0f), ForceMode.Impulse);
                    m_Jumping = true;
                    StartCoroutine("JumpTimer"); // Small tweak here to check if the player is flying  (added during the ConBITi Games Jam)
                }

                if (!m_Jumping && !m_Flying && Mathf.Abs(input.x) < float.Epsilon && Mathf.Abs(input.y) < float.Epsilon && m_RigidBody.velocity.magnitude < 1f)
                {
                    m_RigidBody.Sleep();
                }
            }
            else
            {
                m_RigidBody.drag = 0f;
                if (m_PreviouslyGrounded && !m_Jumping && !m_Flying) // !m_Flying added during the ConBITi Games Jam
                {
                    StickToGroundHelper();
                }
            }
            m_Jump = false;
        }
        // Changes player to flying state if is in the air 1 second after a jump (added during the ConBITi Games Jam)
        IEnumerator JumpTimer()
        {
            yield return new WaitForSeconds(1f);
            if (!m_IsGrounded)
                m_Flying = true;
        }


        private float SlopeMultiplier()
        {
            float angle = Vector3.Angle(m_GroundContactNormal, Vector3.up);
            return movementSettings.SlopeCurveModifier.Evaluate(angle);
        }


        private void StickToGroundHelper()
        {
            RaycastHit hitInfo;
            if (Physics.SphereCast(transform.position, m_Capsule.radius * (1.0f - advancedSettings.shellOffset), Vector3.down, out hitInfo,
                                   ((m_Capsule.height / 2f) - m_Capsule.radius) +
                                   advancedSettings.stickToGroundHelperDistance, Physics.AllLayers, QueryTriggerInteraction.Ignore))
            {
                if (Mathf.Abs(Vector3.Angle(hitInfo.normal, Vector3.up)) < 85f)
                {
                    m_RigidBody.velocity = Vector3.ProjectOnPlane(m_RigidBody.velocity, hitInfo.normal);
                }
            }
        }


        private Vector2 GetInput()
        {

            Vector2 input = new Vector2
            {
                x = CrossPlatformInputManager.GetAxis("Horizontal"),
                y = CrossPlatformInputManager.GetAxis("Vertical")
            };
            movementSettings.UpdateDesiredTargetSpeed(input);
            return input;
        }


        private void RotateView()
        {
            //avoids the mouse looking if the game is effectively paused
            if (Mathf.Abs(Time.timeScale) < float.Epsilon) return;

            // get the rotation before it's changed
            float oldYRotation = transform.eulerAngles.y;

            mouseLook.LookRotation(transform, cam.transform);

            // removed ' || advancedSettings.airControl' drung ConBITi Games Jam to fix object flying behaviour during long jumps
            if (m_IsGrounded)
            {
                // Rotate the rigidbody velocity to match the new direction that the character is looking
                Quaternion velRotation = Quaternion.AngleAxis(transform.eulerAngles.y - oldYRotation, Vector3.up);
                m_RigidBody.velocity = velRotation * m_RigidBody.velocity;
            }
        }

        /// sphere cast down just beyond the bottom of the capsule to see if the capsule is colliding round the bottom
        private void GroundCheck()
        {
            m_PreviouslyGrounded = m_IsGrounded;
            RaycastHit hitInfo;
            if (Physics.SphereCast(transform.position, m_Capsule.radius * (1.0f - advancedSettings.shellOffset), Vector3.down, out hitInfo,
                                   ((m_Capsule.height / 2f) - m_Capsule.radius) + advancedSettings.groundCheckDistance, Physics.AllLayers, QueryTriggerInteraction.Ignore))
            {
                m_IsGrounded = true;
                m_GroundContactNormal = hitInfo.normal;
            }
            else
            {
                m_IsGrounded = false;
                m_GroundContactNormal = Vector3.up;
            }
            if (!m_PreviouslyGrounded && m_IsGrounded && m_Jumping)
            {
                m_Jumping = false;
            }
            // Disable flying state (added during the ConBITi Games Jam)
            if (!m_PreviouslyGrounded && m_IsGrounded && m_Flying)
            {
                m_Flying = false;
            }
        }
    }
}
