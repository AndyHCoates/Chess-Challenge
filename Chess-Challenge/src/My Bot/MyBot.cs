using ChessChallenge.API;

const int maxLevel = 0;

public class MyBot : IChessBot
{
    public Move Think(Board board, Timer timer)
    {
        Move[] moves = board.GetLegalMoves();
        return moves[0];
        //return GetBestMove(board, timer);
    }

    private Move GetBestMove(Board board, Timer timer, int level = 0)
    {
        Move[] moves = board.GetLegalMoves();
        var boardString = board.GetFenString();

        foreach (var move in moves)
        {
            var testBoard = Board.CreateBoardFromFEN(boardString);

            testBoard.MakeMove(move);
            var score = ScoreBoard(testBoard);
        }
        return moves[0];
    }

    private float ScoreBoard(Board board)
    {
        var score = 0.0f;


        return score;
    }
}