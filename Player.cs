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
        private float maxWalkSpeed = 4f;
        private float acceleration = 15f;
        private float decceleration = 20f;
        private float maxFallSpeed = 10.0f;
        private float gravity = 9.8f;
        private float jumpSpeed = 6f;
        private Dictionary<string, bool> inputs = new Dictionary<string, bool>();

        public Player(Vector2 pos, Texture2D texture)
        {
            position = pos;
            sprite = texture;
            origin = new Vector2(texture.Width / 2, texture.Height / 2);

            collide = false;
            collide_important = true;
            controlable = true;

            inputs.Add("m_left", false);
            inputs.Add("m_right", false);
            inputs.Add("m_jump", false);
        }

        public override void Update(double dt)
        {
            position.X += h_velocity;
            position.Y += v_velocity;

            v_velocity += gravity * (float)dt;

            if (inputs["m_left"] ^ inputs["m_right"]) { //we are not going anywhere if both directions are held
                if (inputs["m_left"])
                {
                    h_velocity -= acceleration * (float)dt;
                } else
                {
                    h_velocity += acceleration * (float)dt;
                }
            } else if (h_velocity != 0)
            {
                float reduction = decceleration * (float)dt;
                if (reduction >= Math.Abs(h_velocity))
                {
                    h_velocity = 0f;
                } else
                {
                    if (h_velocity > 0)
                    {
                        h_velocity -= reduction;
                    } else
                    {
                        h_velocity += reduction;
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
        }

        public override void Control(GamePadState gamePadState, KeyboardState keyboardState)
        {
            inputs["m_left"] = keyboardState.IsKeyDown(Keys.Left) || gamePadState.IsButtonDown(Buttons.DPadLeft);
            inputs["m_right"] = keyboardState.IsKeyDown(Keys.Right) || gamePadState.IsButtonDown(Buttons.DPadRight);
            inputs["m_jump"] = keyboardState.IsKeyDown(Keys.Up);
        }
    }
}
