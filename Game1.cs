/******
* Project: Team Turbo - Final Video Game
* Class:   CSC 575/275 - Game Programming
* Year:    Spring 2017
******/

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Media;
using System.IO;
using System;
using System.Collections.Generic;

namespace TheWalkingFred
{

	public class Game1 : Game
	{
		Camera camera;
		bool start = false;
		bool gameover = false;
		GraphicsDeviceManager graphics;
		SpriteBatch spriteBatch;
		public Rectangle spriteRectangle;
		Rectangle fullScreenRect = new Rectangle(); //This can be used for all photos that fill the entire screen (main menu, win screen, game over, etc).
		Cell[,] Maze;
		Texture2D wall;
		Texture2D floor;
		Texture2D player;
		Texture2D stone;
		Texture2D startScreen;
		Texture2D winnerScreen;
		Texture2D zombie;
		Texture2D dirt;
		Texture2D heart;
		SoundEffect startMusic;
		SoundEffectInstance startMusicInstance;
		List<SoundEffect> sfx = new List<SoundEffect>();
		public Vector2 playerPos;
		Vector2 zombiePos1;//This may be best placed in a list or an array rather than many unique variables.
		List<Heart> hearts = new List<Heart>();
		int nrow, ncol;
		int gWidth = 1200;
		int gHeight = 930;
		int cellSize = 30;
		int heartsToCollect;
		int heartsCollected; //This could be used to display hearts collected.
		KeyboardState kbrd;
		int speed = 2;
		int zombieSpeed = 1;

		//Player Collision Data
		Point playerFrameSize = new Point(10);
		int collisionOffset = 10;



		public Game1()
		{
			graphics = new GraphicsDeviceManager(this);
			graphics.PreferredBackBufferWidth = gWidth;
			graphics.PreferredBackBufferHeight = gHeight;
			Content.RootDirectory = "Content";
		}



		//Utility Functions

		public void load_maze()
		{
			string str;
			StreamReader tr = new StreamReader("../../../../maze_data.csv");
			str = tr.ReadLine();
			string[] stary = str.Split(',');
			ncol = Convert.ToInt32(stary[0]);
			nrow = Convert.ToInt32(stary[1]);
			Maze = new Cell[nrow, ncol];
			for (int r = 0; r < nrow; r++)
			{
				str = tr.ReadLine();
				stary = str.Split(',');
				for (int c = 0; c < ncol; c++)
					Maze[r, c] = new Cell(Convert.ToInt32(stary[c]), c * cellSize, r * cellSize);
			}
			tr.Close();
		}//End load_maze()

		public void load_hearts()
		{
			//This determines how many hearts are loaded, to be later drawn.
			//Generates uniquely positioned hearts based on the pre-selected set in the Heart class.

			//WARNING: DO NOT set this to a number greater than the heart positions available (see Heart class). The result is an infinite loop.
			heartsToCollect = 15;

			int i = 0;
			while (i < heartsToCollect)
			{
				Heart randomHeart = new Heart();
				bool unique = true;

				foreach (Heart h in hearts)
				{
					if (randomHeart == h)
					{
						unique = false;
						break;
					}
				}
				//Only use the seeded heart if its position is not currently in use.
				if (unique)
				{
					hearts.Add(randomHeart);
					i++;
				}
			}
		}//End load_hearts()


		public void gamestart()
		{
			startMusicInstance.Play();
			gameover = false;
		}

		public void playRandomSFX()
		{
			//Plays one random SFX upon function call.
			//Since the Heart class already contains a useful random number generator, it is reused.
			int randomIndex = Heart.RandomNumber(0, sfx.Count);
			sfx[randomIndex].Play();
		}

		public void heartCollision()
		{
			//Generage a rectangle based upon the current user position.
			Rectangle playerRectanlge = new Rectangle(
			(int)playerPos.X + collisionOffset,
			(int)playerPos.Y + collisionOffset,
			playerFrameSize.X,
			playerFrameSize.Y
			);

			//This tests and handles the player's collision with the unique heart positions.
			for (int i = 0; i < hearts.Count; i++)
			{
				if (hearts[i].collected == false && playerRectanlge.Intersects(hearts[i].GetHeartRectangle()))
				{
					hearts[i].collected = true;
					hearts.RemoveAt(i);
					playRandomSFX();//This generates the SFX.
				}
			}
		}//End heartCollision()

