using System;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MPQNav.ADT;
using MPQNav.Graphics;

namespace MPQNav {
    /// <summary>
	/// This is the class that controls the entire game.
	/// </summary>
	public class Game1 : Game {
		// XNA uses this variable for graphics information
		private readonly GraphicsDeviceManager graphics;
        
        // Another set of XNA Variables
		private Matrix view;
		private Matrix proj;

		private BasicEffect basicEffect;

		private readonly Map manager;

		/// <summary>
		/// Console used to execute commands while the game is running.
		/// </summary>
		private MpqConsole console;

		// Camera Stuff
		private Vector3 avatarPosition = new Vector3(-100, 100, -100);
        private float avatarYaw;
        private Vector3 thirdPersonReference = new Vector3(0, 20, -20);
        private const float RotationSpeed = 1f/60f;
        private const float ForwardSpeed = 50f/60f;
        private const float ViewAngle = MathHelper.PiOver4;
        private const float NearClip = 1.0f;
        private const float FarClip = 2000.0f;

        private SpriteBatch spriteBatch;
		private SpriteFont spriteFont;

		/// <summary>
		/// Constructor for the game.
		/// </summary>
		public Game1() {
			graphics = new GraphicsDeviceManager(this);
			Content.RootDirectory = "Content";

			manager = new Map(MpqNavSettings.DefaultContinent);

			manager.LoadADT(MpqNavSettings.DefaultMapX, MpqNavSettings.DefaultMapY);

			var renderVerticies = manager.TriangleList.Vertices.ToArray();
			var renderIndices = manager.TriangleList.Indices.ToArray();
			if(renderIndices.Length > 0) {
				avatarPosition = renderVerticies[0].Position;
				avatarYaw = 90;
			}
		}

		/// <summary>
		/// Executes a console command.
		/// </summary>
		private void DoCommand() {
			if(console.Command.commandCode.Equals(MpqConsole.ConsoleCommandStruct.CommandCode.Load)) {
				String command = console.Command.commandData;
				int mapX = int.Parse(command.Split(' ')[0]);
				int mapY = int.Parse(command.Split(' ')[1]);
				console.WriteLine(string.Format("Loading map:{0} {1}", mapX, mapY));
				manager.LoadADT(mapX, mapY);
			}
		}

		/// <summary>
		/// Loads the content needed for the game.
		/// </summary>
		protected override void LoadContent() {
			var font = Content.Load<SpriteFont>(@"mfont");
			spriteFont = font;
			console = new MpqConsole(this, font);
			console.MyEvent += DoCommand;

			spriteBatch = new SpriteBatch(GraphicsDevice);
		}


		/// <summary>
		/// Allows the game to perform any initialization it needs to before starting to run.
		/// This is where it can query for any required services and load any non-graphic
		/// related content.  Calling base.Initialize will enumerate through any components
		/// and initialize them as well.
		/// </summary>
		protected override void Initialize() {
			graphics.PreferredBackBufferWidth = 800;
			graphics.PreferredBackBufferHeight = 600;
			graphics.IsFullScreen = false;
			graphics.ApplyChanges();
			InitializeEffect();
			base.Initialize();
		}

		private void InitializeEffect() {
			basicEffect = new BasicEffect(graphics.GraphicsDevice) {
				VertexColorEnabled = true,
				View = view,
				Projection = proj
			};
		}

		/// <summary>
		/// UnloadContent will be called once per game and is the place to unload
		/// all content.
		/// </summary>
		protected override void UnloadContent() {
		}

		/// <summary>
		/// Allows the game to run logic such as updating the world,
		/// checking for collisions, gathering input, and playing audio.
		/// </summary>
		/// <param name="gameTime">Provides a snapshot of timing values.</param>
		protected override void Update(GameTime gameTime) {
			UpdateAvatarPosition();
			base.Update(gameTime);
		}

		/// <summary>
		/// This is called when the game should draw itself.
		/// </summary>
		/// <param name="gameTime">Provides a snapshot of timing values.</param>
		protected override void Draw(GameTime gameTime) {
			graphics.GraphicsDevice.DepthStencilState = DepthStencilState.Default;
			graphics.GraphicsDevice.Clear(Color.Black);

			// Update our camera
			UpdateCameraThirdPerson();

			basicEffect.Projection = proj;
			basicEffect.View = view;

			basicEffect.Alpha = 1.0f;
			basicEffect.DiffuseColor = new Vector3(.75f, .75f, .75f);
			basicEffect.SpecularColor = new Vector3(0.25f, 0.25f, 0.25f);
			basicEffect.SpecularPower = 5.0f;

			basicEffect.AmbientLightColor = new Vector3(0.75f, 0.75f, 0.75f);
			basicEffect.DirectionalLight0.Enabled = true;
			basicEffect.DirectionalLight0.DiffuseColor = Vector3.One;
			basicEffect.DirectionalLight0.Direction = Vector3.Normalize(new Vector3(1.0f, -1.0f, -1.0f));
			basicEffect.DirectionalLight0.SpecularColor = Vector3.One;

			basicEffect.DirectionalLight1.Enabled = true;
			basicEffect.DirectionalLight1.DiffuseColor = new Vector3(0.8f, 0.8f, 0.8f);
			basicEffect.DirectionalLight1.Direction = Vector3.Normalize(new Vector3(-1.0f, -1.0f, 1.0f));

			basicEffect.LightingEnabled = true;

			foreach(EffectPass pass in basicEffect.CurrentTechnique.Passes) {
				pass.Apply();
  			    graphics.GraphicsDevice.RasterizerState = new RasterizerState {FillMode = FillMode.Solid};
              Render();
			}

			DrawCameraState();
			spriteBatch.Begin();
			spriteBatch.DrawString(spriteFont, "Esc: Opens the console.",
			                       new Vector2(10, graphics.GraphicsDevice.Viewport.Height - 30), Color.White);
			spriteBatch.End();
			base.Draw(gameTime);
		}

