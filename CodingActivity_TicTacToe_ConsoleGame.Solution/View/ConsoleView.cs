using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;
using System.Text.RegularExpressions;
using System.Threading;

namespace CodingActivity_TicTacToe_ConsoleGame
{
    public class ConsoleView
    {
        #region ENUMS

        public enum ViewState
        {
            Active,
            PlayerTimedOut, // TODO Track player time on task
            PlayerUsedMaxAttempts
        }

        #endregion

        #region FIELDS

        private const int GAMEBOARD_VERTICAL_LOCATION = 4;

        private int POSITIONPROMPT_VERTICAL_LOCATION = 12;
        private int POSITIONPROMPT_HORIZONTAL_LOCATION = 3;

        private const int MESSAGEBOX_VERTICAL_MARGIN = 2;

        private const int DROP_PEICE_OFFSET = -2;

        private const int TOP_LEFT_ROW = 3;
        private const int TOP_LEFT_COLUMN = 6;

        private Gameboard _gameboard;
        private ViewState _currentViewStat;

        private static readonly string[] PLAYER_ICONS = { "", "\u2588\u2588", "\u2592\u2592" };

        #endregion

        #region PROPERTIES
        public ViewState CurrentViewState
        {
            get { return _currentViewStat; }
            set { _currentViewStat = value; }
        }

        #endregion

        #region CONSTRUCTORS

        public ConsoleView(Gameboard gameboard)
        {
            _gameboard = gameboard;

            InitializeView();

        }

        #endregion

        #region METHODS

        /// <summary>
        /// Initialize the console view
        /// </summary>
        public void InitializeView()
        {
            _currentViewStat = ViewState.Active;

            InitializeConsole();
        }

        /// <summary>
        /// configure the console window
        /// </summary>
        public void InitializeConsole()
        {
            ConsoleUtil.WindowWidth = ConsoleConfig.windowWidth;
            ConsoleUtil.WindowHeight = ConsoleConfig.windowHeight;

            Console.BackgroundColor = ConsoleConfig.bodyBackgroundColor;
            Console.ForegroundColor = ConsoleConfig.bodyBackgroundColor;

            ConsoleUtil.WindowTitle = "The Tic-tac-toe Game";
        }

        /// <summary>
        /// display the Continue prompt
        /// </summary>
        public void DisplayContinuePrompt()
        {
            Console.CursorVisible = false;

            Console.WriteLine();

            ConsoleUtil.DisplayMessage("Press any key to continue.");
            ConsoleKeyInfo response = Console.ReadKey();

            Console.WriteLine();

            Console.CursorVisible = true;
        }

        /// <summary>
        /// display the Exit prompt on a clean screen
        /// </summary>
        public void DisplayExitPrompt()
        {
            ConsoleUtil.DisplayReset();

            Console.CursorVisible = false;

            Console.WriteLine();
            ConsoleUtil.DisplayMessage("Thank you for play the game. Press any key to Exit.");

            Console.ReadKey();

            System.Environment.Exit(1);
        }

        /// <summary>
        /// display the session timed out screen
        /// </summary>
        public void DisplayTimedOutScreen()
        {
            ConsoleUtil.HeaderText = "Session Timed Out!";
            ConsoleUtil.DisplayReset();

            DisplayMessageBox("It appears your session has timed out.");

            DisplayContinuePrompt();
        }

        /// <summary>
        /// display the maximum attempts reached screen
        /// </summary>
        public void DisplayMaxAttemptsReachedScreen()
        {
            StringBuilder sb = new StringBuilder();

            ConsoleUtil.HeaderText = "Maximum Attempts Reached!";
            ConsoleUtil.DisplayReset();

            sb.Append(" It appears that you are having difficulty entering your");
            sb.Append(" choice. Please refer to the instructions and play again.");

            DisplayMessageBox(sb.ToString());

            DisplayContinuePrompt();
        }



