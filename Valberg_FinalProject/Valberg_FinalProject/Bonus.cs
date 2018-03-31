using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Valberg_FinalProject
{
    class Bonus
    {
        public enum State { Here, Gone }
        public State state;
        protected CelAnimationSequence spinning;
        protected CelAnimationPlayer celAnimationPlayer;
        protected Vector2 _position;
        protected Vector2 _dimensions = new Vector2(21, 21);
        protected double coinTimer = 0;

        internal Rectangle BoundingBox
        {
            get { return new Rectangle((int)_position.X, (int)_position.Y, (int)_dimensions.X, (int)_dimensions.Y); }
        }

        internal void Initialize(Vector2 position)
        {
            this._position = position;
            state = State.Here;
        }

        internal void LoadContent(ContentManager Content)
        {
            Texture2D spin = Content.Load<Texture2D>("coin");
            spinning = new CelAnimationSequence(spin, 21, 1 / 8f);
            celAnimationPlayer = new CelAnimationPlayer();
        }

        internal void Update(GameTime gameTime)
        {
            switch (state)
            {
                case (State.Here):
                    celAnimationPlayer.Play(spinning);
                    celAnimationPlayer.Update(gameTime);
                    coinTimer += gameTime.ElapsedGameTime.TotalSeconds;
                    if (coinTimer > 15)
                        state = State.Gone;
                    break;
                case (State.Gone):
                    break;
            }
        }

        internal void Draw(SpriteBatch spriteBatch)
        {
            switch (state)
            {
                case (State.Here):
                    celAnimationPlayer.Draw(spriteBatch, _position, SpriteEffects.None);
                    break;
                case (State.Gone):
                    break;

            }
        }

        internal void Collected()
        {
            if (state == State.Here)
            {
                state = State.Gone;
                GameManager.score += 5;
            }
        }
        internal void Reset(Vector2 position)
        {
            this._position = position;
            state = State.Here;
            if (coinTimer != 0)
                coinTimer = 0;
        }

    }
}
