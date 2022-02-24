using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Audio;
using System;
using System.Threading;
using System.Collections.Generic;

namespace Space_Invators
{
    public class Game1 : Game
    {
        Random Random = new Random();

        GameState _state; 
        MouseState mouse;
        Texture2D P1Pic, BulletPic, EnemyPic;

        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        public static int Width = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width;
        public static int Height = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height;

        bool Hit = false;


        // ints
        int MovmentSpeed = 8;
        int test = 0;

        List<Rectangle> rects = new List<Rectangle>();
        

        //Rectangle
        Rectangle RectP1 = new Rectangle(Width/2, Height /2 + 500, 80, 80);
        Rectangle EnemyRect1 = new Rectangle(0, Height/5, 80, 80);
        Rectangle BulletRect;



        // Vector
        Vector2 BulletSpeed;
        Vector2 BulletPosition;

        enum GameState
        {
            MainMenu,
            GamePlay,
        }

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            _graphics.PreferredBackBufferWidth = Width;
            _graphics.PreferredBackBufferHeight = Height;
            _graphics.ApplyChanges();

            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            // TODO: Add your initialization logic here
            
            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            P1Pic = Content.Load<Texture2D>("parrot");
            BulletPic = Content.Load<Texture2D>("ball");
            EnemyPic = Content.Load<Texture2D>("LargeAlien");
            // TODO: use this.Content to load your game content here
        }

        protected override void Update(GameTime gameTime)
        {
            
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();


             _state = GameState.GamePlay;

            switch (_state)
            {
                case GameState.MainMenu:
                    break;
                case GameState.GamePlay:
                    GamePlay(gameTime);
                    break;
            }

            // TODO: Add your update logic here

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {

            base.Draw(gameTime);
        }

        void GamePlay(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Blue);
            KeyboardState KeyboardState = Keyboard.GetState();
            mouse = Mouse.GetState();

            _spriteBatch.Begin();

            // Player Draw
            _spriteBatch.Draw(P1Pic, RectP1, Color.White);
            _spriteBatch.Draw(BulletPic, BulletPosition, Color.White);
            _spriteBatch.Draw(EnemyPic, EnemyRect1, Color.White);
            _spriteBatch.End();

            BulletPosition += BulletSpeed;

            MovementPlayer();


            base.Draw(gameTime);
        }

        void MovementPlayer()
        {
            KeyboardState KeyboardState = Keyboard.GetState();

            if (KeyboardState.IsKeyDown(Keys.A))
            {
                RectP1.X -= MovmentSpeed;
            }
            if (KeyboardState.IsKeyDown(Keys.D))
            {
                RectP1.X += MovmentSpeed;
            }


            if (RectP1.X > Width - 85)
            {
                RectP1.X -= MovmentSpeed;
            }
            if (RectP1.X < 0)
            {
                RectP1.X += MovmentSpeed;
            }


            if (mouse.LeftButton == ButtonState.Pressed && Hit == false)
            {
                BulletPosition.X = RectP1.X;
                BulletPosition.Y = RectP1.Y;

                BulletSpeed.Y = Random.Next(-10, -5);
                Hit = true;
            }
           
            if (BulletPosition.Y < -Height/2)
            { 
                Hit = false;
            }

        }

        void Collicon()
        {
            BulletRect.Y = (int)BulletPosition.Y;
            BulletRect.X = (int)BulletPosition.X;

            if (BulletRect.Intersects(EnemyRect1) == true);
            {
                test++;
            }
        }
    }   
}
