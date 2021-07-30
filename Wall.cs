using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace aTTH
{
    public class Wall : Entity
    {
        public Wall(GraphicsDevice _graphics, Vector2 pos, Vector2 size)
        {
            name = "wall";
            sprite = new Texture2D(_graphics, (int)size.X, (int)size.Y);
            Color[] data = new Color[(int)size.X * (int)size.Y];
            for (int i = 0; i < data.Length; ++i) data[i] = Color.Red;
            sprite.SetData(data);

            origin = new Vector2(size.X / 2, size.Y / 2);
            halfSize = new Vector2(size.X / 2, size.Y / 2);

            position = pos;

            collide = true;
        }
    }
}