        /// <summary>
        /// Inform the player that their position choice is not available
        /// </summary>
        public void DisplayGamePositionChoiceNotAvailableScreen()
        {
            StringBuilder sb = new StringBuilder();

            ConsoleUtil.HeaderText = "Position Choice Unavailable";
            ConsoleUtil.DisplayReset();

            sb.Append(" It appears that you have chosen a position that is all ready");
            sb.Append(" taken. Please try again.");

            DisplayMessageBox(sb.ToString());

            DisplayContinuePrompt();
        }

        /// <summary>
        /// display the welcome screen
        /// </summary>
        public void DisplayWelcomeScreen()
        {
            StringBuilder sb = new StringBuilder();

            ConsoleUtil.HeaderText = "Connect 4: The Movie The Game!";
            ConsoleUtil.DisplayReset();

            ConsoleUtil.DisplayMessage("Programmed by Noah Osterhout, Jason Luckhardt, and Chris Coznowski.");
            ConsoleUtil.DisplayMessage("Wolf Group Technolgies");
            Console.WriteLine();

            sb.Clear();
            sb.AppendFormat("Welcome to Connect 4: The Game! This is Connect 4 but in the console ");
            sb.AppendFormat("for Windows. The standard rules for Connect 4 apply, and this is just a two player ");
            sb.AppendFormat("game with each player taking a turn. Good luck and have fun, but remeber, if you find a bug ");
            sb.AppendFormat("it's most likely a feature! ");
            ConsoleUtil.DisplayMessage(sb.ToString());
            Console.WriteLine();

            sb.Clear();
            sb.AppendFormat("You will be redirecred to the main menu.");
            ConsoleUtil.DisplayMessage(sb.ToString());

            DisplayContinuePrompt();
        }

        /// <summary>
        /// display a closing screen when the user quits the application
        /// </summary>
        public void DisplayClosingScreen()
        {
            ConsoleUtil.HeaderText = "The Tic-tac-toe Game";
            ConsoleUtil.DisplayReset();

            ConsoleUtil.DisplayMessage("Thank you for using The Tic-tac-toe Game.");

            DisplayContinuePrompt();
        }

        public void DisplayGameArea()
        {
            ConsoleUtil.HeaderText = "Current Game Board";
            ConsoleUtil.DisplayReset();

            DisplayGameboard();
            DisplayGameStatus();
        }

        public void DisplayCurrentGameStatus(int roundsPlayed, int playerXWins, int playerOWins, int catsGames)
        {
            ConsoleUtil.HeaderText = "Current Game Status";
            ConsoleUtil.DisplayReset();

            double playerXPercentageWins = (double)playerXWins / roundsPlayed;
            double playerOPercentageWins = (double)playerOWins / roundsPlayed;
            double percentageOfCatsGames = (double)catsGames / roundsPlayed;

            ConsoleUtil.DisplayMessage("Rounds Played: " + roundsPlayed);
            ConsoleUtil.DisplayMessage("Rounds for Player X: " + playerXWins + " - " + String.Format("{0:P2}", playerXPercentageWins));
            ConsoleUtil.DisplayMessage("Rounds for Player O: " + playerOWins + " - " + String.Format("{0:P2}", playerOPercentageWins));
            ConsoleUtil.DisplayMessage("Cat's Games: " + catsGames + " - " + String.Format("{0:P2}", percentageOfCatsGames));

            DisplayContinuePrompt();
        }

        public bool DisplayNewRoundPrompt()
        {
            ConsoleUtil.HeaderText = "Continue or Quit";
            ConsoleUtil.DisplayReset();

            return DisplayGetYesNoPrompt("Would you like to play another round?");
        }