		public bool moveOK(int dir, Vector2 pos)
		{
			int r1, c1, r2, c2;
			bool rtnval = true;
			if (dir == 0)
			{
				r1 = (int)(playerPos.Y + 1 - speed) / cellSize;
				c1 = (int)(playerPos.X + 1) / cellSize;
				r2 = r1;
				c2 = (int)(playerPos.X + cellSize - 2) / cellSize;
				if (Maze[r1, c1].Block == 2 || Maze[r2, c2].Block == 2)
					rtnval = false;
			}
			if (dir == 2)
			{
				r1 = (int)(playerPos.Y - 1 + cellSize - 2 + speed) / cellSize;
				c1 = (int)(playerPos.X + 1) / cellSize;
				r2 = r1;
				c2 = (int)(playerPos.X + cellSize - 2) / cellSize;
				if (Maze[r1, c1].Block == 2 || Maze[r2, c2].Block == 2)
					rtnval = false;
			}
			if (dir == 1)
			{
				r1 = (int)(playerPos.Y + 1) / cellSize;
				c1 = (int)(playerPos.X + cellSize - 2 + speed) / cellSize;
				r2 = (int)(playerPos.Y + cellSize - 2) / cellSize;
				c2 = c1;
				if (Maze[r1, c1].Block == 2 || Maze[r2, c2].Block == 2)
					rtnval = false;
			}
			if (dir == 3)
			{
				r1 = (int)(playerPos.Y + 1) / cellSize;
				c1 = (int)(playerPos.X - speed) / cellSize;
				r2 = (int)(playerPos.Y + cellSize - 2) / cellSize;
				c2 = c1;
				if (Maze[r1, c1].Block == 2 || Maze[r2, c2].Block == 2)
					rtnval = false;
			}

			return rtnval;
		}

		public bool zombieMoveOK(int dir, Vector2 pos)
		{
			int r1, c1, r2, c2;
			bool rtnval1 = true;

			if (dir == 0)
			{
				r1 = (int)(zombiePos1.Y + 1 - speed) / cellSize;
				c1 = (int)(zombiePos1.X + 1) / cellSize;
				r2 = r1;
				c2 = (int)(zombiePos1.X + cellSize - 2) / cellSize;
				if (Maze[r1, c1].Block == 2 || Maze[r2, c2].Block == 2)
					rtnval1 = false;
			}
			if (dir == 2)
			{
				r1 = (int)(zombiePos1.Y - 1 + cellSize - 2 + speed) / cellSize;
				c1 = (int)(zombiePos1.X + 1) / cellSize;
				r2 = r1;
				c2 = (int)(zombiePos1.X + cellSize - 2) / cellSize;
				if (Maze[r1, c1].Block == 2 || Maze[r2, c2].Block == 2)
					rtnval1 = false;
			}
			if (dir == 1)
			{
				r1 = (int)(zombiePos1.Y + 1) / cellSize;
				c1 = (int)(zombiePos1.X + cellSize - 2 + speed) / cellSize;
				r2 = (int)(zombiePos1.Y + cellSize - 2) / cellSize;
				c2 = c1;
				if (Maze[r1, c1].Block == 2 || Maze[r2, c2].Block == 2)
					rtnval1 = false;
			}
			if (dir == 3)
			{
				r1 = (int)(zombiePos1.Y + 1) / cellSize;
				c1 = (int)(zombiePos1.X - speed) / cellSize;
				r2 = (int)(zombiePos1.Y + cellSize - 2) / cellSize;
				c2 = c1;
				if (Maze[r1, c1].Block == 2 || Maze[r2, c2].Block == 2)
					rtnval1 = false;

			}
			return rtnval1;
		}



		//Game Engine Functions
		protected override void Initialize()
		{
			// TODO: Add your initialization logic here

			base.Initialize();
			load_maze();
			load_hearts();
			camera = new TheWalkingFred.Camera(GraphicsDevice.Viewport);

			//Player and Zombie positions at start.
			playerPos.X = cellSize + 1;
			playerPos.Y = gHeight - 2 * cellSize - 1;
			zombiePos1.X = 300;
			zombiePos1.Y = 30;

		}

