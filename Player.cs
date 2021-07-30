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
        private float decceleration = 5;
        private float maxFallSpeed = 2.5f;
        private float gravity = 3f;
        private float jumpSpeed = 2f;
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

        public Vector2 originalHalfSize;
        public Vector2 flyingSize;

        public Player(Vector2 pos, Texture2D texture)
        {
            name = "player";
            position = pos;
            sprite = texture;
            origin = new Vector2(texture.Width / 2, texture.Height / 2);
            halfSize = new Vector2(texture.Width / 2, texture.Height / 2);

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
            if (flying)
            {
                flying = !standing && !collided;

                if (standing && flyingTimer < 2)
                {
                    position = previousPosition;
                    hVelocity = previousHVelocity;
                    vVelocity = previousVVelocity;
                    position.X += hVelocity;
                    position.Y += vVelocity;
                }

                flyingTimer++;
            }
                

            if (antiSpamFly)
                antiSpamFly = inputs["c_pressed"];

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
                float distance = (float)Math.Sqrt(Math.Pow(width, 2) + Math.Pow(height, 2));
                float steps = distance / jumpSpeed;
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
                flightCount = 0;
                if (inputs["m_jump"] && !antiSpamJump)
                {
                    vVelocity = jumpSpeed * -1;
                    standing = false;
                    antiSpamJump = true;
                }
            }

            if (inputs["m_jump"])
            {
                antiSpamJump = true;
            }
            else
            {
                antiSpamJump = false;
            }

            previousPosition = position;

            position.X += hVelocity;
            position.Y += vVelocity;
        }

        public override void Control(GamePadState gamePadState, KeyboardState keyboardState, MouseState mouseState)
        {
            inputs["m_left"] = keyboardState.IsKeyDown(Keys.Left) || gamePadState.IsButtonDown(Buttons.DPadLeft);
            inputs["m_right"] = keyboardState.IsKeyDown(Keys.Right) || gamePadState.IsButtonDown(Buttons.DPadRight);
            inputs["c_pressed"] = mouseState.LeftButton == ButtonState.Pressed || gamePadState.Triggers.Right > 0.5f;
            //TODO: gamepad support 
            inputs["m_jump"] = keyboardState.IsKeyDown(Keys.Up);
            inputs["c_x"] = mouseState.X / Params._scale;
            inputs["c_y"] = mouseState.Y / Params._scale;
        }
    }
}
