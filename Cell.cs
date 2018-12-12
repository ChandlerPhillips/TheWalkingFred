using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace TheWalkingFred
{
	class Cell
	{
		public int Block;
		public Vector2 Pos;
		public Cell(int block, int xpos, int ypos)
		{
			Block = block;
			Pos.X = xpos;
			Pos.Y = ypos;
		}
	}
}
