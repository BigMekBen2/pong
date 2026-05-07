class Game
{
    public Ball Ball = new();
    public Paddle Left = new();
    public Paddle Right = new();
    public int ScoreLeft;
    public int ScoreRight;
    public string ScoreLeftStr = "0";
    public string ScoreRightStr = "0";
    public GameState State = GameState.Waiting;

    // Sound flags — cleared each frame after being read
    public bool SoundPaddleHit;
    public bool SoundScore;
    public bool SoundGameOver;

    // Who just lost the last point (serves toward them); null = random at game start
    private bool? _serveToLeft;
    private double _stateEnteredAt;
    private bool _gameOverSoundDone;

    public void Start(double now)
    {
        _serveToLeft = null;
        ResetBall(toLeft: Random.Shared.NextSingle() < 0.5f);
        EnterState(GameState.Waiting, now);
    }

    public void Update(double now, float dt)
    {
        ClearSoundFlags();

        if (State != GameState.GameOver)
            MovePaddles(dt);

        switch (State)
        {
            case GameState.Waiting:
                if (now - _stateEnteredAt >= Constants.WaitingDuration)
                    EnterState(GameState.Playing, now);
                break;

            case GameState.Playing:
                MoveBall(dt);
                ResolveWalls();
                ResolvePaddles();
                CheckScoring(now);
                break;

            case GameState.PointScored:
                if (now - _stateEnteredAt >= Constants.PointScoredDuration)
                {
                    if (ScoreLeft >= Constants.WinningScore || ScoreRight >= Constants.WinningScore)
                    {
                        SoundGameOver = true;
                        _gameOverSoundDone = false;
                        EnterState(GameState.GameOver, now);
                    }
                    else
                    {
                        EnterState(GameState.Waiting, now);
                    }
                }
                break;

            case GameState.GameOver:
                if (!_gameOverSoundDone &&
                    now - _stateEnteredAt >= Constants.GameOverSoundTimeout)
                    _gameOverSoundDone = true;
                break;
        }
    }

    // Called by Program when a key is pressed during GameOver (after sound window)
    public bool CanRestartOnKeyPress(double now) =>
        State == GameState.GameOver && _gameOverSoundDone;

    public void Restart(double now)
    {
        ScoreLeft = 0; ScoreLeftStr = "0";
        ScoreRight = 0; ScoreRightStr = "0";
        _serveToLeft = null;
        ResetBall(toLeft: Random.Shared.NextSingle() < 0.5f);
        EnterState(GameState.Waiting, now);
    }

    // Called by Program to notify sound engine cleared the game-over flag
    public void NotifyGameOverSoundDone() => _gameOverSoundDone = true;

    // ── private ──────────────────────────────────────────────────────────────

    void ClearSoundFlags()
    {
        SoundPaddleHit = false;
        SoundScore = false;
        SoundGameOver = false;
    }

    void EnterState(GameState s, double now)
    {
        State = s;
        _stateEnteredAt = now;
    }

    void MovePaddles(float dt)
    {
        MoveP(Left, dt);
        MoveP(Right, dt);

        static void MoveP(Paddle p, float dt)
        {
            p.Y += Constants.PaddleSpeed * p.Direction * dt;
            p.Y = Math.Clamp(p.Y, Constants.PaddleHalfHeight, Constants.FieldHeight - Constants.PaddleHalfHeight);
        }
    }

    void MoveBall(float dt) { Ball.X += Ball.Vx * dt; Ball.Y += Ball.Vy * dt; }

    void ResolveWalls()
    {
        float top = Constants.BallHalf;
        float bot = Constants.FieldHeight - Constants.BallHalf;
        if (Ball.Y < top) { Ball.Vy = MathF.Abs(Ball.Vy); Ball.Y = top; }
        else if (Ball.Y > bot) { Ball.Vy = -MathF.Abs(Ball.Vy); Ball.Y = bot; }
    }

    void ResolvePaddles()
    {
        // Left paddle: ball moving left, leading edge at left paddle x-zone
        if (Ball.Vx < 0)
        {
            float leftEdge = Ball.X - Constants.BallHalf;
            if (leftEdge <= Constants.PaddleLeftX + Constants.PaddleWidth / 2f &&
                Ball.X > Constants.PaddleLeftX - Constants.PaddleWidth &&
                MathF.Abs(Ball.Y - Left.Y) <= Constants.PaddleHalfHeight)
            {
                Bounce(Left, toRight: true);
                return;
            }
        }

        // Right paddle: ball moving right
        if (Ball.Vx > 0)
        {
            float rightEdge = Ball.X + Constants.BallHalf;
            if (rightEdge >= Constants.PaddleRightX - Constants.PaddleWidth / 2f &&
                Ball.X < Constants.PaddleRightX + Constants.PaddleWidth &&
                MathF.Abs(Ball.Y - Right.Y) <= Constants.PaddleHalfHeight)
            {
                Bounce(Right, toRight: false);
            }
        }
    }

    void Bounce(Paddle paddle, bool toRight)
    {
        float offset = Ball.Y - paddle.Y;
        float relative = Math.Clamp(offset / Constants.PaddleHalfHeight, -1f, 1f);
        float angleDeg = relative * Constants.MaxReturnAngleDeg;
        float angleRad = angleDeg * MathF.PI / 180f;

        float speed = Ball.Speed;
        Ball.Vx = (toRight ? 1 : -1) * speed * MathF.Cos(angleRad);
        Ball.Vy = speed * MathF.Sin(angleRad);

        // Push ball outside paddle
        if (toRight)
            Ball.X = Constants.PaddleLeftX + Constants.PaddleWidth / 2f + Constants.BallHalf + 1;
        else
            Ball.X = Constants.PaddleRightX - Constants.PaddleWidth / 2f - Constants.BallHalf - 1;

        Ball.HitCount++;
        if (Ball.HitCount % Constants.HitsPerSpeedup == 0)
        {
            float newSpeed = speed * (1f + Constants.BallSpeedIncrement);
            if (newSpeed > Constants.BallSpeedCap) newSpeed = Constants.BallSpeedCap;
            float scale = newSpeed / speed;
            Ball.Vx *= scale;
            Ball.Vy *= scale;
        }

        SoundPaddleHit = true;
    }

    void CheckScoring(double now)
    {
        if (Ball.X < 0)
        {
            ScoreRight++; ScoreRightStr = ScoreRight.ToString();
            OnPoint(toLeft: true, now);
        }
        else if (Ball.X > Constants.FieldWidth)
        {
            ScoreLeft++; ScoreLeftStr = ScoreLeft.ToString();
            OnPoint(toLeft: false, now);
        }
    }

    void OnPoint(bool toLeft, double now)
    {
        _serveToLeft = toLeft;
        ResetBall(toLeft);
        SoundScore = true;
        EnterState(GameState.PointScored, now);
    }

    void ResetBall(bool toLeft)
    {
        Ball.X = Constants.BallInitialX;
        Ball.Y = Constants.BallInitialY;
        Ball.HitCount = 0;

        float angleRad = (Random.Shared.NextSingle() * 2 - 1) *
                         Constants.ServeAngleRangeDeg * MathF.PI / 180f;
        float dir = toLeft ? -1f : 1f;
        Ball.Vx = dir * Constants.BallSpeedInitial * MathF.Cos(angleRad);
        Ball.Vy = Constants.BallSpeedInitial * MathF.Sin(angleRad);
    }
}
