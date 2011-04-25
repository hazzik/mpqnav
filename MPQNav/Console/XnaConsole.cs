//
// Xna Console
// www.codeplex.com/XnaConsole
// Copyright (c) 2008 Samuel Christie
//
using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

//using IronPython.Hosting;
//using IronPython.Runtime.Exceptions;
//using IronPython.Modules;

namespace XnaConsole {
	/// <summary>
	/// Different handlers for input (Python for example).
	/// </summary>
	/// <param name="str"></param>
	public delegate void InputHandler(string str);

	/// <remarks>This class creates a graphical text console for running code while executing a game.</remarks>
	public class XnaConsoleComponent : DrawableGameComponent {
		private readonly Game game;
		private InputHandler input;

		#region Configuration constants

		private const double AnimationTime = 0.05;
		private const double CursorBlinkTime = 0.3;
		private const int LinesDisplayed = 15;
		private const string NewLine = "\n";
		private const string Version = "Xna Console v.1.0";

		#endregion

		#region Rendering stuff

		private readonly Texture2D background;
		private readonly GraphicsDevice device;
		private readonly SpriteFont font;
		private readonly SpriteBatch spriteBatch;

		#endregion

		#region Console text management stuff

		private readonly double firstInterval;
		private readonly History history;
		private readonly Dictionary<Keys, double> keyTimes;
		private readonly double repeatInterval;
		private int consoleXSize, consoleYSize;
		private int cursorOffset;
		private int cursorPos;
		private string inputBuffer;
		private int lineWidth;
		private string outputBuffer;

		#endregion

		#region State and timing management stuff

		private KeyboardState currentKeyState;
		private KeyboardState lastKeyState;
		private ConsoleState state;
		private double stateStartTime;

		#endregion

		/// <summary>
		/// Initializes a new instance of the class, which creates a console for executing commands while running a game.
		/// </summary>
		/// <param name="game"></param>
		/// <param name="font"></param>
		public XnaConsoleComponent(Game game, SpriteFont font)
			:
				base(game) {
			this.game = game;
			device = game.GraphicsDevice;
			spriteBatch = new SpriteBatch(device);
			this.font = font;
			background = new Texture2D(device, 1, 1, false, SurfaceFormat.Color);
			background.SetData(new[] { new Color(50, 50, 50, 100) });

			inputBuffer = "";
			history = new History();

			WriteLine("### ");
			WriteLine("### " + Version);
			WriteLine("### ");

			consoleXSize = Game.Window.ClientBounds.Right - Game.Window.ClientBounds.Left - 20;
			consoleYSize = font.LineSpacing * LinesDisplayed + 20;
			lineWidth = (int)(consoleXSize / font.MeasureString("a").X) - 2;
			//calculate number of letters that fit on a line, using "a" as example character

			state = ConsoleState.Closed;
			stateStartTime = 0;
			lastKeyState = currentKeyState = Keyboard.GetState();
			firstInterval = 500f;
			repeatInterval = 50f;

			//used for repeating keystrokes
			keyTimes = new Dictionary<Keys, double>();
			for(int i = 0; i < Enum.GetValues(typeof(Keys)).Length; i++) {
				var key = (Keys)Enum.GetValues(typeof(Keys)).GetValue(i);
				keyTimes[key] = 0f;
			}
		}

		/// <summary>
		/// Is this console open?
		/// </summary>
		/// <returns>Boolean value representing if the console is open</returns>
		public bool IsOpen() {
			if(state.Equals(ConsoleState.Open) || state.Equals(ConsoleState.Open)) {
				return true;
			}
			return false;
		}

		/// <summary>
		/// Returns a string with one less character at the end
		/// </summary>
		/// <param name="str">String to chomp</param>
		/// <returns>String with one less character at the end</returns>
		public string Chomp(string str) {
			if(str.Length > 0 && str.Substring(str.Length - 1, 1) == "\n") {
				return str.Substring(0, str.Length - 1);
			}
			return str;
		}

