class Ball
{
    public float X = Constants.BallInitialX;
    public float Y = Constants.BallInitialY;
    public float Vx;
    public float Vy;
    public int HitCount;

    public float Speed => MathF.Sqrt(Vx * Vx + Vy * Vy);
}
