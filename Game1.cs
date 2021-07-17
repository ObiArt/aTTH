using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;
using System.Diagnostics;

namespace aTTH
{
    public class Game1 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        private Texture2D player_sprite;

        private List<Entity> Entities;

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            Entities = new List<Entity>();

            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            player_sprite = Content.Load<Texture2D>("player");

            Entities.Add(new Wall(_graphics.GraphicsDevice, new Vector2(0, 0), new Vector2(100, 20)));
            Entities.Add(new Wall(_graphics.GraphicsDevice, new Vector2(0, 400), new Vector2(100, 20)));
            Entities.Add(new Wall(_graphics.GraphicsDevice, new Vector2(150, 380), new Vector2(20, 20)));
            Entities.Add(new Wall(_graphics.GraphicsDevice, new Vector2(170, 360), new Vector2(20, 20)));
            Entities.Add(new Wall(_graphics.GraphicsDevice, new Vector2(190, 340), new Vector2(20, 20)));
            Entities.Add(new Wall(_graphics.GraphicsDevice, new Vector2(190, 340), new Vector2(20, 100)));
            Entities.Add(new Player(new Vector2(300, 200), player_sprite));
            Entities.Add(new Player(new Vector2(100, 200), player_sprite));
            Entities.Add(new Player(new Vector2(25, 200), player_sprite));
            Entities.Add(new Player(new Vector2(200, 200), player_sprite));
        }

        protected override void Update(GameTime gameTime)
        {
            double deltaTime = gameTime.ElapsedGameTime.TotalSeconds;
            KeyboardState keyboardState = Keyboard.GetState();
            GamePadState gamePadState = GamePad.GetState(PlayerIndex.One);
            MouseState mouseState = Mouse.GetState(Window);

            if (gamePadState.Buttons.Back == ButtonState.Pressed || keyboardState.IsKeyDown(Keys.Escape))
                Exit();

            for (int i = 0; i < Entities.Count; i++)
            {
                Entities[i].Update(deltaTime);
                if (Entities[i].controlable)
                {
                    Entities[i].Control(gamePadState, keyboardState, mouseState);
                }
                if (Entities[i].collide_important)
                {
                    Entities[i].CollissionCheck(Entities);
                }
            }

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            _spriteBatch.Begin(SpriteSortMode.BackToFront, BlendState.NonPremultiplied, SamplerState.PointClamp);

            for (int i = 0; i < Entities.Count; i++)
            {
                Entities[i].Draw(_spriteBatch);
            }

            _spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