		/// <summary>
		/// This takes a single string and splits it at the newlines and the specified number of columns
		/// </summary>
		/// <param name="line"></param>
		/// <param name="columns"></param>
		/// <returns></returns>
		private List<string> WrapLine(string line, int columns) {
			var wraplines = new List<string>();
			if(line.Length > 0) {
				wraplines.Add("");
				int lineNum = 0;

				for(int i = 0; i < line.Length; i++) {
					string ch = line.Substring(i, 1);

					if(ch == "\n" || wraplines[lineNum].Length > columns) {
						wraplines.Add("");
						lineNum++;
					}
					else {
						wraplines[lineNum] += ch;
					}
				}
			}

			return wraplines;
		}

		/// <summary>
		/// Write to the console
		/// </summary>
		/// <param name="str"></param>
		public void Write(string str) {
			outputBuffer += str;
		}

		/// <summary>
		/// Write a line to the console
		/// </summary>
		/// <param name="str"></param>
		public void WriteLine(string str) {
			Write(str + NewLine);
		}

		/// <summary>
		/// Clears the output.
		/// </summary>
		public void Clear() {
			outputBuffer = "";
		}

		/// <summary>
		/// Clears the command history.
		/// </summary>
		public void ClearHistory() {
			history.Clear();
		}

		/// <summary>
		/// Prompts for input asynchronously via callback
		/// </summary>
		/// <param name="str"></param>
		/// <param name="callback"></param>
		public void Prompt(string str, InputHandler callback) {
			Write(str);
			string[] lines = WrapLine(outputBuffer, lineWidth).ToArray();
			input = callback;
			cursorOffset = lines[lines.Length - 1].Length;
		}

		/// <summary>
		/// Opens the console
		/// </summary>
		public void Open() {
			if(state == ConsoleState.Closed) {
				state = ConsoleState.Opening;
				Visible = true;
			}
		}

		/// <summary>
		/// Updates
		/// </summary>
		/// <param name="gameTime">Not Sure</param>
		public override void Update(GameTime gameTime) {
			double now = gameTime.TotalGameTime.TotalSeconds;
			double elapsedTime = gameTime.ElapsedGameTime.TotalMilliseconds; //time since last update call

			//get keyboard state
			lastKeyState = currentKeyState;
			currentKeyState = Keyboard.GetState();

			#region Closing & Opening states management

			if(state == ConsoleState.Closing) {
				if(now - stateStartTime > AnimationTime) {
					state = ConsoleState.Closed;
					stateStartTime = now;
				}

				return;
			}

			if(state == ConsoleState.Opening) {
				if(now - stateStartTime > AnimationTime) {
					state = ConsoleState.Open;
					stateStartTime = now;
				}

				return;
			}

			#endregion

			#region Closed state management

			if(state == ConsoleState.Closed) {
				if(IsKeyPressed(Keys.Escape)) //this opens the console
				{
					state = ConsoleState.Opening;
					stateStartTime = now;
					Visible = true;
				}
				else {
					return;
				}
			}

			#endregion

			if(state == ConsoleState.Open) {
				#region initialize closing animation if user presses ` or ~

				if(IsKeyPressed(Keys.Escape)) {
					state = ConsoleState.Closing;
					stateStartTime = now;
					return;
				}

				#endregion

				//execute current line with the interpreter
				if(IsKeyPressed(Keys.Enter)) {
					if(inputBuffer.Length > 0) {
						history.Add(inputBuffer); //add command to history
					}
					WriteLine(inputBuffer);

					input(inputBuffer);

					inputBuffer = "";
					cursorPos = 0;
				}
				//erase previous letter when backspace is pressed
				if(KeyPressWithRepeat(Keys.Back, elapsedTime)) {
					if(cursorPos > 0) {
						inputBuffer = inputBuffer.Remove(cursorPos - 1, 1);
						cursorPos--;
					}
				}
				//delete next letter when delete is pressed
				if(KeyPressWithRepeat(Keys.Delete, elapsedTime)) {
					if(inputBuffer.Length != 0) {
						inputBuffer = inputBuffer.Remove(cursorPos, 1);
					}
				}
				//cycle backwards through the command history
				if(KeyPressWithRepeat(Keys.Up, elapsedTime)) {
					inputBuffer = history.Previous();
					cursorPos = inputBuffer.Length;
				}
				//cycle forwards through the command history
				if(KeyPressWithRepeat(Keys.Down, elapsedTime)) {
					inputBuffer = history.Next();
					cursorPos = inputBuffer.Length;
				}
				//move the cursor to the right
				if(KeyPressWithRepeat(Keys.Right, elapsedTime) && cursorPos != inputBuffer.Length) {
					cursorPos++;
				}
				//move the cursor left
				if(KeyPressWithRepeat(Keys.Left, elapsedTime) && cursorPos > 0) {
					cursorPos--;
				}
				//move the cursor to the beginning of the line
				if(IsKeyPressed(Keys.Home)) {
					cursorPos = 0;
				}
				//move the cursor to the end of the line
				if(IsKeyPressed(Keys.End)) {
					cursorPos = inputBuffer.Length;
				}
				//get a letter from input
				string nextChar = GetStringFromKeyState(elapsedTime);

				//only add it if it isn't null
				if(nextChar != "") {
					//if the cursor is at the end of the line, add the letter to the end
					if(inputBuffer.Length == cursorPos) {
						inputBuffer += nextChar;
					}
						//otherwise insert it where the cursor is
					else {
						inputBuffer = inputBuffer.Insert(cursorPos, nextChar);
					}
					cursorPos += nextChar.Length;
				}
			}
		}

