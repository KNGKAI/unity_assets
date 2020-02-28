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
    }

    public PhysicsBodySettings settings = new PhysicsBodySettings()
    {
        levelLerp = 0.5f,
        groundCheckResolution = 32,
        groundSphereShave = 0.01f
    };

    private static LayerMask mask = 0;

    private static float relativeVerticalVelocity;

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
            if (Physics.CheckCapsule(
                Position + Vector3.up * (Cap.radius * 2.0f),
                Position + Vector3.up * (value - Cap.radius),
                Cap.radius,
                Mask))
            {
                return;
            }
            value -= Radius;
            Cap.center = Vector3.up * (value / 2.0f + Cap.radius);
            Cap.height = value;
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

    public Vector3 Position
    {
        get
        {
            return (transform.position);
        }
        set
        {
            transform.position = value;
        }
    }

    private void CheckGrounded(out float level)
    {
        Vector3 position;

        level = 0.0f;
        grounded = false;
        if (Velocity.y > 0.0f)
        {
            ground.Act();
            return;
        }

        for (int i = settings.groundCheckResolution; i >= (Velocity.y > 0 ? 0 : -settings.groundCheckResolution); i--)
        {
            position = Position;
            level = (float)(i) / settings.groundCheckResolution * Radius;
            position.y += level + Radius;
            if (Physics.CheckSphere(position, Radius - settings.groundSphereShave, Mask))
            {
                level = position.y - Radius - settings.groundSphereShave;
                ground.Procces();
                grounded = !ground.Active;
                return;
            }
        }

        ground.Act();
    }

    public Vector2 PlaneVelocity
    {
        get
        {
            Vector3 v;

            v = Velocity;
            return (new Vector2(v.x, v.z));
        }
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

    private void FixedUpdate()
    {
        Vector3 v;

        CheckGrounded(out float level);

        if (Grounded)
        {
            v = Position;
            v.y = Mathf.Lerp(v.y, level, settings.levelLerp);

            relativeVerticalVelocity = v.y - Position.y;

            if (relativeVerticalVelocity > 0)
            {
                v.y += relativeVerticalVelocity;
            }

            if (v.y < Position.y ||
                !Physics.CheckCapsule(
                    Position + Vector3.up * (Radius * (2.0f + relativeVerticalVelocity)),
                    Position + Vector3.up * (Height - Radius + relativeVerticalVelocity),
                    Cap.radius,
                    Mask))
            {
                Position = v;
            }

            v = Velocity;
            v.y = 0.0f;
            Velocity = v;
        }
    }
}