        public void DisplayGameStatus()
        {
            StringBuilder sb = new StringBuilder();

            switch (_gameboard.CurrentRoundState)
            {
                case Gameboard.GameboardState.NewRound:
                    //
                    // The new game status should not be an necessary option here
                    //
                    break;
                case Gameboard.GameboardState.PlayerXTurn:
                    DisplayMessageBox("It is currently Player X's turn.");
                    break;
                case Gameboard.GameboardState.PlayerOTurn:
                    DisplayMessageBox("It is currently Player O's turn.");
                    break;
                case Gameboard.GameboardState.PlayerXWin:
                    DisplayMessageBox("Player X Wins! Press any key to continue.");

                    Console.CursorVisible = false;
                    Console.ReadKey();
                    Console.CursorVisible = true;
                    break;
                case Gameboard.GameboardState.PlayerOWin:
                    DisplayMessageBox("Player O Wins! Press any key to continue.");

                    Console.CursorVisible = false;
                    Console.ReadKey();
                    Console.CursorVisible = true;
                    break;
                case Gameboard.GameboardState.CatsGame:
                    DisplayMessageBox("Cat's Game! Press any key to continue.");

                    Console.CursorVisible = false;
                    Console.ReadKey();
                    Console.CursorVisible = true;
                    break;
                default:
                    break;
            }
        }

        public void DisplayMessageBox(string message)
        {
            string leftMargin = new String(' ', ConsoleConfig.displayHorizontalMargin);
            string topBottom = new String('*', ConsoleConfig.windowWidth - 2 * ConsoleConfig.displayHorizontalMargin);

            StringBuilder sb = new StringBuilder();

            Console.SetCursorPosition(0, _gameboard.MaxNumOfRowsColumns * 2 + 1 + GAMEBOARD_VERTICAL_LOCATION + MESSAGEBOX_VERTICAL_MARGIN);
            Console.WriteLine(leftMargin + topBottom);

            Console.WriteLine(ConsoleUtil.Center("Game Status"));

            ConsoleUtil.DisplayMessage(message);

            Console.WriteLine(Environment.NewLine + leftMargin + topBottom);
        }

        /// <summary>
        /// Displays the gameboard based on _gameboard.MaxNumOfRowsColumns.
        /// </summary>
        private void DisplayGameboard()
        {
            Console.OutputEncoding = System.Text.Encoding.Unicode;

            char ulCorner = '\u2554';
            char llCorner = '\u255A';
            char urCorner = '\u2557';
            char lrCorner = '\u255D';
            char vertical = '\u2551';
            string horizontal = "\u2550\u2550";
            char cross = '\u256C';
            char crossBottom = '\u2569';
            char crossRight = '\u2560';
            char crossLeft = '\u2563';
            char crossTop = '\u2566';
            string space = "  ";
            string playerX = "\u2588\u2588";
            string playerO = "\u2592\u2592";


            int horizontal_offset = ConsoleUtil.WindowWidth / 2;
            int max = _gameboard.MaxNumOfRowsColumns * 2 + 1;
            int min = 0;

            for (int row = 0; row < max; row++)
            {
                Console.SetCursorPosition(horizontal_offset - max / 2 - _gameboard.MaxNumOfRowsColumns / 2, GAMEBOARD_VERTICAL_LOCATION + row);
                for (int column = 0; column < max; column++)
                {
                    if (!(row % 2 == 1 && column % 2 == 1)) // Not odd row and column. In other words not a player space.
                    {
                        if (row == min && column == min)
                            Console.Write(ulCorner);
                        else if (row == max - 1 && column == max - 1)
                            Console.Write(lrCorner);
                        else if (row == min && column == max - 1)
                            Console.Write(urCorner);
                        else if (row == max - 1 && column == min)
                            Console.Write(llCorner);
                        else if (row % 2 == 0 && column == min) // if row is even and column is 0. Draw a right spacer.
                            Console.Write(crossRight);
                        else if (row % 2 == 0 && column == max - 1) // if row is even and column is max -1. Draw a left spacer.
                            Console.Write(crossLeft);
                        else if (column % 2 == 0 && row == 0) // if column is even and row is 0. Draw a top spacer.
                            Console.Write(crossTop);
                        else if (column % 2 == 0 && row == max - 1) // if column is even and row is max -1. Draw a bottom spacer.
                            Console.Write(crossBottom);
                        else if (row == min || row == max - 1)
                            Console.Write(horizontal);
                        else if (column == min || column == max - 1)
                            Console.Write(vertical);
                        else if (row % 2 == 0 && column % 2 == 1) // even row and odd column. Draw a horizontal spacer.
                            Console.Write(horizontal);
                        else if (row % 2 == 1 && column % 2 == 0) // odd row and even column. Draw a vertical spacer.
                            Console.Write(vertical);
                        else if (row % 2 == 0 && column % 2 == 0) // even row and even column. Draw a cross spacer.
                            Console.Write(cross);
                    }
                    else switch(_gameboard._board[row / 2, column / 2].Status) // This is the first time I've ever used an else switch lol.
                    {
                        case PlayerPiece.NULL: // This might be breaking MVC but it saves me the work of flipping through the board twice.
                            _gameboard._board[row / 2, column / 2].Row = Console.CursorLeft;
                            _gameboard._board[row / 2, column / 2].Column = Console.CursorTop;
                            _gameboard._board[row / 2, column / 2].Status = PlayerPiece.None;
                            Console.Write(space);
                            break;
                        case PlayerPiece.X:
                            Console.Write(playerX);
                            break;
                        case PlayerPiece.O:
                            Console.Write(playerO);
                            break;
                        case PlayerPiece.None:
                            Console.Write(space);
                            break;
                    }
                }
                Console.WriteLine();
            }
        }