		/// <summary>
		/// Renders a list of strings to the console
		/// </summary>
		/// <param name="output">String to render</param>
		/// <returns>List of strings</returns>
		public List<string> Render(string output) {
			List<string> lines = WrapLine(output, lineWidth);
			for(int i = 0; i < lines.Count; i++) {
				lines[i] = lines[i].Replace("\t", "    ");
			}
			lines.Reverse();
			return lines;
		}

		/// <summary>
		/// Used to draw the cursor
		/// </summary>
		/// <param name="now">Current time</param>
		/// <returns>Not Sure</returns>
		public string DrawCursor(double now) {
			int spaces = (inputBuffer.Length > 0 && cursorPos > 0)
			             	?
			             		Render(inputBuffer.Substring(0, cursorPos))[0].Length + cursorOffset
			             	:
			             		cursorOffset;
			return new String(' ', spaces) + (((int)(now / CursorBlinkTime) % 2 == 0) ? "_" : "");
		}

		/// <summary>
		/// Draws the console
		/// </summary>
		/// <param name="gameTime">gameTime</param>
		public override void Draw(GameTime gameTime) {
			game.GraphicsDevice.RasterizerState = new RasterizerState {FillMode = FillMode.Solid};
			//don't draw the console if it's closed
			if(state == ConsoleState.Closed) {
				Visible = false;
			}

			double now = gameTime.TotalGameTime.TotalSeconds;

			#region Console size & dimension management

			//get console dimensions
			consoleXSize = Game.Window.ClientBounds.Right - Game.Window.ClientBounds.Left - 20;
			consoleYSize = font.LineSpacing * LinesDisplayed + 20;

			//set the offsets 
			int consoleXOffset = 10;
			int consoleYOffset = 10;

			//run the opening animation
			if(state == ConsoleState.Opening) {
				int startPosition = 0 - consoleYOffset - consoleYSize;
				int endPosition = consoleYOffset;
				consoleYOffset =
					(int)MathHelper.Lerp(startPosition, endPosition, (float)(now - stateStartTime) / (float)AnimationTime);
			}
				//run the closing animation
			else if(state == ConsoleState.Closing) {
				int startPosition = consoleYOffset;
				int endPosition = 0 - consoleYOffset - consoleYSize;
				consoleYOffset =
					(int)MathHelper.Lerp(startPosition, endPosition, (float)(now - stateStartTime) / (float)AnimationTime);
			}
			//calculate the number of letters that fit on a line
			lineWidth = (int)(consoleXSize / font.MeasureString("a").X) - 2;
			//remeasure lineWidth, incase the screen size changes

			#endregion

		    spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend);

			#region Background Drawing

			spriteBatch.Draw(background, new Rectangle(consoleXOffset, consoleYOffset, consoleXSize, consoleYSize), Color.White);

			#endregion

			#region Text Drawing

			string cursorString = DrawCursor(now);

