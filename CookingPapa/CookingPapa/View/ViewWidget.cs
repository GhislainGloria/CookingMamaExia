﻿using System.Collections.Generic;
using System.Windows.Forms;
using System.Drawing;
using Controller;
using System;

namespace View
{
    public class ViewWidget
    {
		private int TileSize;
		private const int ViewWidth = 800;
		private const int ViewHeight = 600;
		private Size WorldDimensions;
        private Brush TileBrush;
        private Brush WallBrush;
        private RestaurantMap _Model;

        public PictureBox PictureBox { get; set; }
		public RestaurantMap Model 
		{
			get { return _Model; }
			set {
				_Model = value;
				WorldDimensions = _Model.MapSize;
				TileSize = Math.Min(
					ViewWidth / _Model.MapSize.Width,
					ViewHeight / _Model.MapSize.Height
				);
				TextureFactory.TileDimension = TileSize;
			}
		}


        /**
         * Constructor
         */
        public ViewWidget()
        {
            PictureBox = new PictureBox();
			PictureBox.Size = new Size(ViewWidth, ViewHeight);
            PictureBox.Paint += new PaintEventHandler(this.PaintPictureBox);

        }

        /**
         * Callback when the picturebox needs to be redrawn
         */
        public void PaintPictureBox(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;

            // We draw the loor (background and majority of tiles)
            TileBrush = TextureFactory.CreateBrush("tile");
            WallBrush = TextureFactory.CreateBrush("wall");
            for (int i = 0; i < WorldDimensions.Width; i++) {
				for (int j = 0; j < WorldDimensions.Height; j++) {
                    if(j == 0 || j == 1)
                        g.FillRectangle(WallBrush, new Rectangle(i * TileSize, j * TileSize, TileSize, TileSize));
                    else
                        g.FillRectangle(TileBrush, new Rectangle(i * TileSize, j * TileSize, TileSize, TileSize));
				}
			}

            // We draw the actors above the floor
			foreach (Tuple<string, Point> actor in Model.DisplayableData()) {
				TileBrush = TextureFactory.CreateBrush(actor.Item1);
				g.FillRectangle(
					TileBrush,
					new Rectangle(actor.Item2.X * TileSize, actor.Item2.Y * TileSize, TileSize, TileSize)
				);
			}
			// => PaintPictureBox

        }
    }
}