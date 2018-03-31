using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Audio;
using System;

namespace Valberg_FinalProject
{
    public class Obstacle
    {
        public enum State { Up, Down, Falling, Dying, Dead }
        public State state;
        protected string _textureName;
        double dyingTimer = 0;
        protected Vector2 _position;
        protected Vector2 _dimensions;
        protected Texture2D _texture;
        public Vector2 velocity;
        public float speed;
        protected CelAnimationSequence brokenAnimationSequence;
        protected CelAnimationPlayer celAnimationPlayer;

        internal Rectangle BoundingBox
        {
            get { return new Rectangle((int)_position.X, (int)_position.Y, (int)_dimensions.X, (int)_dimensions.Y); }
        }

        internal void Initialize(string texture, Vector2 position, Vector2 dimensions)
        {
            this._textureName = texture;
            this._position = position;
            this._dimensions = dimensions;
            state = State.Up;
        }

        internal void LoadContent(ContentManager Content)
        {
            _texture = Content.Load<Texture2D>(_textureName);
            Texture2D brokenTexture = Content.Load<Texture2D>("Poof");
            brokenAnimationSequence = new CelAnimationSequence(brokenTexture, 16, 1 / 16f);
            celAnimationPlayer = new CelAnimationPlayer();
        }

        internal void Update(GameTime gameTime)
        {
            int rightSide = 675 - (int)_dimensions.X;
            switch (state)
            {
                case State.Up:
                    if (_position.X > rightSide)
                        _position.X = rightSide;
                    if (_position.X < 0)
                        _position.X = 0;
                    _position += velocity * (float)gameTime.ElapsedGameTime.TotalSeconds;
                    velocity.Y += Game1.Gravity;
                    if (velocity.X > 0)
                        velocity.X -= speed * (float)gameTime.ElapsedGameTime.TotalSeconds;
                    if (velocity.X < 0)
                        velocity.X += speed * (float)gameTime.ElapsedGameTime.TotalSeconds;
                    if (Math.Abs(velocity.Y) > 16 * Game1.Gravity)
                        state = State.Falling;
                    break;
                case State.Falling:
                    if (_position.X > rightSide)
                        _position.X = rightSide;
                    if (_position.X < 0)
                        _position.X = 0;
                    _position += velocity * (float)gameTime.ElapsedGameTime.TotalSeconds;
                    velocity.Y += Game1.Gravity;
                    if (velocity.X > 0)
                        velocity.X -= speed * (float)gameTime.ElapsedGameTime.TotalSeconds;
                    if (velocity.X < 0)
                        velocity.X += speed * (float)gameTime.ElapsedGameTime.TotalSeconds;
                    break;
                case State.Down:
                    if (_position.X > rightSide)
                        _position.X = rightSide;
                    if (_position.X < 0)
                        _position.X = 0;
                    _position += velocity * (float)gameTime.ElapsedGameTime.TotalSeconds;
                    velocity.Y += Game1.Gravity;
                    if (velocity.X > 0)
                        velocity.X -= speed * (float)gameTime.ElapsedGameTime.TotalSeconds;
                    if (velocity.X < 0)
                        velocity.X += speed * (float)gameTime.ElapsedGameTime.TotalSeconds;
                    if (Math.Abs(velocity.Y) > 16 * Game1.Gravity)
                        state = State.Falling;
                    break;
                case State.Dying:
                    celAnimationPlayer.Play(brokenAnimationSequence);
                    dyingTimer += gameTime.ElapsedGameTime.TotalSeconds;
                    celAnimationPlayer.Update(gameTime);
                    if (dyingTimer > 3)
                        state = State.Dead;
                    break;
            }
        }

        internal void Reset(Vector2 position)
        {
            if (state == State.Dying || state == State.Dead)
                state = State.Down;
            this._position = position;
            this.velocity = new Vector2(0,0);
        }

        internal void Draw(SpriteBatch spriteBatch)
        {
            if (state != State.Dead && state != State.Dying)
                spriteBatch.Draw(_texture, _position, Color.White);
            else if (state == State.Dying)
                celAnimationPlayer.Draw(spriteBatch, _position, SpriteEffects.None);
        }

        internal void Fall()
        {
            if (state != State.Falling)
            {
                state = State.Falling;
                velocity.Y = -300;
            }
        }
        internal void MoveHorizontally(float direction)
        {
            velocity.X = direction * speed;
        }

        internal void Land(Rectangle whatILandedOn)
        {
            if (state == State.Falling)
            {
                velocity.Y = 0;
                state = State.Down;
            }
        }
        internal void StandOn(Rectangle whatImStandingOn)
        {
            _position.Y = whatImStandingOn.Top - _dimensions.Y;
            velocity.Y -= Game1.Gravity;
        }

        internal void ProcessOtherObstacleCollisions(Obstacle obstacle)
        {
            if (BoundingBox.Intersects(obstacle.BoundingBox))
            {
                int direction = 0;
                if (velocity.X > 0)
                    direction = 1;
                else if (velocity.X < 0)
                    direction = -1;
                obstacle.speed = speed;
                obstacle.MoveHorizontally(direction);
            }
        }

    }
}
