using Raylib_cs;

Raylib.InitWindow(Constants.FieldWidth, Constants.FieldHeight, "Pong");
Raylib.SetTargetFPS(60);

var game = new Game();
game.Start(Raylib.GetTime());

while (!Raylib.WindowShouldClose())
{
    double now = Raylib.GetTime();
    float dt = Raylib.GetFrameTime();

    // --- Input ---
    game.Left.Direction = 0;
    if (Raylib.IsKeyDown(KeyboardKey.W)) game.Left.Direction = -1;
    if (Raylib.IsKeyDown(KeyboardKey.S)) game.Left.Direction = 1;

    game.Right.Direction = 0;
    if (Raylib.IsKeyDown(KeyboardKey.Up)) game.Right.Direction = -1;
    if (Raylib.IsKeyDown(KeyboardKey.Down)) game.Right.Direction = 1;

    bool anyKey = Raylib.GetKeyPressed() != 0;

    // --- Update ---
    game.Update(now, dt);

    if (game.CanRestartOnKeyPress(now) && anyKey)
        game.Restart(now);

    // --- Draw ---
    Raylib.BeginDrawing();
    Raylib.ClearBackground(Color.Black);

    // Center divider
    for (int y = 0; y < Constants.FieldHeight; y += 20)
        Raylib.DrawRectangle(Constants.FieldWidth / 2 - 1, y, 2, 10, Color.DarkGray);

    // Paddles
    DrawPaddle(Constants.PaddleLeftX, game.Left.Y);
    DrawPaddle(Constants.PaddleRightX, game.Right.Y);

    // Ball (hidden during Waiting and GameOver)
    if (game.State == GameState.Playing || game.State == GameState.PointScored)
        Raylib.DrawRectangle(
            (int)game.Ball.X - Constants.BallHalf,
            (int)game.Ball.Y - Constants.BallHalf,
            Constants.BallSize, Constants.BallSize, Color.White);

    // Scores
    Raylib.DrawText(game.ScoreLeft.ToString(), Constants.FieldWidth / 2 - 60, 20, 40, Color.White);
    Raylib.DrawText(game.ScoreRight.ToString(), Constants.FieldWidth / 2 + 30, 20, 40, Color.White);

    // State overlays
    switch (game.State)
    {
        case GameState.Waiting:
            DrawCentered("READY", Constants.FieldHeight / 2 - 20, 40, Color.Yellow);
            break;
        case GameState.PointScored:
            string scorer = game.ScoreLeft > game.ScoreRight ? "PLAYER 1 SCORES!" : "PLAYER 2 SCORES!";
            DrawCentered(scorer, Constants.FieldHeight / 2 - 20, 40, Color.Yellow);
            break;
        case GameState.GameOver:
            string winner = game.ScoreLeft >= Constants.WinningScore
                ? "GAME OVER - PLAYER 1 WON"
                : "GAME OVER - PLAYER 2 WON";
            DrawCentered(winner, Constants.FieldHeight / 2 - 20, 30, Color.Yellow);
            if (game.CanRestartOnKeyPress(now))
                DrawCentered("Press any key to play again", Constants.FieldHeight / 2 + 30, 20, Color.Gray);
            break;
    }

    Raylib.EndDrawing();
}

Raylib.CloseWindow();

void DrawPaddle(int cx, float cy)
{
    Raylib.DrawRectangle(
        cx - Constants.PaddleWidth / 2,
        (int)cy - Constants.PaddleHalfHeight,
        Constants.PaddleWidth, Constants.PaddleHeight, Color.White);
}

void DrawCentered(string text, int y, int size, Color color)
{
    int w = Raylib.MeasureText(text, size);
    Raylib.DrawText(text, (Constants.FieldWidth - w) / 2, y, size, color);
}
