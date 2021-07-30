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
        /// <summary>
        /// Used to prevent players from instantly using up all their flights
        /// </summary>
        private bool antiSpam = false;
        private Dictionary<string, dynamic> inputs = new Dictionary<string, dynamic>(); //dynamic is pog af

        public Vector2 originalhalfsize;
        public Vector2 flyingsize;

        public Player(Vector2 pos, Texture2D texture)
        {
            name = "player";
            position = pos;
            sprite = texture;
            origin = new Vector2(texture.Width / 2, texture.Height / 2);
            originalhalfsize = new Vector2(texture.Width / 2, texture.Height / 2);
            halfsize = originalhalfsize;
            flyingsize = new Vector2(originalhalfsize.X, originalhalfsize.X); //not a typo - it's intended to be this way

            collide = false;
            collide_important = true;
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
            prev_position = position;

            if (flying)
                flying = !standing && !collided;

            if (antiSpam)
                antiSpam = inputs["c_pressed"];

            //starting to fly
            if (inputs["c_pressed"] && !antiSpam && flightCount < 2)
            {
                flying = true;
                antiSpam = true;
                flightCount += 1;

                float width = inputs["c_x"] - position.X;
                float height = inputs["c_y"] - position.Y;
                float distance = (float)Math.Sqrt(Math.Pow(width, 2) + Math.Pow(height, 2));
                float steps = distance / jumpSpeed;
                angle = (float)(Math.Atan2(inputs["c_y"] - position.Y, inputs["c_x"] - position.X) + Math.PI / 2);
                halfsize = flyingsize;
                v_velocity = height / steps;
                h_velocity = width / steps;
            }

            if (!flying)
            {
                //fixing collission boxes after flight
                if (halfsize != originalhalfsize)
                {
                    halfsize = originalhalfsize;
                }
                //gravity go brrr
                v_velocity += gravity * (float)dt;
                //walking
                if (inputs["m_left"] ^ inputs["m_right"]) //we are not going anywhere if both directions are held
                {
                    if (inputs["m_left"])
                    {
                        h_velocity -= acceleration * (float)dt;
                    }
                    else
                    {
                        h_velocity += acceleration * (float)dt;
                    }
                }
                //Deceleration
                else if (h_velocity != 0)
                {
                    float reduction = decceleration * (float)dt;
                    if (reduction >= Math.Abs(h_velocity))
                    {
                        h_velocity = 0f;
                    }
                    else
                    {
                        if (h_velocity > 0)
                        {
                            h_velocity -= reduction;
                        }
                        else
                        {
                            h_velocity += reduction;
                        }
                    }
                }
                //Not allowing player to move too fast
                if (v_velocity > maxFallSpeed)
                {
                    v_velocity = maxFallSpeed;
                }
                if (Math.Abs(h_velocity) > maxWalkSpeed)
                {
                    if (h_velocity > 0)
                        h_velocity = maxWalkSpeed;
                    else
                        h_velocity = maxWalkSpeed * -1;
                }
            }

            if (standing)
            {
                flightCount = 0;
                if (inputs["m_jump"])
                {
                    v_velocity = jumpSpeed * -1;
                    standing = false;
                }
            }

            position.X += h_velocity;
            position.Y += v_velocity;
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
