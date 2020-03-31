using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody)), RequireComponent(typeof(CapsuleCollider))]
public class CharacterPhysicsBody : MonoBehaviour
{
    public static readonly float MinRadius = 0.1f;

    [System.Serializable]
    public struct PhysicsBodySettings
    {
        [Range(0, 1)]
        public float levelLerp;
        public int groundCheckResolution;
        public float groundSphereShave;
        public bool checkHeightChange;
    }

    public PhysicsBodySettings settings = new PhysicsBodySettings()
    {
        levelLerp = 0.5f,
        groundCheckResolution = 32,
        groundSphereShave = 0.01f,
        checkHeightChange = true
    };

    public enum PositionalType
    {
        Normal,
        Translate,
        CheckTranslate
    }

    public PositionalType positionType;

    private static LayerMask mask = 0;

    public static LayerMask Mask
    {
        get
        {
            if (mask == 0)
            {
                mask = ~(1 << LayerMask.NameToLayer("Character"));
            }
            return (mask);
        }
    }

    private Rigidbody rig;

    public Rigidbody Rig
    {
        get
        {
            if (rig == null)
            {
                rig = GetComponent<Rigidbody>();
                rig.constraints = RigidbodyConstraints.FreezeRotation;
                rig.useGravity = false;
            }
            return (rig);
        }
    }

    private CapsuleCollider cap;

    public CapsuleCollider Cap
    {
        get
        {
            if (cap == null)
            {
                cap = GetComponent<CapsuleCollider>();
                cap.material = new PhysicMaterial("zero")
                {
                    dynamicFriction = 0.0f,
                    staticFriction = 0.0f,
                    bounciness = 0.0f,
                    frictionCombine = PhysicMaterialCombine.Minimum,
                    bounceCombine = PhysicMaterialCombine.Minimum
                };
            }
            return (cap);
        }
    }

    public float Radius
    {
        get
        {
            return (Cap.radius);
        }
        set
        {
            float h;

            if (value == Radius)
            {
                return;
            }
            h = Height;
            Cap.radius = Mathf.Clamp(value, MinRadius, Cap.height / 2.0f);
            Height = h;
        }
    }

    public float Height
    {
        get
        {
            return (Cap.height + Cap.radius);
        }
        set
        {
            if (value == Height)
            {
                return;
            }
            if (value < Radius * 2.0f)
            {
                value = Radius * 2.0f;
            }
            if (settings.checkHeightChange && value > Height)
            {
                if (Physics.CheckCapsule(
                    Position + Vector3.up * (Cap.radius * 2.0f),
                    Position + Vector3.up * (value - Cap.radius),
                    Cap.radius,
                    Mask))
                {
                    return;
                }
            }
            value -= Radius;
            Cap.center = Vector3.up * (value / 2.0f + Cap.radius);
            Cap.height = value;
        }
    }

    public Vector3 Position
    {
        get
        {
            return (transform.position);
        }
        set
        {
            switch (positionType)
            {
                case PositionalType.Normal:
                    transform.position = value;
                    break;
                case PositionalType.Translate:
                    transform.Translate(value - transform.position);
                    break;
                case PositionalType.CheckTranslate:
                    Vector3 v, p;

                    v = value - transform.position;
                    p = transform.position + v;

                    if (!Physics.CheckCapsule(
                        Position + Vector3.up * (Cap.radius * 2.0f),
                        Position + Vector3.up * (Height - Cap.radius),
                        Cap.radius,
                        Mask))
                    {
                        transform.Translate(v);
                    }
                    break;
                default:
                    break;
            }
        }
    }

    public Vector3 Velocity
    {
        get
        {
            return (Rig.velocity);
        }
        set
        {
            Rig.velocity = value;
        }
    }

    public float Speed
    {
        get
        {
            return (Rig.velocity.magnitude);
        }
        set
        {
            Velocity = Velocity.normalized * value;
        }
    }

    private Ticker ground;

    private bool grounded;

