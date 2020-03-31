using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterMovement : CharacterBase
{
    public static readonly float gravityForce = 9.8f;

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

        public enum JumpType
        {
            Hold,
            Immediate
        }

        public float speed;
        public float sprint;
        public float accel;
        public float drag;
        public float jump;
        public float rotate;

        [Space(3)]
        public float maxSlope;
        public float maxFallSlope;
        public MovementType moveType;
        public float moveDeadzone;
        public float inputDeadzone;
        [Space(3)]
        public JumpType jumpType;
        public float jumpAccel;
    }

    public MovementSettings movementSettings = new MovementSettings
    {
        speed = 5.0f,
        sprint = 10.0f,
        accel = 2.0f,
        drag = 4.0f,
        jump = 5.0f,
        rotate = 3.0f,

        maxSlope = 45.0f,
        maxFallSlope = 75.0f,
        moveType = MovementSettings.MovementType.Immediate,
        moveDeadzone = 0.1f,
        inputDeadzone = 0.01f,

        jumpType = MovementSettings.JumpType.Hold,
        jumpAccel = 8.0f
    };

    protected Vector2 move;

    protected Ticker moveTick;

    public enum CharacterMovementState
    {
        Idle,
        Move,
        Sprint,
        Slide,
        Jump
    }

    private CharacterMovementState state;

    public CharacterMovementState State
    {
        get
        {
            return (state);
        }
        set
        {
            switch (value)
            {
                case CharacterMovementState.Idle:
                    moveTick.Act();
                    break;
                case CharacterMovementState.Move:
                    moveTick.Act(2);
                    break;
                case CharacterMovementState.Sprint:
                    moveTick.Act();
                    break;
                case CharacterMovementState.Slide:
                    moveTick.Act();
                    break;
                case CharacterMovementState.Jump:
                    moveTick.Act(1);
                    break;
                default:
                    break;
            }
            state = value;
        }
    }

    protected void RotateCharacter(float a)
    {
        transform.Rotate(0.0f, a, 0.0f, Space.World);
    }

    protected void MovementUpdate(float delta)
    {
        Vector3 v;

        void MoveCharacter(float s)
        {
            switch (movementSettings.moveType)
            {
                case MovementSettings.MovementType.Accelerate:
                    v = Body.Velocity;
                    float a = movementSettings.accel * delta;

                    v.x += move.x * a;
                    v.z += move.y * a;

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

        void DragCharacter(float drag)
        {
            if (Body.Grounded)
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
                            Body.Drag(drag * delta);
                            break;
                        case MovementSettings.MovementType.Immediate:
                            Body.Drag(drag * delta);
                            break;
                        case MovementSettings.MovementType.Continuous:
                            break;
                        case MovementSettings.MovementType.Tank:
                            Body.Drag(drag * delta);
                            break;
                        default:
                            break;
                    }
                }
            }
        }

        void Gravity()
        {
            v = Body.Velocity;
            if (Body.Grounded)
            {
                v.y = Mathf.Lerp(v.y, 0, 0.1f);
            }
            else
            {
                v.y -= gravityForce * delta;
            }
            Body.Velocity = v;
        }

        switch (State)
        {
            case CharacterMovementState.Idle:
                if (Body.GroundNormalAngle > movementSettings.maxSlope)
                {
                    State = CharacterMovementState.Slide;
                    MovementUpdate(delta);
                    return;
                }
                DragCharacter(movementSettings.drag);
                move.x = Body.Velocity.x / movementSettings.speed;
                move.y = Body.Velocity.z / movementSettings.speed;
                Gravity();
                break;
            case CharacterMovementState.Move:
                MoveCharacter(movementSettings.speed);
                Gravity();
                break;
            case CharacterMovementState.Sprint:
                MoveCharacter(movementSettings.sprint);
                Gravity();
                break;
            case CharacterMovementState.Slide:
                v = Body.Velocity;

                v = Vector3.ProjectOnPlane(v, Body.GroundNormal);
                v.y -= gravityForce * delta;

                Body.Velocity = v;
                break;
            case CharacterMovementState.Jump:
                v = Body.Velocity;

                switch (movementSettings.jumpType)
                {
                    case MovementSettings.JumpType.Hold:
                        if (v.y > 0)
                        {
                            if (v.y < movementSettings.jump)
                            {
                                v.y += movementSettings.jumpAccel * Time.fixedDeltaTime;
                            }
                            else
                            {
                                State = CharacterMovementState.Idle;
                            }
                        }
                        else
                        {
                            v.y = movementSettings.jump / 2.0f;
                        }
                        break;
                    case MovementSettings.JumpType.Immediate:
                        if (Body.JumpGrounded)
                        {
                            v.y = movementSettings.jump;
                        }
                        break;
                    default:
                        break;
                }

                Body.Velocity = v;
                break;
            default:
                break;
        }
    }
}

