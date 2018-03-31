using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using Microsoft.Xna.Framework.Audio;
using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.Xna.Framework.Media;

namespace Valberg_FinalProject
{
    public class Game1 : Game
    {
        protected enum GameState { MainMenu, Ready, Level1, Level2Ready, Level2, Level3Ready, Level3, GameOver }
        protected GameState currentState = GameState.MainMenu;
        protected const int WINDOWWIDTH = 675;
        protected const int WINDOWHEIGHT = 469;
        protected SoundEffect crash;
        protected Song mainTheme;
        internal static int Gravity = 5;
        private SpriteFont font;
        double timer = 0;

        protected Texture2D background, menubackground, readyscreen, level2readyscreen, level2background, level3readyscreen, level3background, gameoverbackground;

        Rectangle gameBoundingBox = new Rectangle(0, 0, WINDOWWIDTH, WINDOWHEIGHT);

        Cat cat;
        protected List<Obstacle> obstacles = new List<Obstacle>();
        Obstacle book, mug, books, plant, laptop, clock;

        // level 1
        Collider floor, table, couch, bookshelf, rightShelf, middleShelf;

        // level 2
        Collider highleftshelf, desk, sidetable, timershelf, computer;

        // level 3
        Collider leftcabinet, midcabinet, rightcabinet, kitchenshelf, fridgetop, sink;

        Bonus coin;

        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        protected override void Initialize()
        {
            graphics.PreferredBackBufferWidth = WINDOWWIDTH;
            graphics.PreferredBackBufferHeight = WINDOWHEIGHT;
            graphics.ApplyChanges();

            base.Initialize();
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);
            mainTheme = Content.Load<Song>("maintheme");
            crash = Content.Load<SoundEffect>("crash");
            menubackground = Content.Load<Texture2D>("menubackground");
            readyscreen = Content.Load<Texture2D>("readyscreen");
            background = Content.Load<Texture2D>("level1background");
            level2background = Content.Load<Texture2D>("level2background");
            level2readyscreen = Content.Load<Texture2D>("level2readyscreen");
            level3readyscreen = Content.Load<Texture2D>("level3readyscreen");
            level3background = Content.Load<Texture2D>("level3background");
            gameoverbackground = Content.Load<Texture2D>("gameoverbackground");

            //bonus
            coin = new Bonus();
            coin.Initialize(new Vector2(520, 12));
            coin.LoadContent(Content);

            // player character
            cat = new Cat();
            cat.Initialize(new Vector2(50, 450), gameBoundingBox);

            // obstacles to push
            book = new Obstacle();
            mug = new Obstacle();
            books = new Obstacle();
            plant = new Obstacle();
            laptop = new Obstacle();
            clock = new Obstacle();
            book.Initialize("book", new Vector2(560, 341), new Vector2(40, 30));
            mug.Initialize("mug", new Vector2(350, 75), new Vector2(18, 17));
            books.Initialize("books2", new Vector2(100, 170), new Vector2(49, 28));
            plant.Initialize("plant", new Vector2(575, 141), new Vector2(23, 32));
            laptop.Initialize("laptop", new Vector2(300, 281), new Vector2(60, 41));
            clock.Initialize("clock", new Vector2(275, 75), new Vector2(42, 38));
            obstacles.Add(book);
            obstacles.Add(mug);
            obstacles.Add(books);
            obstacles.Add(plant);
            obstacles.Add(laptop);
            obstacles.Add(clock);

            // colliders
            floor = new Collider();
            table = new Collider();
            couch = new Collider();
            bookshelf = new Collider();
            rightShelf = new Collider();
            middleShelf = new Collider();
            floor.Initialize(new Vector2(0, 450), new Vector2(675, 1), Collider.ColliderType.Top);
            table.Initialize(new Vector2(541, 342), new Vector2(100, 1), Collider.ColliderType.Top);
            couch.Initialize(new Vector2(283, 282), new Vector2(202, 1), Collider.ColliderType.Top);
            bookshelf.Initialize(new Vector2(42, 171), new Vector2(147, 1), Collider.ColliderType.Top);
            rightShelf.Initialize(new Vector2(513, 142), new Vector2(109, 1), Collider.ColliderType.Top);
            middleShelf.Initialize(new Vector2(217, 76), new Vector2(184, 1), Collider.ColliderType.Top);

            //level 2 colliders
            highleftshelf = new Collider();
            desk = new Collider();
            sidetable = new Collider();
            timershelf = new Collider();
            computer = new Collider();

            highleftshelf.Initialize(new Vector2(30, 76), new Vector2(184, 1), Collider.ColliderType.Top);
            desk.Initialize(new Vector2(84, 258), new Vector2(425, 1), Collider.ColliderType.Top);
            sidetable.Initialize(new Vector2(529, 334), new Vector2(131, 1), Collider.ColliderType.Top);
            timershelf.Initialize(new Vector2(493, 30), new Vector2(108, 1), Collider.ColliderType.Top);
            computer.Initialize(new Vector2(246, 167), new Vector2(130, 1), Collider.ColliderType.Top);

