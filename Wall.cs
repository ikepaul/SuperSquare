using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace SuperSquare
{
    internal class Wall
    {
        public Vector2 Rotation { get; set; }

        public float Distance { get; set; }
        public float Speed { get; set; }
        public Wall(Vector2 rotation, float distance, float speed) {
            Rotation = rotation;
            Distance = distance;
            Speed = speed;
        }
        public Vector2[] Points()
        {

            return new Vector2[] { Rotation * Distance - new Vector2(Rotation.Y, -Rotation.X) * Distance, Rotation * Distance + new Vector2(Rotation.Y, -Rotation.X) * Distance };
        }

    }
}
