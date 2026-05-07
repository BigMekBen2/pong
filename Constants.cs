static class Constants
{
    public const int FieldWidth = 800;
    public const int FieldHeight = 600;

    public const int PaddleWidth = 10;
    public const int PaddleHeight = 80;
    public const int PaddleHalfHeight = PaddleHeight / 2;
    public const int PaddleLeftX = 20;
    public const int PaddleRightX = FieldWidth - PaddleLeftX;

    public const int BallSize = 10;
    public const int BallHalf = BallSize / 2;
    public const int BallInitialX = FieldWidth / 2;
    public const int BallInitialY = FieldHeight / 2;

    public const float PaddleSpeed = 360f;
    public const float BallSpeedInitial = 300f;
    public const float BallSpeedIncrement = 0.15f;
    public const float BallSpeedCap = BallSpeedInitial * 2f;

    public const int HitsPerSpeedup = 4;
    public const float MaxReturnAngleDeg = 75f;
    public const float ServeAngleRangeDeg = 30f;

    public const int WinningScore = 11;

    public const float WaitingDuration = 3f;
    public const float PointScoredDuration = 2f;
    public const float GameOverSoundTimeout = 3f;
}