        /// <summary>
        /// display prompt for a player's next move
        /// </summary>
        /// <param name="coordinateType"></param>
        private void DisplayPositionPrompt(string coordinateType)
        {
            POSITIONPROMPT_VERTICAL_LOCATION = _gameboard.MaxNumOfRowsColumns * 2 + 1 + GAMEBOARD_VERTICAL_LOCATION;

            //
            // Clear line by overwriting with spaces
            //
            Console.SetCursorPosition(POSITIONPROMPT_HORIZONTAL_LOCATION, POSITIONPROMPT_VERTICAL_LOCATION);
            Console.Write(new String(' ', ConsoleConfig.windowWidth));
            //
            // Write new prompt
            //
            Console.SetCursorPosition(POSITIONPROMPT_HORIZONTAL_LOCATION, POSITIONPROMPT_VERTICAL_LOCATION);
            Console.Write("Enter " + coordinateType + " number: ");
        }

        /// <summary>
        /// Display a Yes or No prompt with a message
        /// </summary>
        /// <param name="promptMessage">prompt message</param>
        /// <returns>bool where true = yes</returns>
        private bool DisplayGetYesNoPrompt(string promptMessage)
        {
            bool yesNoChoice = false;
            bool validResponse = false;
            string userResponse;

            while (!validResponse)
            {
                ConsoleUtil.DisplayReset();

                ConsoleUtil.DisplayPromptMessage(promptMessage + "\r\n (yes/no): ");
                userResponse = Console.ReadLine();

                if (Regex.IsMatch(userResponse.ToLower(), "^y*$|^yes$"))
                {
                    validResponse = true;
                    yesNoChoice = true;
                }
                else if (Regex.IsMatch(userResponse.ToLower(), "^n*$|^no$"))
                {
                    validResponse = true;
                    yesNoChoice = false;
                }
                else
                {
                    ConsoleUtil.DisplayMessage(
                        "It appears that you have entered an incorrect response." +
                        " Please enter either \"yes\" or \"no\"."
                        );
                    DisplayContinuePrompt();
                }
            }

            return yesNoChoice;
        }

