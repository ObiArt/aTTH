using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace aTTH
{
    public class Player : Entity
    {
        private float maxWalkSpeed = 1f;
        private float acceleration = 4f;
        private float decceleration = 7f;
        private float maxFallSpeed = 2.5f;
        private float gravity = 3f;
        private float jumpSpeed = 1.5f;
        private float flyingSpeed = 2.0f;
        private bool flying = false;
        private short flightCount = 0;
        private int flyingTimer = 0;
        /// <summary>
        /// Used to prevent players from instantly using up all their flights
        /// </summary>
        private bool antiSpamFly = false;
        /// <summary>
        /// Used to prevent players from jumping non-stop
        /// </summary>
        private bool antiSpamJump = false;
        private Dictionary<string, dynamic> inputs = new Dictionary<string, dynamic>(); //dynamic is pog af

        /// <summary>
        /// Maximum distance cursor can be away from the player when using gamepad
        /// </summary>
        private float cursorDistance = 30f;
        public Texture2D cursorSprite;
        public Vector2 cursorOrigin;

        public Vector2 flyingSize;

        public Player(Vector2 pos, Texture2D texture, Texture2D cursorTexture)
        {
            name = "player";
            position = pos;
            sprite = texture;
            cursorSprite = cursorTexture;
            origin = new Vector2(texture.Width / 2, texture.Height / 2);
            cursorOrigin = new Vector2(cursorTexture.Width / 2, cursorTexture.Height / 2);

            collide = false;
            collideImportant = true;
            controlable = true;

            inputs.Add("m_left", false);
            inputs.Add("m_right", false);
            inputs.Add("m_jump", false);
            inputs.Add("c_x", 0);
            inputs.Add("c_y", 0);
            inputs.Add("c_pressed", false);
        }

        public override void Update(double dt)
        {
            float framerateScale = (float)dt / (1f / 60f);
            if (flying)
            {
                flying = !standing && !collided;

                if (standing && flyingTimer < 2)
                {
                    position = previousPosition;
                    hVelocity = previousHVelocity;
                    vVelocity = previousVVelocity;
                    position.X += hVelocity * framerateScale;
                    position.Y += vVelocity * framerateScale;
                }

                flyingTimer++;
            }

            if (antiSpamFly)
                antiSpamFly = inputs["c_pressed"];

            previousPosition = position;
            previousHVelocity = hVelocity;
            previousVVelocity = vVelocity;

            //starting to fly
            if (inputs["c_pressed"] && !antiSpamFly && flightCount < 1)
            {
                flying = true;
                antiSpamFly = true;
                flightCount += 1;

                float width = inputs["c_x"] - position.X;
                float height = inputs["c_y"] - position.Y;
                float distance = Vector2.Distance(new Vector2(inputs["c_x"], inputs["c_y"]), position);
                float steps = distance / flyingSpeed;
                angle = (float)(Math.Atan2(inputs["c_y"] - position.Y, inputs["c_x"] - position.X) + Math.PI / 2);
                vVelocity = height / steps;
                hVelocity = width / steps;
            }

            if (!flying)
            {
                flyingTimer = 0;
                //gravity go brrr
                vVelocity += gravity * (float)dt;
                //walking
                if (inputs["m_left"] ^ inputs["m_right"]) //we are not going anywhere if both directions are held
                {
                    if (inputs["m_left"])
                    {
                        hVelocity -= acceleration * (float)dt;
                    }
                    else
                    {
                        hVelocity += acceleration * (float)dt;
                    }
                }
                //Deceleration
                else if (hVelocity != 0)
                {
                    float reduction = decceleration * (float)dt;
                    if (reduction >= Math.Abs(hVelocity))
                    {
                        hVelocity = 0f;
                    }
                    else
                    {
                        if (hVelocity > 0)
                        {
                            hVelocity -= reduction;
                        }
                        else
                        {
                            hVelocity += reduction;
                        }
                    }
                }
                //Not allowing player to move too fast
                if (vVelocity > maxFallSpeed)
                {
                    vVelocity = maxFallSpeed;
                }
                if (Math.Abs(hVelocity) > maxWalkSpeed)
                {
                    if (hVelocity > 0)
                        hVelocity = maxWalkSpeed;
                    else
                        hVelocity = maxWalkSpeed * -1;
                }
            }

            if (standing)
            {
                if (antiSpamJump)
                    antiSpamJump = inputs["m_jump"];

                flightCount = 0;
                if (inputs["m_jump"] && !antiSpamJump)
                {
                    vVelocity = jumpSpeed * -1;
                    standing = false;
                    antiSpamJump = true;
                }
            }

            position.X += hVelocity * framerateScale;
            position.Y += vVelocity * framerateScale;
        }

        public override void Control(GamePadState gamePadState, KeyboardState keyboardState, MouseState mouseState)
        {
            inputs["m_left"] = keyboardState.IsKeyDown(Keys.Left) || gamePadState.IsButtonDown(Buttons.DPadLeft)
                || gamePadState.ThumbSticks.Left.X < Params._movementDeadzone * -1;
            inputs["m_right"] = keyboardState.IsKeyDown(Keys.Right) || gamePadState.IsButtonDown(Buttons.DPadRight)
                || gamePadState.ThumbSticks.Left.X > Params._movementDeadzone; ;
            inputs["c_pressed"] = mouseState.LeftButton == ButtonState.Pressed || gamePadState.Triggers.Right > 0.5f;
            inputs["m_jump"] = keyboardState.IsKeyDown(Keys.Up) || gamePadState.IsButtonDown(Buttons.A);
            if (Params._gamepadUsed)
            {
                inputs["c_x"] = position.X + gamePadState.ThumbSticks.Right.X * cursorDistance;
                inputs["c_y"] = position.Y + gamePadState.ThumbSticks.Right.Y * cursorDistance * -1;
            }
            else
            {
                inputs["c_x"] = mouseState.X / Params._scale;
                inputs["c_y"] = mouseState.Y / Params._scale;
            }
        }

        public override void Draw(SpriteBatch _spritebatch)
        {
            _spritebatch.Draw(sprite, Vector2.Multiply(position, Params._scale), null, Color.White, angle, origin, Params._scale, SpriteEffects.None, 0);
            Vector2 cursorPosition = new Vector2(inputs["c_x"], inputs["c_y"]);
            float cursorOpacity = (Vector2.Distance(cursorPosition, position) + 1) / cursorDistance * 2 + 0.25f;
            _spritebatch.Draw(cursorSprite, Vector2.Multiply(cursorPosition, Params._scale), null, Color.White * cursorOpacity, 0f, cursorOrigin, Params._scale, SpriteEffects.None, 0);
        }
    }
}
