using System;
using System.Collections.Generic;
using ChessChallenge.API;
using ChessChallenge.Chess;
using Board = ChessChallenge.API.Board;
using Move = ChessChallenge.API.Move;

public class MyBot : IChessBot
{
    const int maxLevel = 0;

    public Move Think(Board board, Timer timer)
    {
        return GetBestMove(board, timer);
    }

    private Move GetBestMove(Board board, Timer timer, int level = 0)
    {
        Move[] moves = board.GetLegalMoves();
        var boardString = board.GetFenString();
        Move bestMove = new Move();
        var bestScore = -10000f;


        foreach (var move in moves)
        {
            if (level < maxLevel)
            {
                var testBoard = Board.CreateBoardFromFEN(boardString);
                testBoard.MakeMove(move);
                bestMove = GetBestMove(testBoard, timer, level + 1);
            }
            else
            {
                var testBoard = Board.CreateBoardFromFEN(boardString);
                testBoard.MakeMove(move);
                var score = ScoreBoard(testBoard);
                Console.Write($"{score} ");
                if (score > bestScore)
                {
                    bestScore = score;
                    bestMove = move;
                }
            }
        }
        Console.WriteLine($"Best Score: {bestScore}");
        return bestMove;
    }

    private float ScoreBoard(Board board)
    {
        var score = 0.0f;
        var pieces = board.WhitePiecesBitboard;
        score = ScorePieces(board, pieces);

        pieces = board.BlackPiecesBitboard;
        score -= ScorePieces(board, pieces);

        if (board.IsWhiteToMove)
            score = -score;


        return score;
    }

    private static float ScorePieces(Board board, ulong pieces)
    {
        var score = 0f;

        for (var i = 0; i < 64; i++)
        {
            if ((pieces & 1) == 1)
            {
                var piece = board.GetPiece(new Square(i % 8, i / 8));

                switch (piece.PieceType)
                {
                    case PieceType.Pawn:
                        score += 1;
                        break;
                    case PieceType.Knight:
                        score += 3;
                        break;
                    case PieceType.Bishop:
                        score += 3;
                        break;
                    case PieceType.Rook:
                        score += 5;
                        break;
                    case PieceType.Queen:
                        score += 9;
                        break;
                    case PieceType.King:
                        score += 0;
                        break;
                    case PieceType.None:
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }

            pieces >>= 1;
        }

        return score;
    }
}