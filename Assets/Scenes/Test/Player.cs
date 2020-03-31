using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : Character
{
    public Inventory Inventory = new Inventory();

    public void Controll(float mX, float mY, float rX, float rY, bool jump, bool sprint)
    {
        if (Body.Grounded)
        {
            Move(mX, mY, sprint);
            Rotate(rX, rY);
        }
        else
        {
            if ((mX > 0 && rY > 0) || (mX < 0 && rY < 0))
            {
                Rotate(rX, 0);
                Body.Strafe(Mathf.Clamp(rY, -1.0f, 1.0f) * cameraSettings.rotateY);
            }
            else
            {
                Rotate(rX, rY);
            }
        }

        if (jump)
        {
            Jump();
        }
    }

    void OnGUI()
    {
        GUI.Label(new Rect(0, 0, 1000, 50), State.ToString());
        GUI.Label(new Rect(0, 20, 1000, 50), "Velocity:" + Body.Velocity);
        GUI.Label(new Rect(0, 40, 1000, 50), "Speed:" + Body.Speed);
        GUI.Label(new Rect(0, 60, 1000, 50), "Ground Angle:" + Body.GroundNormalAngle);

        Inventory.DrawGUI(0, 80);
    }

    void OnDrawGizmos()
    {
        Gizmos.DrawLine(transform.position, transform.position + Vector3.down);
        Gizmos.DrawLine(transform.position, transform.position + Body.GroundNormal);
    }
}