    public bool Grounded { get { return (grounded); } }

    public bool JumpGrounded { get { return (ground.Tick < Ticker.DefaultTick); } }

    private Vector3 groundNormal;

    public Vector3 GroundNormal
    {
        get
        {
            return (groundNormal);
        }
    }

    private float groundNormalAngle;

    public float GroundNormalAngle
    {
        get
        {
            return (groundNormalAngle);
        }
    }

    private void CheckGrounded(out float level)
    {
        RaycastHit[] hits;
        Vector3 position;

        level = Position.y;
        grounded = false;
        groundNormal = Vector3.up;
        groundNormalAngle = 0.0f;

        position = Position;
        position.y = Radius;

        /*
        if (Physics.SphereCast(position, Radius - settings.groundSphereShave, Vector3.down, out RaycastHit hit, Radius * Radius, Mask))
        {
            level = position.y - hit.distance;

            ground.Procces();
            grounded = !ground.Active;
            if (grounded)
            {
                groundNormal = hit.normal;
            }
        }
        */

        for (int i = settings.groundCheckResolution; i >= (Velocity.y > 0 ? 0 : -settings.groundCheckResolution); i--)
        {
            position = Position;
            level = (float)(i) / settings.groundCheckResolution * Radius;
            position.y += level + Radius;

            hits = Physics.SphereCastAll(position, Radius - settings.groundSphereShave, Vector3.down, settings.groundSphereShave, Mask);

            if (hits.Length > 0)
            {
                level = position.y - Radius - settings.groundSphereShave;

                ground.Procces();
                grounded = !ground.Active;

                groundNormal = Vector3.zero;
                foreach (RaycastHit h in hits)
                {
                    groundNormal += h.normal;
                }
                groundNormal /= hits.Length;
                groundNormal.Normalize();

                groundNormalAngle = Vector3.Angle(groundNormal, Vector3.up);

                return;
            }
        }

        ground.Act();
    }

    public Vector2 PlaneVelocity
    {
        get
        {
            return (new Vector2(Velocity.x, Velocity.z));
        }
    }

    public float PlaneSpeed
    {
        get
        {
            return (Mathf.Sqrt(Velocity.x * Velocity.x + Velocity.z * Velocity.z));
        }
        set
        {
            Vector3 v;
            float y;

            v = Velocity;
            y = v.y;
            v.y = 0;

            v = v.normalized * value;
            v.y = y;

            Velocity = v;
        }
    }

    public bool Wall()
    {
        return (false);
    }

    public void SetPlaneVelocity(float x, float y)
    {
        Velocity = new Vector3(x, Velocity.y, y);
    }

    public void Freeze()
    {
        Velocity = Vector3.zero;
        Rig.Sleep();
    }

    public void Drag(float amount)
    {
        Vector3 v;

        if (Speed == 0)
        {
            return;
        }
        if (Speed < amount)
        {
            SetPlaneVelocity(0, 0);
            return;
        }

        v = Velocity;

        float Axis(float a)
        {
            return ((a > 0 ? amount : -amount) * Mathf.Abs(a) / v.magnitude);
        }

        v -= new Vector3(Axis(v.x), 0.0f, Axis(v.z));
        Velocity = v;
    }

    public void Strafe(float a)
    {
        transform.Rotate(0, a, 0, Space.World);
        Velocity = Quaternion.Euler(0, a, 0) * Velocity;

        a = Mathf.Abs(a) * Time.deltaTime;

        if (PlaneSpeed > 0)
        {
            PlaneSpeed += a;
        }
        else
        {
            SetPlaneVelocity(transform.forward.x * a, transform.forward.z * a);
        }
    }

    private void FixedUpdate()
    {
        Vector3 v;

        CheckGrounded(out float level);

        if (Grounded)
        {
            v = Position;
            v.y = Mathf.Lerp(v.y, level + Velocity.y * Time.fixedDeltaTime, settings.levelLerp);
            Position = v;
        }
    }
}