using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Valberg_FinalProject
{
    public class Cat
    {
        protected enum State
        {
            Idle, Walking, Jumping
        }
        protected State state;
        protected Texture2D cat, catright;
        protected CelAnimationSequence walkSequence;
        protected CelAnimationPlayer animationPlayer;
        protected Vector2 position = Vector2.Zero;
        protected SpriteEffects se;
        protected Vector2 dimensions;
        protected Vector2 velocity;
        internal Vector2 Velocity
        {
            get { return velocity; }
            set { velocity = value; }
        }
        protected Rectangle gameBoundingBox;
        protected const int Speed = 100;
        internal Rectangle BoundingBox
        {
            get { return new Rectangle((int)position.X, (int)position.Y, (int)dimensions.X, (int)dimensions.Y); }
        }

        internal void Initialize(Vector2 position, Rectangle gameBoundingBox)
        {
            this.position = position;
            this.gameBoundingBox = gameBoundingBox;
            dimensions = new Vector2(32, 28);
            state = State.Idle;
            animationPlayer = new CelAnimationPlayer();
        }

        internal void LoadContent(ContentManager Content)
        {
            cat = Content.Load<Texture2D>("cat");
            catright = Content.Load<Texture2D>("catright");
            walkSequence = new CelAnimationSequence(Content.Load<Texture2D>("catwalk"), 43, 1 / 8f);
        }
        
        internal void Update(GameTime gameTime)
        {
            int rightSide = 675 - (int)dimensions.X;
            KeyboardState kb = Keyboard.GetState();

            if (position.X > rightSide)
                position.X = rightSide;
            if (position.X < 0)
                position.X = 0;
            if (position.Y < 0)
                position.Y = 0;

            position += velocity * (float)gameTime.ElapsedGameTime.TotalSeconds;
            velocity.Y += Game1.Gravity;
            if (Math.Abs(velocity.Y) > 16 * Game1.Gravity)
                state = State.Jumping;

            switch(state)
            {
                case (State.Idle):
                    break;
                case (State.Walking):
                    if (kb.IsKeyDown(Keys.Left))
                    {
                        animationPlayer.Play(walkSequence);
                        animationPlayer.Update(gameTime);
                        se = SpriteEffects.None;
                    }
                    if (kb.IsKeyDown(Keys.Right))
                    {
                        animationPlayer.Play(walkSequence);
                        animationPlayer.Update(gameTime);
                        se = SpriteEffects.FlipHorizontally;
                    }
                    break;
                case (State.Jumping):
                    if (kb.IsKeyDown(Keys.Left))
                    {
                        animationPlayer.Play(walkSequence);
                        animationPlayer.Update(gameTime);
                        se = SpriteEffects.None;
                    }
                    if (kb.IsKeyDown(Keys.Right))
                    {
                        animationPlayer.Play(walkSequence);
                        animationPlayer.Update(gameTime);
                        se = SpriteEffects.FlipHorizontally;
                    }
                    break;
            }
        }

        internal void Reset(Vector2 position)
        {
            state = State.Idle;
            this.position = position;
            Velocity = new Vector2(0, 0);
        }

        internal void Draw(SpriteBatch spriteBatch)
        {
            switch (state)
            {
                case (State.Idle):
                    if (se == SpriteEffects.FlipHorizontally)
                        spriteBatch.Draw(catright, position, Color.White);
                    else
                        spriteBatch.Draw(cat, position, Color.White);
                    break;
                case (State.Walking):
                    animationPlayer.Draw(spriteBatch, position, se);
                    break;
                case (State.Jumping):
                    animationPlayer.Draw(spriteBatch, position, se);
                    break;
            }
        }

        internal void Jump()
        {
            if (state != State.Jumping)
            {
                state = State.Jumping;
                velocity.Y = -300;
            }
        }

        internal void MoveHorizontally(float direction)
        {
            velocity.X = direction * Speed;
        }

        internal void MoveVertically(float direction)
        {
            velocity.Y = direction * Speed;
        }

        internal void Move(Vector2 direction)
        {
            velocity.X = direction.X * Speed;
            if (state == State.Idle)
            {
                state = State.Walking;
                animationPlayer.Play(walkSequence);
            }
        }
        internal void Stop()
        {
            if (state == State.Walking)
            {
                velocity.X = 0;
                state = State.Idle;
            }
        }
        internal void Land(Rectangle whatILandedOn)
        {
            if (state == State.Jumping)
            {
                velocity.Y = 0;
                state = State.Walking;
            }
        }
        internal void StandOn(Rectangle whatImStandingOn)
        {
            position.Y = whatImStandingOn.Top - dimensions.Y;
            velocity.Y -= Game1.Gravity;
        }
        internal void ProcessCollisions(Obstacle obstacle)
        {
            if (BoundingBox.Intersects(obstacle.BoundingBox))
            {
                int direction = 0;
                obstacle.speed = Speed;
                if (se == SpriteEffects.None)
                    direction = -1;
                else if (se == SpriteEffects.FlipHorizontally)
                    direction = 1;
                obstacle.MoveHorizontally(direction);
            }
        }
        internal void ProcessBonus(Bonus bonus)
        {
            if (BoundingBox.Intersects(bonus.BoundingBox))
                bonus.Collected();
        }
    }
}
