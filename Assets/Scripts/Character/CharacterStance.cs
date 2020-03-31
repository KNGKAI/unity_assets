using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterStance : CharacterMovement
{
    [System.Serializable]
    public struct StanceSettings
    {
        public enum CharacterStanceType
        {
            Hold,
            Toggle
        }
        public CharacterStanceType type;
        public float speed;
        public float crouchHeight;
        public float proneHeight;
        [Space(3)]
        public float crouchDeadzone;
    }

    public StanceSettings stanceSettings = new StanceSettings()
    {
        speed = 5.0f,
        crouchHeight = 1.2f,
        proneHeight = 0.6f,
        crouchDeadzone = 0.1f,
        type = StanceSettings.CharacterStanceType.Hold
    };

    public enum CharacterStanceState
    {
        Stand,
        Crouch,
        Prone
    }

    protected Ticker stanceTick;

    private CharacterStanceState stance;

    public CharacterStanceState Stance
    {
        get
        {
            return (stance);
        }
        set
        {
            switch (value)
            {
                case CharacterStanceState.Stand:
                    stanceTick.Act();
                    break;
                case CharacterStanceState.Crouch:
                    stanceTick.Act();
                    break;
                case CharacterStanceState.Prone:
                    stanceTick.Act();
                    break;
                default:
                    break;
            }
            switch (stanceSettings.type)
            {
                case StanceSettings.CharacterStanceType.Hold:
                    stance = value;
                    break;
                case StanceSettings.CharacterStanceType.Toggle:
                    if (stance == value)
                    {
                        stance = CharacterStanceState.Stand;
                    }
                    else
                    {
                        stance = value;
                    }
                    break;
                default:
                    break;
            }
        }
    }

    protected void StanceUpdate(float delta)
    {
        float speed;

        speed = stanceSettings.speed * delta;

        switch (Stance)
        {
            case CharacterStanceState.Stand:
                if (Body.Height < characterSettings.height)
                {
                    Body.Height += speed;

                    if (!Body.Grounded)
                    {
                        transform.Translate(0.0f, -speed, 0.0f);
                    }
                }
                else
                {
                    Body.Height = characterSettings.height;
                }
                break;
            case CharacterStanceState.Crouch:
                if (Body.Height > stanceSettings.crouchHeight)
                {
                    speed = -speed;
                }

                if (Mathf.Abs(Body.Height - stanceSettings.crouchHeight) > stanceSettings.crouchDeadzone)
                {
                    Body.Height += speed;

                    if (!Body.Grounded)
                    {
                        transform.Translate(0.0f, -speed, 0.0f);
                    }
                }
                else
                {
                    Body.Height = stanceSettings.crouchHeight;
                }
                break;
            case CharacterStanceState.Prone:
                if (Body.Height > stanceSettings.proneHeight)
                {
                    Body.Height -= speed;

                    if (!Body.Grounded)
                    {
                        transform.Translate(0.0f, -speed * Time.fixedDeltaTime, 0.0f);
                    }
                }
                else
                {
                    Body.Height = stanceSettings.crouchHeight;
                }
                break;
            default:
                break;
        }
    }
}
