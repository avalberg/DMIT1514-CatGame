using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Valberg_FinalProject
{
    public class Collider
    {
        public enum ColliderType
        {
            Left,
            Right,
            Top,
            Bottom
        }
        protected ColliderType colliderType;
        protected Vector2 position = Vector2.Zero;
        protected Vector2 dimensions;
        protected string textureName = "";

        internal Rectangle BoundingBox
        {
            get
            {
                return new Rectangle((int)position.X, (int)position.Y, (int)dimensions.X, (int)dimensions.Y);
            }
        }

        internal void Initialize(Vector2 position, Vector2 dimensions, ColliderType colliderType)
        {
            this.position = position;
            this.dimensions = dimensions;
            this.colliderType = colliderType;
        }
        internal void LoadContent(ContentManager Content)
        {
        }
        internal void Draw(SpriteBatch spriteBatch)
        {
        }

        internal void ProcessCollisions(Cat cat)
        {
            if (BoundingBox.Intersects(cat.BoundingBox))
            {
                switch (colliderType)
                {
                    case ColliderType.Top:
                        cat.Land(BoundingBox);
                        cat.StandOn(BoundingBox);
                        break;
                    case ColliderType.Right:
                        //if they're moving left
                        if (cat.Velocity.X < 0)
                        {
                            cat.MoveHorizontally(0);
                        }
                        break;
                    case ColliderType.Bottom:
                        //if they're moving upwards
                        if (cat.Velocity.Y < 0)
                        {
                            cat.MoveVertically(0);
                        }
                        break;
                    case ColliderType.Left:
                        if (cat.Velocity.X > 0)
                        {
                            cat.MoveHorizontally(0);
                        }
                        break;
                }
            }
        }
        internal void ProcessObstacleCollisions(Obstacle obstacle)
        {
            if (BoundingBox.Intersects(obstacle.BoundingBox) && this.colliderType == ColliderType.Top)
            {
                obstacle.Land(BoundingBox);
                obstacle.StandOn(BoundingBox);
            }
        }
    }
}
