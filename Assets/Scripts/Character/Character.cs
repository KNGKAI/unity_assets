using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : CharacterCamera
{
    protected Ticker busying;

    public bool Busy
    {
        get
        {
            return (busying.Active);
        }
    }

    private void Awake()
    {
        gameObject.layer = LayerMask.NameToLayer("Character");

        Body.Radius = characterSettings.radius;
        Body.Height = characterSettings.height;
    }

    private void FixedUpdate()
    {
        if (Busy)
        {
            return;
        }

        MovementUpdate(Time.fixedDeltaTime);
        StanceUpdate(Time.fixedDeltaTime);
    }

    private void Update()
    {
        if (!Alive && Body.Grounded)
        {
            Body.Freeze();
            return;
        }

        if (moveTick.Active)
        {
            moveTick.Procces();
        }
        else
        {
            State = CharacterMovementState.Idle;
        }

        if (stanceSettings.type == StanceSettings.CharacterStanceType.Hold)
        {
            if (stanceTick.Active)
            {
                stanceTick.Procces();
            }
            else
            {
                Stance = CharacterStanceState.Stand;
            }
        }
    }

    public void Rotate(float x, float y)
    {
        RotateCamera(Mathf.Clamp(x, -1.0f, 1.0f) * cameraSettings.rotateX, Mathf.Clamp(y, -1.0f, 1.0f) * cameraSettings.rotateY);
        if (Shooter)
        {
            transform.Rotate(0.0f, y, 0.0f, Space.World);
        }
    }

    public void Move(float x, float y, bool sprint)
    {
        if (Body.Grounded && !(Mathf.Abs(x) < movementSettings.inputDeadzone && Mathf.Abs(y) < movementSettings.inputDeadzone))
        {
            Transform t = Shooter || movementSettings.moveType == MovementSettings.MovementType.Tank ? transform : Camera.transform;
            Vector3 m = t.TransformDirection(x, 0, y);

            move.x = m.x;
            move.y = m.z;

            if (move.magnitude > 1)
            {
                move.Normalize();
            }

            if (movementSettings.moveType == MovementSettings.MovementType.Tank)
            {
                transform.Rotate(0.0f, move.x * movementSettings.rotate, 0.0f);
                move.x = 0;
            }

            if (!Shooter)
            {
                float a = Vector3.Angle(transform.forward, m);
                if (Vector3.Angle(transform.right, m) > 90.0f)
                {
                    a *= -1;
                }
                transform.Rotate(0.0f, a, 0.0f);
            }

            State = sprint ? CharacterMovementState.Sprint : CharacterMovementState.Move;
        }
    }

    public void Jump()
    {
        if (Stance != CharacterStanceState.Prone)
        {
            if (Body.JumpGrounded || State == CharacterMovementState.Jump)
            {
                State = CharacterMovementState.Jump;
            }
        }
    }

    public void Slide()
    {
        if (Body.Grounded)
        {
            State = CharacterMovementState.Slide;
        }
    }

    public void Stand()
    {
        Stance = CharacterStanceState.Stand;
    }

    public void Crouch()
    {
        Stance = CharacterStanceState.Crouch;
    }

    public void Prone()
    {
        if (Body.Grounded)
        {
            Stance = CharacterStanceState.Prone;
        }
    }
}