			spriteBatch.DrawString(font, cursorString,
			                       new Vector2(consoleXOffset + 10, consoleYOffset + consoleYSize - 10 - font.LineSpacing),
			                       Color.White);

			int j = 0;
			List<string> lines = Render(outputBuffer + inputBuffer);
			//show them in the proper order, because we're drawing from the bottom
			foreach(string str in lines) {
				//draw each line at an offset determined by the line height and line count
				j++;

				spriteBatch.DrawString(font, str,
				                       new Vector2(consoleXOffset + 10, consoleYOffset + consoleYSize - 10 - font.LineSpacing * (j)),
				                       Color.White);
			}

			#endregion

			spriteBatch.End();

			//reset depth buffer to normal status, so as not to mess up 3d code
			game.GraphicsDevice.DepthStencilState = DepthStencilState.Default;


		    game.GraphicsDevice.RasterizerState = new RasterizerState {FillMode = FillMode.WireFrame};  
		}

		#region keyboard status management

		//check if the key has just been pressed
		private bool IsKeyPressed(Keys key) {
			return currentKeyState.IsKeyDown(key) && !lastKeyState.IsKeyDown(key);
		}

		//check if a key is pressed, and repeat it at the default repeat rate
		private bool KeyPressWithRepeat(Keys key, double elapsedTime) {
			if(currentKeyState.IsKeyDown(key)) {
				if(IsKeyPressed(key)) {
					return true; //if the key has just been pressed, it automatically counts
				}
				keyTimes[key] -= elapsedTime; //count down to next repeat
				if(keyTimes[key] <= 0) //if the time has run out, repeat the letter
				{
					keyTimes[key] = repeatInterval; //reset the timer to the repeat interval
					return true;
				}
				return false;
			}
			//if the key is not pressed, reset it's time to the first interval, which is usually longer
			keyTimes[key] = firstInterval;
			return false;
		}

		/// <summary>
		/// Takes keyboard input and returns certain characters as a string
		/// </summary>
		/// <param name="elapsedTime"></param>
		/// <returns></returns>
		private string GetStringFromKeyState(double elapsedTime) {
			bool shiftPressed = currentKeyState.IsKeyDown(Keys.LeftShift) || currentKeyState.IsKeyDown(Keys.RightShift);
			bool altPressed = currentKeyState.IsKeyDown(Keys.LeftAlt) || currentKeyState.IsKeyDown(Keys.RightAlt);

			foreach(KeyBinding binding in KeyboardHelper.AmericanBindings) {
				if(KeyPressWithRepeat(binding.Key, elapsedTime)) {
					return !shiftPressed
					       	? (!altPressed ? binding.UnmodifiedString : binding.AltString)
					       	: (altPressed ? binding.ShiftAltString : binding.ShiftString);
				}
			}

			return "";
		}

		#endregion

		#region Nested type: ConsoleState

		private enum ConsoleState {
			Closed,
			Closing,
			Open,
			Opening
		}

		#endregion

		#region Nested type: History

		/// <summary>
		/// Object for storing command history
		/// </summary>
		public class History {
			private readonly List<string> history;
			private int index;

			/// <summary>
			/// Make a new history object with capacity maxLength
			/// </summary>
			public History() {
				history = new List<string>();
			}

			/// <summary>
			/// Returns the current command in history
			/// </summary>
			public string Current {
				get {
					if(index < history.Count) {
						return history[index];
					}
					else {
						return "";
					}
				}
			}

			/// <summary>
			/// Add a command to the history
			/// </summary>
			/// <param name="str"></param>
			public void Add(string str) {
				history.Add(str);
				index = history.Count;
			}

			/// <summary>
			/// Cycle backwards through commands in history
			/// </summary>
			/// <returns></returns>
			public string Previous() {
				if(index > 0) {
					index--;
				}
				return Current;
			}

			/// <summary>
			/// Cycle forwards through commands in history
			/// </summary>
			/// <returns></returns>
			public string Next() {
				if(index < history.Count - 1) {
					index++;
				}
				return Current;
			}

			/// <summary>
			/// Erase command history
			/// </summary>
			public void Clear() {
				history.Clear();
			}
		}

		#endregion
	}
}