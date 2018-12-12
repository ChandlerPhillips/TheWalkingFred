using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Audio;

namespace TheWalkingFred
{
	class Heart
	{
		//Members
		private Vector2 heartPos;//Contains the (x,y) coords of the heart itself.
		private Rectangle heartRectangle;
		public bool collected;

		private static readonly Random random = new Random();
		private static readonly object syncLock = new object();

		//Constructors
		public Heart()
		{
			//A default Heart object will generate a pre-verified randomized position.
			RandomHeartPosition();

			SetHeartRectangle();//Draw a rectangle around the heart for detecting collision.

			collected = false;
		}//End of Heart Default Constructor.

		//Methods
		public static int RandomNumber(int min, int max)
		{
			//Monogame seeds numbers quickly, this allows for greater variation.
			lock (syncLock)
			{
				return random.Next(min, max);
			}
		}

		public void RandomHeartPosition()
		{
			/***
                All random heart positions are tested for being valid, and placed to challenge the player.
                To Change the number of hearts loaded: refer to the load hearts function in the Game file.
                If another pair of (x,y) coordinates are inserted, assure that the heart can be collected by the player in that position. 
            ***/
			int[,] validPositions = new int[,]
			{
				{405, 240},     //[0][0] and [0][1] 
                {31, 30},
				{35, 299},
				{270, 299},
				{150, 180},
				{555, 30},
				{930, 30},
				{1140, 30},
				{810, 120},
				{870, 450},
				{870, 600},
				{1010, 720},
				{1140, 840},
				{465, 420},
				{660, 835}    //[14][0] and [14][1]
            };

			//Generate a random number that is between index zero and the amount of ROWS only, of the 2D array.
			int randomIndex = RandomNumber(0, validPositions.GetLength(0));

			heartPos.X = validPositions[randomIndex, 0]; //This sets the heart's x coordinate of the randomly selected coordinate.
			heartPos.Y = validPositions[randomIndex, 1]; //This sets the heart's y coordinate of the randomly selected coordinate.
		}

		public Vector2 GetHeartPosition()
		{
			return heartPos;
		}

		public void SetHeartRectangle()
		{
			heartRectangle = new Rectangle((int)heartPos.X + 5, (int)heartPos.Y + 5, 7, 7);
		}

		public Rectangle GetHeartRectangle()
		{
			return heartRectangle;
		}

		public static bool operator ==(Heart obj1, Heart obj2)
		{
			if (obj1.GetHeartPosition() == obj2.GetHeartPosition())
			{
				return true;
			}
			else
			{
				return false;
			}
		}

		public static bool operator !=(Heart obj1, Heart obj2)
		{
			if (obj1.GetHeartPosition() == obj2.GetHeartPosition())
			{
				return false;
			}
			else
			{
				return true;
			}
		}

	}//End of Heart Class
}//End Namespace
