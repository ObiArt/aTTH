using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace aTTH
{
    public class Entity
    {
        public string name = "entity";
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
        public Single angle = 0;
        public bool standing = false;

        /// <summary>
        /// Can entity be collided with?
        /// </summary>
        public bool collide = false;
        /// <summary>
        /// Did entity collide in the previous frame?
        /// </summary>
        public bool collided = false;
        public string collidedWith = "";
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

        public virtual void Control(GamePadState gamePadState, KeyboardState keyboardState, MouseState mouseState)
        {

        }

        public virtual void CollissionCheck(List<Entity> entities)
        {
            standing = false;
            collided = false;
            for (int i = 0; i < entities.Count; i++)
            {
                if (entities[i].collide && entities[i].name != name) //same objects cannot collide with each other
                {
                    if ((Math.Abs(position.Y - entities[i].position.Y) < (halfsize.Y + entities[i].halfsize.Y)) 
                        & (Math.Abs(position.X - entities[i].position.X) < (halfsize.X + entities[i].halfsize.X)))
                    {
                        collided = true;
                        collidedWith = entities[i].name;
                        angle = 0;
                        if (prev_position.Y < entities[i].position.Y - entities[i].halfsize.Y) //Top
                        {
                            position.Y = entities[i].position.Y - entities[i].halfsize.Y - halfsize.Y;
                            v_velocity = 0;
                            standing = true;
                            //Debug.WriteLine("top");
                        } else if (prev_position.X < entities[i].position.X - entities[i].halfsize.X) //Left
                        {
                            position.X = entities[i].position.X - entities[i].halfsize.X - halfsize.X;
                            h_velocity = 0;
                            //Debug.WriteLine("left");
                        } else if (prev_position.X > entities[i].position.X + entities[i].halfsize.X) //Right
                        {
                            position.X = entities[i].position.X + entities[i].halfsize.X + halfsize.X;
                            h_velocity = 0;
                            //Debug.WriteLine("right");
                        } else //Bottom
                        //if (prev_position.Y > entities[i].position.Y + entities[i].halfsize.Y) 
                        {
                            position.Y = entities[i].position.Y + entities[i].halfsize.Y + halfsize.Y;
                            v_velocity = 0;
                            //Debug.WriteLine("bottom");
                        }
                    }
                }
            }
        }

        public virtual void Draw(SpriteBatch _spritebatch)
        {
            _spritebatch.Draw(sprite, Vector2.Multiply(position, Params._scale), null, Color.White, angle, origin, Params._scale, SpriteEffects.None, 0);
        }
    }
}
