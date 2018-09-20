using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;

//using TicTacToe.ConsoleApp.Model;

namespace CodingActivity_TicTacToe_ConsoleGame
{
    public enum PlayerPiece
    {
        NULL,
        X,
        O,
        None
    }

    public class Gameboard
    {
        #region ENUMS

        public enum GameboardState
        {
            NewRound,
            PlayerXTurn,
            PlayerOTurn,
            PlayerXWin,
            PlayerOWin,
            CatsGame
        }

        #endregion

        #region FIELDS

        private const int MAX_NUM_OF_ROWS_COLUMNS = 7;

        private PlayerPiece[,] _positionState;

        private GameboardState _currentRoundState;

        public GameboardPosition[,] _board = new GameboardPosition[MAX_NUM_OF_ROWS_COLUMNS, MAX_NUM_OF_ROWS_COLUMNS];

        #endregion

        #region PROPERTIES

        public int MaxNumOfRowsColumns
        {
            get { return MAX_NUM_OF_ROWS_COLUMNS; }
        }

        public PlayerPiece[,] PositionState
        {
            get { return _positionState; }
            set { _positionState = value; }
        }

        public GameboardState CurrentRoundState
        {
            get { return _currentRoundState; }
            set { _currentRoundState = value; }
        }
        #endregion

        #region CONSTRUCTORS

        public Gameboard()
        {
            _positionState = new PlayerPiece[MAX_NUM_OF_ROWS_COLUMNS, MAX_NUM_OF_ROWS_COLUMNS];

            InitializeGameboard();
        }

        #endregion

        #region METHODS

        /// <summary>
        /// fill the game board array with "None" enum values
        /// </summary>
        public void InitializeGameboard()
        {
            _currentRoundState = GameboardState.NewRound;

            //
            // Set all PlayerPiece array values to "None"
            //
            for (int row = 0; row < MAX_NUM_OF_ROWS_COLUMNS; row++)
            {
                for (int column = 0; column < MAX_NUM_OF_ROWS_COLUMNS; column++)
                {
                    _positionState[row, column] = PlayerPiece.None;
                }
            }
        }


        /// <summary>
        /// Determine if the game board position is taken
        /// </summary>
        /// <param name="gameboardColumn"></param>
        /// <returns>-1 if there is no space in that column otherwise it will return the row number.</returns>
        public int GameboardPositionAvailable(int gameboardColumn)
        {
            //
            // Confirm that the board position is empty
            // Note: gameboardPosition converted to array index by subtracting 1
            //

            for (int row = MaxNumOfRowsColumns - 1; row >= 0; row--)
            {
                if (_board[row, gameboardColumn].Status == PlayerPiece.None)
                {
                    return row;
                }
            }

            return -1;
        }

        /// <summary>
        /// Update the game board state if a player wins or a cat's game happens.
        /// </summary>
        public void UpdateGameboardState()
        {
            if (ThreeInARow(PlayerPiece.X))
            {
                _currentRoundState = GameboardState.PlayerXWin;
            }

            //A player O has won
            
            else if (ThreeInARow(PlayerPiece.O))
            {
                _currentRoundState = GameboardState.PlayerOWin;
            }

            //All positions filled
            
            else if (IsCatsGame())
            {
                _currentRoundState = GameboardState.CatsGame;
            }
        }

        public bool IsCatsGame()
        {
            //
            // All positions on board are filled and no winner
            //
            for (int row = 0; row < 3; row++)
            {
                for (int column = 0; column < 3; column++)
                {
                    if (_positionState[row, column] == PlayerPiece.None)
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        /// <summary>
        /// Check for any four in a row.
        /// </summary>
        /// <param name="playerPieceToCheck">Player's game piece to check</param>
        /// <returns>true if a player has won</returns>
        /// 

        //private bool FourInARow(PlayerPiece playerPieceToCheck)
        //{

        //}


        private bool ThreeInARow(PlayerPiece playerPieceToCheck)
        {
            //
            // Check rows for player win
            //

            for (int row = 0; row < 3; row++)
            {
                if (_positionState[row, 0] == playerPieceToCheck &&
                    _positionState[row, 1] == playerPieceToCheck &&
                    _positionState[row, 2] == playerPieceToCheck)
                {
                    return true;
                }
            }

            //
            // Check columns for player win
            //
            for (int column = 0; column < 3; column++)
            {
                if (_positionState[0, column] == playerPieceToCheck &&
                    _positionState[1, column] == playerPieceToCheck &&
                    _positionState[2, column] == playerPieceToCheck)
                {
                    return true;
                }
            }

            //
            // Check diagonals for player win
            //
            if (
                (_positionState[0, 0] == playerPieceToCheck &&
                _positionState[1, 1] == playerPieceToCheck &&
                _positionState[2, 2] == playerPieceToCheck)
                ||
                (_positionState[0, 2] == playerPieceToCheck &&
                _positionState[1, 1] == playerPieceToCheck &&
                _positionState[2, 0] == playerPieceToCheck)
                )
            {
                return true;
            }

            //
            // No Player Has Won
            //

            return false;
        }

        /// <summary>
        /// Switch the game board state to the next player.
        /// </summary>
        public void SetNextPlayer()
        {
            if (_currentRoundState == GameboardState.PlayerXTurn)
            {
                _currentRoundState = GameboardState.PlayerOTurn;
            }
            else
            {
                _currentRoundState = GameboardState.PlayerXTurn;
            }
        }

        #endregion
    }
}