        /// <summary>
        /// Get a player's position choice within the correct range of the array
        /// Note: The ConsoleView is allowed access to the GameboardPosition struct.
        /// </summary>
        /// <returns>GameboardPosition</returns>
        public int GetPlayerPositionChoice()
        {
            Console.CursorVisible = false;
            Console.OutputEncoding = System.Text.Encoding.Unicode;
            ConsoleKeyInfo keyInfo;
            int player_column = 0;
            do
            {
                Console.SetCursorPosition(_gameboard._board[0, player_column].Row, _gameboard._board[0, player_column].Column + DROP_PEICE_OFFSET);
                Console.Write(PLAYER_ICONS[(int)_gameboard.CurrentRoundState]);
                keyInfo = Console.ReadKey();
                //Console.SetCursorPosition(Console.CursorLeft -3, Console.CursorTop);
                Console.SetCursorPosition(_gameboard._board[0, player_column].Row, _gameboard._board[0, player_column].Column + DROP_PEICE_OFFSET);
                Console.Write("  ");
                switch (keyInfo.Key)
                {
                    case ConsoleKey.LeftArrow:
                        if (player_column > 0)
                            player_column--;
                        break;
                    case ConsoleKey.RightArrow:
                        if (player_column < _gameboard.MaxNumOfRowsColumns -1)
                            player_column++;
                        break;
                    case ConsoleKey.F1:
                        break;
                    case ConsoleKey.F2:
                        break;
                    case ConsoleKey.F3:
                        break;
                    case ConsoleKey.F4:
                        break;
                    default:
                        break;
                }
            } while (keyInfo.Key != ConsoleKey.Enter);

            /*
            //
            // Initialize gameboardPosition with -1 values
            //
            GameboardPosition gameboardPosition = new GameboardPosition(-1, -1);

            //
            // Get row number from player.
            //
            gameboardPosition.Row = PlayerCoordinateChoice("Row");

            //
            // Get column number.
            //
            if (CurrentViewState != ViewState.PlayerUsedMaxAttempts)
            {
                gameboardPosition.Column = PlayerCoordinateChoice("Column");
            }*/

            return player_column;

        }

        public void DisplayPieceDrop(int endRow, int column)
        {
            int row;
            for (row = 0; row <= endRow; row++)
            {
                Console.SetCursorPosition(_gameboard._board[row, column].Row, _gameboard._board[row, column].Column);
                Console.Write(PLAYER_ICONS[(int)_gameboard.CurrentRoundState]);

                Thread.Sleep(100);

                //Console.SetCursorPosition(Console.CursorLeft -3, Console.CursorTop);
                Console.SetCursorPosition(_gameboard._board[row, column].Row, _gameboard._board[row, column].Column);
                Console.Write("  ");
            }
            Console.SetCursorPosition(_gameboard._board[row -1, column].Row, _gameboard._board[row -1, column].Column);
            Console.Write(PLAYER_ICONS[(int)_gameboard.CurrentRoundState]);
        }

        /// <summary>
        /// Validate the player's coordinate response for integer and range
        /// </summary>
        /// <param name="coordinateType">an integer value within proper range or -1</param>
        /// <returns></returns>
        private int PlayerCoordinateChoice(string coordinateType)
        {
            int tempCoordinate = -1;
            int numOfPlayerAttempts = 1;
            int maxNumOfPlayerAttempts = 4;

            while ((numOfPlayerAttempts <= maxNumOfPlayerAttempts))
            {
                DisplayPositionPrompt(coordinateType);

                if (int.TryParse(Console.ReadLine(), out tempCoordinate))
                {
                    //
                    // Player response within range
                    //
                    if (tempCoordinate >= 1 && tempCoordinate <= _gameboard.MaxNumOfRowsColumns)
                    {
                        return tempCoordinate;
                    }
                    //
                    // Player response out of range
                    //
                    else
                    {
                        DisplayMessageBox(coordinateType + " numbers are limited to (1,2,3)");
                    }
                }
                //
                // Player response cannot be parsed as integer
                //
                else
                {
                    DisplayMessageBox(coordinateType + " numbers are limited to (1,2,3)");
                }

                //
                // Increment the number of player attempts
                //
                numOfPlayerAttempts++;
            }

            //
            // Player used maximum number of attempts, set view state and return
            //
            CurrentViewState = ViewState.PlayerUsedMaxAttempts;
            return tempCoordinate;
        }

        #endregion
    }
}