		protected override void LoadContent()
		{
			// Create a new SpriteBatch, which can be used to draw textures.
			spriteBatch = new SpriteBatch(GraphicsDevice);
			wall = Content.Load<Texture2D>("wall");
			floor = Content.Load<Texture2D>("floor");
			player = Content.Load<Texture2D>("player");
			stone = Content.Load<Texture2D>("stone");
			dirt = Content.Load<Texture2D>("dirt");
			startScreen = Content.Load<Texture2D>("StartScreen");
			winnerScreen = Content.Load<Texture2D>("temporary Endslate"); //This needs to be changed to the final WIN screen later.
			zombie = Content.Load<Texture2D>("Zombie_Front");
			heart = Content.Load<Texture2D>("Heart");
			fullScreenRect.Width = gWidth;
			fullScreenRect.Height = gHeight;

			gamestart();
			// TODO: use this.Content to load your game content here
		}

		protected override void UnloadContent()
		{

		}

		/// <summary>
		/// Allows the game to run logic such as updating the world,
		/// checking for collisions, gathering input, and playing audio.
		/// </summary>
		/// <param name="gameTime">Provides a snapshot of timing values.</param>
		protected override void Update(GameTime gameTime)
		{
			if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
				Exit();

			kbrd = Keyboard.GetState();
			//Player Updating
			if (kbrd.IsKeyDown(Keys.Up) && moveOK(0, playerPos))
				playerPos.Y -= speed;
			if (kbrd.IsKeyDown(Keys.Down) && moveOK(2, playerPos))
				playerPos.Y += speed;
			if (kbrd.IsKeyDown(Keys.Left) && moveOK(3, playerPos))
				playerPos.X -= speed;
			if (kbrd.IsKeyDown(Keys.Right) && moveOK(1, playerPos))
				playerPos.X += speed;
			//Zombie Updating
			if (zombiePos1.X < playerPos.X && zombieMoveOK(0, zombiePos1))
				zombiePos1.X += zombieSpeed;
			if (zombiePos1.Y < playerPos.Y && zombieMoveOK(2, zombiePos1))
				zombiePos1.Y += zombieSpeed;
			if (zombiePos1.X > playerPos.X && zombieMoveOK(3, zombiePos1))
				zombiePos1.X -= zombieSpeed;
			if (zombiePos1.Y > playerPos.Y && zombieMoveOK(4, zombiePos1))
				zombiePos1.Y -= zombieSpeed;
			//Heart Updating
			heartCollision();


			if (kbrd.IsKeyDown(Keys.Space))
			{
				if (gameover && start) gamestart();
				else start = true;
			}


			camera.Update(gameTime, this);
			base.Update(gameTime);
		}

		/// <summary>
		/// This is called when the game should draw itself.
		/// </summary>
		/// <param name="gameTime">Provides a snapshot of timing values.</param>
		protected override void Draw(GameTime gameTime)
		{
			spriteBatch.Begin(SpriteSortMode.Deferred,
				BlendState.AlphaBlend,
				null, null, null, null,
				camera.transform);

			if (!start)
			{
				//This is the starting (main) menu.
				spriteBatch.Draw(startScreen, fullScreenRect, Color.White);
			}
			else if (start && hearts.Count == 0)
			{
				//This is the WIN screen the users sees when they have collected all hearts.
				startMusicInstance.Stop();//This stops the game music on the end screen.
				spriteBatch.Draw(winnerScreen, fullScreenRect, Color.White);
			}
			else
			{
				GraphicsDevice.Clear(Color.White);
				//spriteBatch.Begin();
				for (int r = 0; r < nrow; r++)
				{
					for (int c = 0; c < ncol; c++)
					{
						if (Maze[r, c].Block == 1)
							spriteBatch.Draw(floor, Maze[r, c].Pos, Color.White);
						else
						if (Maze[r, c].Block == 3)
							spriteBatch.Draw(stone, Maze[r, c].Pos, Color.White);
						else
						if (Maze[r, c].Block == 4)
							spriteBatch.Draw(dirt, Maze[r, c].Pos, Color.White);
						else
							spriteBatch.Draw(wall, Maze[r, c].Pos, Color.White);
					}
				}

				spriteBatch.Draw(zombie, zombiePos1, Color.White);
				spriteBatch.Draw(player, playerPos, Color.White);

				//Draw all uncollected hearts in the heart list.
				foreach (Heart h in hearts)
				{
					if (h.collected == false) spriteBatch.Draw(heart, h.GetHeartPosition(), Color.White);
				}
			}

			//These functions must remain at the end of the Draw() function.
			//Do not place them inside conditionals.
			spriteBatch.End();
			base.Draw(gameTime);
		}//End of Draw
	}
}