		private void Render() {
			graphics.GraphicsDevice.DrawTriangleList(manager.TriangleList);
		}

		private void DrawCameraState() {
			graphics.GraphicsDevice.RasterizerState = RasterizerState.CullNone;
			graphics.GraphicsDevice.DepthStencilState = DepthStencilState.None;
		}

		/// <summary>
		/// This is the method that is called to move our avatar 
		/// </summary>
		/// <code>
		/// // create the class that does translations
		/// GiveHelpTransforms ght = new GiveHelpTransforms();
		/// // have it load our XML into the SourceXML property
		/// ght.LoadXMLFromFile(
		///      "E:\\Inetpub\\wwwroot\\GiveHelp\\GiveHelpDoc.xml");
		/// </code>
		private void UpdateAvatarPosition() {
			KeyboardState keyboardState = Keyboard.GetState();
			GamePadState currentState = GamePad.GetState(PlayerIndex.One);

		    if (console.IsOpen())
		        return;

		    if(keyboardState.IsKeyDown(Keys.P)) {
		        Console.WriteLine("Open!");
		    }

		    if(keyboardState.IsKeyDown(Keys.A) || (currentState.DPad.Left == ButtonState.Pressed)) {
		        // Rotate left.
		        avatarYaw += RotationSpeed;
		    }

		    if(keyboardState.IsKeyDown(Keys.D) || (currentState.DPad.Right == ButtonState.Pressed)) {
		        // Rotate right.
		        avatarYaw -= RotationSpeed;
		    }

		    if(keyboardState.IsKeyDown(Keys.W) || (currentState.DPad.Up == ButtonState.Pressed)) {
		        Matrix forwardMovement = Matrix.CreateRotationY(avatarYaw);
		        var v = new Vector3(0, 0, ForwardSpeed);
		        v = Vector3.Transform(v, forwardMovement);
		        avatarPosition.Z += v.Z;
		        avatarPosition.X += v.X;
		    }

		    if(keyboardState.IsKeyDown(Keys.S) || (currentState.DPad.Down == ButtonState.Pressed)) {
		        Matrix forwardMovement = Matrix.CreateRotationY(avatarYaw);
		        var v = new Vector3(0, 0, -ForwardSpeed);
		        v = Vector3.Transform(v, forwardMovement);
		        avatarPosition.Z += v.Z;
		        avatarPosition.X += v.X;
		    }

		    if(keyboardState.IsKeyDown(Keys.F)) {
		        avatarPosition.Y = avatarPosition.Y - 1;
		    }

		    if(keyboardState.IsKeyDown(Keys.R) || keyboardState.IsKeyDown(Keys.Space)) {
		        avatarPosition.Y = avatarPosition.Y + 1;
		    }

		    if(keyboardState.IsKeyDown(Keys.E)) {
		        var forwardMovement = Matrix.CreateRotationY(avatarYaw);
		        var v = new Vector3(1, 0, 0);
		        v = Vector3.Transform(v, forwardMovement);
		        avatarPosition.X -= v.X;
		        avatarPosition.Y -= v.Y;
		        avatarPosition.Z -= v.Z;
		    }

		    if(keyboardState.IsKeyDown(Keys.Q)) {
		        Matrix forwardMovement = Matrix.CreateRotationY(avatarYaw);
		        var v = new Vector3(1, 0, 0);
		        v = Vector3.Transform(v, forwardMovement);
		        avatarPosition.X += v.X;
		        avatarPosition.Y += v.Y;
		        avatarPosition.Z += v.Z;
		    }

		    if(keyboardState.IsKeyDown(Keys.T)) {
		        thirdPersonReference.Y = thirdPersonReference.Y - 0.25f;
		    }
		    if(keyboardState.IsKeyDown(Keys.G)) {
		        thirdPersonReference.Y = thirdPersonReference.Y + 0.25f;
		    }
		}

		private void UpdateCameraThirdPerson() {
			Matrix rotationMatrix = Matrix.CreateRotationY(avatarYaw);

			// Create a vector pointing the direction the camera is facing.
			Vector3 transformedReference = Vector3.Transform(thirdPersonReference, rotationMatrix);

			// Calculate the position the camera is looking from.
			Vector3 cameraPosition = transformedReference + avatarPosition;

			// Set up the view matrix and projection matrix.
			view = Matrix.CreateLookAt(cameraPosition, avatarPosition, new Vector3(0.0f, 1.0f, 0.0f));

			Viewport viewport = graphics.GraphicsDevice.Viewport;
			float aspectRatio = viewport.Width / (float)viewport.Height;

			proj = Matrix.CreatePerspectiveFieldOfView(ViewAngle, aspectRatio, NearClip, FarClip);
		}
	}
}