            //level 3 colliders
            leftcabinet = new Collider();
            midcabinet = new Collider();
            rightcabinet = new Collider();
            kitchenshelf = new Collider();
            fridgetop = new Collider();
            sink = new Collider();

            leftcabinet.Initialize(new Vector2(0, 243), new Vector2(140, 1), Collider.ColliderType.Top);
            midcabinet.Initialize(new Vector2(323, 243), new Vector2(138, 1), Collider.ColliderType.Top);
            rightcabinet.Initialize(new Vector2(622, 243), new Vector2(53, 1), Collider.ColliderType.Top);
            kitchenshelf.Initialize(new Vector2(39, 116), new Vector2(276, 1), Collider.ColliderType.Top);
            fridgetop.Initialize(new Vector2(467, 153), new Vector2(154, 1), Collider.ColliderType.Top);
            sink.Initialize(new Vector2(141, 288), new Vector2(180, 1), Collider.ColliderType.Top);

            cat.LoadContent(Content);
            foreach (Obstacle i in obstacles)
                i.LoadContent(Content);
            font = Content.Load<SpriteFont>("SystemArialFont");
            MediaPlayer.Play(mainTheme);
            MediaPlayer.IsRepeating = true;
        }

        protected override void UnloadContent()
        {

        }

        protected override void Update(GameTime gameTime)
        {
            KeyboardState kb = Keyboard.GetState();

            switch (currentState)
            {
                case (GameState.MainMenu):
                    GameManager.score = 0;
                    GameManager.currenthighscore = 0;
                    this.IsMouseVisible = true;
                    MouseState mouseState = Mouse.GetState();
                    Rectangle playButton = new Rectangle(236, 156, 225, 53);
                    Rectangle exitButton = new Rectangle(236, 245, 225, 53);
                    if (mouseState.LeftButton == ButtonState.Pressed)
                    {
                        Point mousePos = new Point(mouseState.X, mouseState.Y);
                        if (playButton.Contains(mousePos))
                            currentState = GameState.Ready;
                    }
                    if (mouseState.LeftButton == ButtonState.Pressed)
                    {
                        Point mousePos = new Point(mouseState.X, mouseState.Y);
                        if (exitButton.Contains(mousePos))
                            Exit();
                    }
                    break;
                case (GameState.Ready):
                    book.Reset(new Vector2(560, 341));
                    mug.Reset(new Vector2(350, 75));
                    books.Reset(new Vector2(100, 170));
                    plant.Reset(new Vector2(575, 141));
                    laptop.Reset(new Vector2(300, 281));
                    clock.Reset(new Vector2(275, 75));

                    timer = 0;
                    if (GameManager.timer != 30)
                        GameManager.timer = 30;
                    timer += gameTime.TotalGameTime.TotalSeconds;
                    this.IsMouseVisible = true;
                    MouseState mS = Mouse.GetState();
                    if (mS.LeftButton == ButtonState.Pressed && timer > 3)
                        currentState = GameState.Level1;
                    break;
                case (GameState.Level1):
                    GameManager.timer -= gameTime.ElapsedGameTime.TotalSeconds;
                    if (GameManager.timer < 0.10)
                        currentState = GameState.Level2Ready;
                    if (kb.IsKeyDown(Keys.Left))
                        cat.Move(new Vector2(-1, 0));
                    else if (kb.IsKeyDown(Keys.Right))
                        cat.Move(new Vector2(1, 0));
                    else
                        cat.Stop();
                    if (kb.IsKeyDown(Keys.Space))
                        cat.Jump();

                    floor.ProcessCollisions(cat);
                    table.ProcessCollisions(cat);
                    couch.ProcessCollisions(cat);
                    bookshelf.ProcessCollisions(cat);
                    rightShelf.ProcessCollisions(cat);
                    middleShelf.ProcessCollisions(cat);

                    cat.Update(gameTime);

                    for (int i = 0; i < obstacles.Count; i++)
                    {
                        if (floor.BoundingBox.Intersects(obstacles[i].BoundingBox) && obstacles[i].state != Obstacle.State.Dying && obstacles[i].state != Obstacle.State.Dead)
                        {
                            obstacles[i].state = Obstacle.State.Dying;
                            GameManager.score++;
                            crash.Play();
                        }
                    }
                    foreach (Obstacle i in obstacles)
                    {
                        cat.ProcessCollisions(i);
                        table.ProcessObstacleCollisions(i);
                        couch.ProcessObstacleCollisions(i);
                        bookshelf.ProcessObstacleCollisions(i);
                        rightShelf.ProcessObstacleCollisions(i);
                        middleShelf.ProcessObstacleCollisions(i);
                        foreach (Obstacle o in obstacles)
                        {
                            if (obstacles.IndexOf(i) != obstacles.IndexOf(o))
                                i.ProcessOtherObstacleCollisions(o);
                        }
                        i.Update(gameTime);
                    }
                    break;
                case (GameState.Level2Ready):
                    //reset obstacles & cat to their new places
                    cat.Reset(new Vector2(50, 450));
                    book.Reset(new Vector2(70, 75));
                    mug.Reset(new Vector2(400, 255));
                    books.Reset(new Vector2(540, 333));
                    plant.Reset(new Vector2(100, 255));
                    laptop.Reset(new Vector2(130, 75));
                    clock.Reset(new Vector2(150, 255));

                    //reset the timer
                    if (GameManager.timer != 25)
                        GameManager.timer = 25;
                    timer = 0;
                    timer += gameTime.TotalGameTime.TotalSeconds;

                    //check for mouse click
                    this.IsMouseVisible = true;
                    mS = Mouse.GetState();
                    if (mS.LeftButton == ButtonState.Pressed && timer > 3)
                        currentState = GameState.Level2;
                    break;
                case (GameState.Level2):
                    GameManager.timer -= gameTime.ElapsedGameTime.TotalSeconds;
                    if (GameManager.timer < 0.10)
                        currentState = GameState.Level3Ready;
                    if (kb.IsKeyDown(Keys.Left))
                        cat.Move(new Vector2(-1, 0));
                    else if (kb.IsKeyDown(Keys.Right))
                        cat.Move(new Vector2(1, 0));
                    else
                        cat.Stop();
                    if (kb.IsKeyDown(Keys.Space))
                        cat.Jump();

                    floor.ProcessCollisions(cat);
                    highleftshelf.ProcessCollisions(cat);
                    desk.ProcessCollisions(cat);
                    sidetable.ProcessCollisions(cat);
                    timershelf.ProcessCollisions(cat);
                    computer.ProcessCollisions(cat);
                    cat.ProcessBonus(coin);

                    cat.Update(gameTime);
                    coin.Update(gameTime);

                    for (int i = 0; i < obstacles.Count; i++)
                    {
                        if (floor.BoundingBox.Intersects(obstacles[i].BoundingBox) && obstacles[i].state != Obstacle.State.Dying && obstacles[i].state != Obstacle.State.Dead)
                        {
                            obstacles[i].state = Obstacle.State.Dying;
                            GameManager.score++;
                            crash.Play();
                        }
                    }
                    foreach (Obstacle i in obstacles)
                    {
                        cat.ProcessCollisions(i);
                        highleftshelf.ProcessObstacleCollisions(i);
                        desk.ProcessObstacleCollisions(i);
                        sidetable.ProcessObstacleCollisions(i);
                        timershelf.ProcessObstacleCollisions(i);

                        foreach (Obstacle o in obstacles)
                        {
                            if (obstacles.IndexOf(i) != obstacles.IndexOf(o))
                                i.ProcessOtherObstacleCollisions(o);
                        }
                        i.Update(gameTime);
                    }
                    break;
                case (GameState.Level3Ready):
                    coin.Reset(new Vector2(630, 220));
                    cat.Reset(new Vector2(50, 450));
                    book.Reset(new Vector2(500, 152));
                    mug.Reset(new Vector2(330, 242));
                    books.Reset(new Vector2(50, 116));
                    plant.Reset(new Vector2(80, 116));
                    laptop.Reset(new Vector2(120, 116));
                    clock.Reset(new Vector2(180, 116));

                    if (GameManager.timer != 20)
                        GameManager.timer = 20;

                    timer = 0;
                    timer += gameTime.TotalGameTime.TotalSeconds;
                    this.IsMouseVisible = true;
                    mS = Mouse.GetState();
                    if (mS.LeftButton == ButtonState.Pressed && timer > 3)
                        currentState = GameState.Level3;
                    break;
                case (GameState.Level3):
                    GameManager.timer -= gameTime.ElapsedGameTime.TotalSeconds;
                    if (GameManager.timer < 0.10)
                        currentState = GameState.GameOver;
                    if (kb.IsKeyDown(Keys.Left))
                        cat.Move(new Vector2(-1, 0));
                    else if (kb.IsKeyDown(Keys.Right))
                        cat.Move(new Vector2(1, 0));
                    else
                        cat.Stop();
                    if (kb.IsKeyDown(Keys.Space))
                        cat.Jump();

                    floor.ProcessCollisions(cat);
                    leftcabinet.ProcessCollisions(cat);
                    midcabinet.ProcessCollisions(cat);
                    rightcabinet.ProcessCollisions(cat);
                    kitchenshelf.ProcessCollisions(cat);
                    fridgetop.ProcessCollisions(cat);
                    sink.ProcessCollisions(cat);

                    coin.Update(gameTime);
                    cat.ProcessBonus(coin);
                    cat.Update(gameTime);

                    for (int i = 0; i < obstacles.Count; i++)
                    {
                        if (floor.BoundingBox.Intersects(obstacles[i].BoundingBox) && obstacles[i].state != Obstacle.State.Dying && obstacles[i].state != Obstacle.State.Dead)
                        {
                            obstacles[i].state = Obstacle.State.Dying;
                            GameManager.score++;
                            crash.Play();
                        }
                    }
                    foreach (Obstacle i in obstacles)
                    {
                        cat.ProcessCollisions(i);
                        leftcabinet.ProcessObstacleCollisions(i);
                        midcabinet.ProcessObstacleCollisions(i);
                        rightcabinet.ProcessObstacleCollisions(i);
                        kitchenshelf.ProcessObstacleCollisions(i);
                        fridgetop.ProcessObstacleCollisions(i);

                        foreach (Obstacle o in obstacles)
                        {
                            if (obstacles.IndexOf(i) != obstacles.IndexOf(o))
                                i.ProcessOtherObstacleCollisions(o);
                        }
                        i.Update(gameTime);
                    }
                    break;
                case (GameState.GameOver):
                    GameManager.GameOver();
                    this.IsMouseVisible = true;
                    mouseState = Mouse.GetState();
                    Rectangle GOplayButton = new Rectangle(27, 121, 226, 55);
                    Rectangle GOexitButton = new Rectangle(27, 201, 226, 55);
                    if (mouseState.LeftButton == ButtonState.Pressed)
                    {
                        Point mousePos = new Point(mouseState.X, mouseState.Y);
                        if (GOplayButton.Contains(mousePos))
                            currentState = GameState.MainMenu;
                    }
                    if (mouseState.LeftButton == ButtonState.Pressed)
                    {
                        Point mousePos = new Point(mouseState.X, mouseState.Y);
                        if (GOexitButton.Contains(mousePos))
                            Exit();
                    }
                    break;

            }
            base.Update(gameTime);

        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);
            spriteBatch.Begin();

