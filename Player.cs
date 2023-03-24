using System;
using Microsoft.Xna.Framework;

namespace SuperSquare
{
    internal class Player
    {
        public float Radius { get; set; }
        public Vector2 Rotation { get; set; }

        public float Distance { get; set; }

        public float RPM  {get; set; }
        public bool IsDead { get; set; }

        public Player(float radius, Vector2 rotation, float distance, float rpm)
        {
            Radius = radius;
            Rotation = rotation;
            Distance = distance;
            RPM = rpm;
            IsDead = false;
        }

        public void Rotate(float radians)
        {
            var ca = (float) Math.Cos(radians);
            var sa = (float) Math.Sin(radians);
            Rotation = new Vector2(ca * Rotation.X - sa * Rotation.Y, sa * Rotation.X + ca * Rotation.Y);
        }

    }
}
