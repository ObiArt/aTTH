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

        private Texture2D playerSprite;
        private Texture2D cursorSprite;

        private List<Entity> Entities;

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        protected override void Initialize()
        {
            Entities = new List<Entity>();

            IsMouseVisible = true;
            Window.AllowUserResizing = true;

            Params._calculateScale(_graphics.GraphicsDevice.PresentationParameters.BackBufferWidth);

            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            playerSprite = Content.Load<Texture2D>("player");
            cursorSprite = Content.Load<Texture2D>("cursor");

            Entities.Add(new Wall(_graphics.GraphicsDevice, new Vector2(200, 0), new Vector2(150, 5)));
            Entities.Add(new Wall(_graphics.GraphicsDevice, new Vector2(0, 100), new Vector2(200, 5)));
            Entities.Add(new Wall(_graphics.GraphicsDevice, new Vector2(40, 110), new Vector2(5, 5)));
            Entities.Add(new Wall(_graphics.GraphicsDevice, new Vector2(100, 90), new Vector2(5, 100)));
            Entities.Add(new Wall(_graphics.GraphicsDevice, new Vector2(50, 85), new Vector2(200, 25)));
            Entities.Add(new Player(new Vector2(50, 50), playerSprite, cursorSprite));
        }

        protected override void Update(GameTime gameTime)
        {
            Params._calculateScale(_graphics.GraphicsDevice.PresentationParameters.BackBufferWidth);
            double deltaTime = gameTime.ElapsedGameTime.TotalSeconds;
            KeyboardState keyboardState = Keyboard.GetState();
            GamePadState gamePadState = GamePad.GetState(PlayerIndex.One, GamePadDeadZone.None);
            MouseState mouseState = Mouse.GetState(Window);

            if (gamePadState.Buttons.Back == ButtonState.Pressed || keyboardState.IsKeyDown(Keys.Escape))
                Exit();

            for (int i = 0; i < Entities.Count; i++)
            {
                if (Entities[i].collideImportant)
                {
                    Entities[i].CollissionCheck(Entities);
                }
                if (Entities[i].controlable)
                {
                    Entities[i].Control(gamePadState, keyboardState, mouseState);
                }
                Entities[i].Update(deltaTime);
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
