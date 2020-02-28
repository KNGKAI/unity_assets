using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterControll : CharacterCamera
{
    [System.Serializable]
    public struct MovementSettings
    {
        public enum MovementType
        {
            Accelerate,
            Immediate,
            Continuous,
            Tank
        }

        public float speed;
        public float sprint;
        public float accel;
        public float drag;
        public float jump;
        public float crouch;
        public float rotate;

        public MovementType moveType;
        [Space(3)]
        public float inputDeadzone;
        public float moveDeadzone;
    }

    public MovementSettings movementSettings = new MovementSettings
    {
        speed = 5.0f,
        sprint = 10.0f,
        accel = 2.0f,
        drag = 4.0f,
        jump = 5.0f,
        crouch = 5.0f,
        rotate = 3.0f,
        moveType = MovementSettings.MovementType.Immediate,
        inputDeadzone = 0.01f,
        moveDeadzone = 0.1f
    };

    private Vector2 move;

    protected Ticker moving;

    public bool Moving
    {
        get
        {
            return (moving.Active);
        }
    }

    protected Ticker crouching;

    public bool Crouching
    {
        get
        {
            return (crouching.Active);
        }
    }

    protected Ticker sprinting;

    public bool Sprinting
    {
        get
        {
            return (sprinting.Active);
        }
    }

    private void CrouchCharacter(bool down)
    {
        float c;

        c = down ? -movementSettings.crouch : movementSettings.crouch;

        Body.Height += c * Time.fixedDeltaTime;
        Camera.transform.localPosition = Vector3.up * (Body.Height - Body.Radius);

        if (!Body.Grounded)
        {
            transform.Translate(0.0f, -c * Time.fixedDeltaTime, 0.0f);
        }
    }

    private void FixedUpdate()
    {
        if (Crouching)
        {
            if (crouching.Tick >= 2 && Body.Height > characterSettings.crouchHeight)
            {
                CrouchCharacter(true);
            }
        }
        else if (Body.Height < characterSettings.height)
        {
            CrouchCharacter(false);
        }

        if (Body.Grounded)
        {
            float s = Sprinting ? movementSettings.sprint : movementSettings.speed;

            if (Moving)
            {
                switch (movementSettings.moveType)
                {
                    case MovementSettings.MovementType.Accelerate:
                        Vector3 v = Body.Velocity;

                        v.x += move.x * movementSettings.accel * Time.fixedDeltaTime;
                        v.z += move.y * movementSettings.accel * Time.fixedDeltaTime;

                        if (v.magnitude > s)
                        {
                            v = v.normalized * s;
                        }

                        Body.SetPlaneVelocity(v.x, v.z);
                        break;
                    case MovementSettings.MovementType.Immediate:
                        Body.SetPlaneVelocity(move.x * s, move.y * s);
                        break;
                    case MovementSettings.MovementType.Continuous:
                        Body.SetPlaneVelocity(move.x * s, move.y * s);
                        break;
                    case MovementSettings.MovementType.Tank:
                        Body.SetPlaneVelocity(move.x * s, move.y * s);
                        break;
                    default:
                        break;
                }
            }
            else
            {
                if (Body.Speed < movementSettings.moveDeadzone)
                {
                    Body.Freeze();
                }
                else
                {
                    switch (movementSettings.moveType)
                    {
                        case MovementSettings.MovementType.Accelerate:
                            Body.Drag(movementSettings.drag * Time.fixedDeltaTime);
                            move.x = Body.Velocity.x / s;
                            move.y = Body.Velocity.z / s;
                            break;
                        case MovementSettings.MovementType.Immediate:
                            Body.Drag(movementSettings.drag * Time.fixedDeltaTime);
                            break;
                        case MovementSettings.MovementType.Continuous:
                            break;
                        case MovementSettings.MovementType.Tank:
                            Body.Drag(movementSettings.drag * Time.fixedDeltaTime);
                            break;
                        default:
                            break;
                    }
                }
            }
        }
    }

    public void Rotate(float x, float y)
    {
        RotateCamera(Mathf.Clamp(x, -1.0f, 1.0f) * cameraSettings.rotateX, Mathf.Clamp(y, -1.0f, 1.0f) * cameraSettings.rotateY);
        if (Shooter)
        {
            transform.Rotate(0.0f, Mathf.Clamp(y, -1.0f, 1.0f) * cameraSettings.rotateY, 0.0f, Space.Self);
        }
    }

    public void Move(float x, float y)
    {
        Transform t;
        Vector3 m;
        float a;

        if (!Body.Grounded || (Mathf.Abs(x) < movementSettings.inputDeadzone && Mathf.Abs(y) < movementSettings.inputDeadzone))
        {
            return;
        }

        t = Shooter || movementSettings.moveType == MovementSettings.MovementType.Tank ? transform : Camera.transform;
        
        if (movementSettings.moveType == MovementSettings.MovementType.Tank)
        {
            transform.Rotate(0.0f, x * movementSettings.rotate, 0.0f);
            x = 0;
        }

        m = t.TransformDirection(x, 0, y);

        m.y = 0;
        if (m.magnitude > 1)
        {
            m.Normalize();
        }

        move.x = m.x;
        move.y = m.z;

        moving.Act();

        if (!Shooter)
        {
            a = Vector3.Angle(transform.forward, m);
            if (Vector3.Angle(transform.right, m) > 90.0f)
            {
                a *= -1;
            }
            transform.Rotate(0.0f, a, 0.0f);
        }
    }

    public void StopMove()
    {
        move = Vector2.zero;
        moving.Kill();
    }

    public void Jump()
    {
        Vector3 v;

        if (!Body.Grounded)
        {
            return;
        }
        v = Body.Velocity;
        v.y = movementSettings.jump;
        Body.Velocity = v;
    }

    public void Crouch()
    {
        crouching.Act();
    }

    public void Sprint()
    {
        sprinting.Act();
    }
}

