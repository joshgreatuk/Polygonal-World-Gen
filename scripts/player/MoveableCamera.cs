using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InnoRPG.scripts.player
{
    [GlobalClass]
    public partial class MoveableCamera : Camera3D
    {
        [Export] public float cameraSpeed = 1f;
        [Export] public float cameraPanSpeed = 1f;
        Vector3 cameraDirection;
        Vector2 panDirection;

        public override void _UnhandledInput(InputEvent @event)
        {
            Vector2 xzAxis = Input.GetVector("camera_left", "camera_right", "camera_down", "camera_up");
            float yAxis = Input.GetAxis("camera_descend", "camera_ascend");
            cameraDirection = new Vector3(xzAxis.X, xzAxis.Y, yAxis);

            panDirection = Input.GetVector("camera_pan_left", "camera_pan_right", "camera_pan_down", "camera_pan_up");
        }

        public override void _PhysicsProcess(double delta)
        {
            Position += (cameraDirection * (float)delta) * cameraSpeed;
            GlobalRotate(Vector3.Right, panDirection.Y * (float)delta);
            GlobalRotate(Vector3.Up, panDirection.X * (float)delta);
        }
    }
}
