﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;

namespace aTTH
{
    public class Entity
    {
        public Vector2 position;
        /// <summary>
        /// Only used for moving objects, obviously
        /// </summary>
        public Vector2 prev_position;
        /// <summary>
        /// Only matters for objects with collide_important
        /// </summary>
        public float v_velocity;
        public float h_velocity;
        public bool standing = false;

        public bool collide = false;
        /// <summary>
        /// Is it important for this object to check if it collides with anything else (i.e. players, lasers)
        /// </summary>
        public bool collide_important = false;
        public bool controlable = false;

        public Texture2D sprite;
        public Vector2 origin;
        public Vector2 halfsize;

        public Entity()
        {

        }

        public virtual void Update(double dt)
        {
            
        }

        public virtual void Control(GamePadState gamePadState, KeyboardState keyboardState)
        {

        }

        public virtual void CollissionCheck(List<Entity> entities)
        {
            standing = false;
            for (int i = 0; i < entities.Count; i++)
            {
                if (entities[i].collide)
                {
                    if ((Math.Abs(position.Y - entities[i].position.Y) < (halfsize.Y + entities[i].halfsize.Y) * Params._scale) 
                        & (Math.Abs(position.X - entities[i].position.X) < (halfsize.X + entities[i].halfsize.X) * Params._scale))
                    {
                        //TODO: make it look less awful
                        List<float> dirs = new List<float>();
                        dirs.Add(entities[i].position.Y - (halfsize.Y + entities[i].halfsize.Y) * Params._scale);
                        dirs.Add(entities[i].position.Y + (halfsize.Y + entities[i].halfsize.Y) * Params._scale);
                        dirs.Add(entities[i].position.X - (halfsize.X + entities[i].halfsize.X) * Params._scale);
                        dirs.Add(entities[i].position.X + (halfsize.X + entities[i].halfsize.X) * Params._scale);

                        int smallest = 0;
                        float diff = Math.Abs(prev_position.Y - dirs[0]);
                        for (int u = 1; u < dirs.Count; u++) //no reason to check the first one
                        {
                            if (u == 1)
                            {
                                if (Math.Abs(prev_position.Y - dirs[u]) < diff)
                                {
                                    smallest = u;
                                    diff = Math.Abs(prev_position.Y - dirs[u]);
                                }
                            } else
                            {
                                if (Math.Abs(prev_position.X - dirs[u]) < diff)
                                {
                                    smallest = u;
                                    diff = Math.Abs(prev_position.X - dirs[u]);
                                }
                            }
                        }

                        if (smallest == 0 || smallest == 1)
                        {
                            v_velocity = 0;
                            position.Y = dirs[smallest];
                            standing = true;
                        } else
                        {
                            h_velocity = 0;
                            position.X = dirs[smallest];
                        }
                    }
                }
            }
        }

        public virtual void Draw(SpriteBatch _spritebatch)
        {
            _spritebatch.Draw(sprite, position, null, Color.White, 0, origin, Params._scale, SpriteEffects.None, 0);
        }
    }
}