            switch (currentState)
            {
                case (GameState.MainMenu):
                    spriteBatch.Draw(menubackground, Vector2.Zero);
                    break;
                case (GameState.Ready):
                    spriteBatch.Draw(readyscreen, Vector2.Zero);
                    break;
                case (GameState.Level1):
                    spriteBatch.Draw(background, Vector2.Zero);
                    cat.Draw(spriteBatch);
                    foreach (Obstacle i in obstacles)
                        i.Draw(spriteBatch);
                    spriteBatch.DrawString(font, GameManager.score.ToString(), new Vector2(108, 15), Color.Navy);
                    spriteBatch.DrawString(font, Math.Round(GameManager.timer, 2).ToString(), new Vector2(570, 12), Color.Navy);
                    break;
                case (GameState.Level2Ready):
                    spriteBatch.Draw(level2readyscreen, Vector2.Zero);
                    break;
                case (GameState.Level2):
                    spriteBatch.Draw(level2background, Vector2.Zero);
                    cat.Draw(spriteBatch);
                    coin.Draw(spriteBatch);
                    foreach (Obstacle i in obstacles)
                        i.Draw(spriteBatch);
                    spriteBatch.DrawString(font, GameManager.score.ToString(), new Vector2(108, 15), Color.Navy);
                    spriteBatch.DrawString(font, Math.Round(GameManager.timer, 2).ToString(), new Vector2(570, 12), Color.Navy);
                    break;
                case (GameState.Level3Ready):
                    spriteBatch.Draw(level3readyscreen, Vector2.Zero);
                    break;
                case (GameState.Level3):
                    spriteBatch.Draw(level3background, Vector2.Zero);
                    coin.Draw(spriteBatch);
                    cat.Draw(spriteBatch);
                    foreach (Obstacle i in obstacles)
                        i.Draw(spriteBatch);
                    spriteBatch.DrawString(font, GameManager.score.ToString(), new Vector2(108, 15), Color.Navy);
                    spriteBatch.DrawString(font, Math.Round(GameManager.timer, 2).ToString(), new Vector2(570, 12), Color.Navy);
                    break;
                case (GameState.GameOver):
                    spriteBatch.Draw(gameoverbackground, Vector2.Zero);
                    string highscores = GameManager.ListToText(GameManager.highscores);
                    string currentscore = "Your Score: " + GameManager.currenthighscore;
                    spriteBatch.DrawString(font, highscores, new Vector2(380, 160), Color.Black);
                    spriteBatch.DrawString(font, currentscore, new Vector2(377, 121), Color.Black);
                    break;
            }
            spriteBatch.End();
            base.Draw(gameTime);
        }
    }
}
