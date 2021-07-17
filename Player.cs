﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace aTTH
{
    public class Player : Entity
    {
        private float maxWalkSpeed = 4f;
        private float acceleration = 15f;
        private float decceleration = 20f;
        private float maxFallSpeed = 10.0f;
        private float gravity = 9.8f;
        private float jumpSpeed = 6f;
        private bool flying = false;
        private Dictionary<string, dynamic> inputs = new Dictionary<string, dynamic>(); //dynamic is pog af

        public Vector2 originalhalfsize;
        public Vector2 flyingsize;

        public Player(Vector2 pos, Texture2D texture)
        {
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
            position.X += h_velocity;
            position.Y += v_velocity;

            if (flying)
                flying = !standing && !collided;

            if (halfsize != originalhalfsize && !flying)
            {
                halfsize = originalhalfsize;
            }

            if (!flying)
            {
                //gravity go brrr
                v_velocity += gravity * (float)dt;
                //starting to fly
                if (inputs["c_pressed"])
                {
                    flying = true;

                    float width = inputs["c_x"] - position.X;
                    float height = inputs["c_y"] - position.Y;
                    float distance = (float)Math.Sqrt(Math.Pow(width, 2) + Math.Pow(height, 2));
                    float steps = distance / jumpSpeed;
                    angle = (float)(Math.Atan2(inputs["c_y"] - position.Y, inputs["c_x"] - position.X) + Math.PI / 2);
                    halfsize = flyingsize;
                    v_velocity = height / steps;
                    h_velocity = width / steps;
                }
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
            }

            if (inputs["m_jump"] && standing)
            {
                v_velocity = jumpSpeed * -1;
                standing = false;
            }

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

            prev_position = position;
        }

        public override void Control(GamePadState gamePadState, KeyboardState keyboardState, MouseState mouseState)
        {
            inputs["m_left"] = keyboardState.IsKeyDown(Keys.Left) || gamePadState.IsButtonDown(Buttons.DPadLeft);
            inputs["m_right"] = keyboardState.IsKeyDown(Keys.Right) || gamePadState.IsButtonDown(Buttons.DPadRight);
            //TODO: gamepad support 
            inputs["m_jump"] = keyboardState.IsKeyDown(Keys.Up);
            inputs["c_x"] = mouseState.X;
            inputs["c_y"] = mouseState.Y;
            inputs["c_pressed"] = mouseState.LeftButton == ButtonState.Pressed || gamePadState.Triggers.Right > 0.5f;
        }
    }
}
