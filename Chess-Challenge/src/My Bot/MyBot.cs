using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Runtime.CompilerServices;
using ChessChallenge.API;

public class ResultMove
{
    public Move NextMove { get; set; }
    public float Score { get; set; }
    public Board LastBoard { get; set; }
    public int Level { get; set; }
    public ResultMove ParentMove { get; set; }
}

public class MyBot : IChessBot
{
    private const int maxLevel = 4;
    private List<ResultMove> _results = new();
    private Random _rnd = new(0);
    private ResultMove rootMove;

    public Move Think(Board board, Timer timer)
    {
        _results.Clear();

        rootMove = new ResultMove
        {
            NextMove = new Move(),
            Score = 0,
            LastBoard = board,
            Level = -1
        };
        GetBestMove(board, timer, rootMove);



        var level = maxLevel;
        ResultMove? bestResult;

        bestResult = GetBestResultMove(level, true);

        while (bestResult == null)
        {
            level -= 2;
            bestResult = GetBestResultMove(level, true);
        }

        while (level > 0)
        {
            Console.Write($"{bestResult.NextMove} ");
            bestResult = bestResult.ParentMove;
            level--;
        }

        Console.WriteLine($"{bestResult.NextMove} ");

        return bestResult.NextMove;


    }

    private ResultMove? GetBestResultMove(int level, bool isMax)
    {
        if (_results.All(r => r.Level != level))
            return null;

        float bestScore;

        if (isMax)
        {
            bestScore = (from r in _results
                where r.Level == level
                select r.Score).Max();
        }
        else
        {
            bestScore = (from r in _results
                where r.Level == level
                select r.Score).Min();
        }

        //Console.Write($"{bestScore} ");

        var bestMoves = (from r in _results
            where r.Level == level && r.Score == bestScore
            select r).ToList();

        var bestMoveIndex = _rnd.Next(bestMoves.Count);
        var bestResult = bestMoves[bestMoveIndex];
        return bestResult;
    }

    private void GetBestMove(Board board, Timer timer, ResultMove parentMove, int level = 0)
    {
        var moves = board.GetLegalMoves();
        var boardString = board.GetFenString();

        if (level <= maxLevel)
        {
            foreach (var move in moves)
            {

                var testBoard = Board.CreateBoardFromFEN(boardString);
                testBoard.MakeMove(move);
                var score = ScoreBoard(testBoard);

                //Console.Write($"{score} ");

                var testMove = new ResultMove
                {
                    NextMove = move,
                    Score = score,
                    LastBoard = testBoard,
                    Level = level,
                    ParentMove = parentMove
                };
                _results.Add(testMove);


                if (level % 2 == 0)
                    GetBestMove(testBoard, timer, testMove, level + 1);
            }

            if (level % 2 == 1)
            {
                var bestResult = GetBestResultMove(level, false);
                if(bestResult != null)
                    GetBestMove(bestResult.LastBoard, timer, bestResult, level + 1);
            }
        }
    }

    private float ScoreBoard(Board board)
    {
        var pieces = board.WhitePiecesBitboard;
        var score = ScorePieces(board, pieces);

        pieces = board.BlackPiecesBitboard;
        score -= ScorePieces(board, pieces);

        if (board.IsWhiteToMove)
            score = -score;

        if(board.IsInCheckmate())
            score += 1000;

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