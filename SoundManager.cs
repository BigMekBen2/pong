using Raylib_cs;

class SoundManager
{
    readonly Sound _paddleHit;
    readonly Sound _score;
    readonly Sound _gameOver;

    public SoundManager()
    {
        _paddleHit = MakeBeep(480f, 0.05f);
        _score     = MakeBeep(300f, 0.25f);
        _gameOver  = MakeBeep(180f, 0.6f);
    }

    public void PlayPaddleHit() => Raylib.PlaySound(_paddleHit);
    public void PlayScore()     => Raylib.PlaySound(_score);
    public void PlayGameOver()  => Raylib.PlaySound(_gameOver);

    public void Unload()
    {
        Raylib.UnloadSound(_paddleHit);
        Raylib.UnloadSound(_score);
        Raylib.UnloadSound(_gameOver);
    }

    static unsafe Sound MakeBeep(float frequency, float duration, float volume = 0.4f)
    {
        const uint sampleRate = 44100;
        uint frames = (uint)(sampleRate * duration);
        float fadeFrames = sampleRate * 0.01f; // 10 ms fade in/out to avoid clicks

        var wave = new Wave
        {
            SampleCount = frames,
            SampleRate = sampleRate,
            SampleSize = 32,
            Channels = 1,
            Data = Raylib.MemAlloc(frames * sizeof(float))
        };

        float* data = (float*)wave.Data;
        for (uint i = 0; i < frames; i++)
        {
            float envelope = Math.Clamp(Math.Min(i / fadeFrames, (frames - i) / fadeFrames), 0f, 1f);
            data[i] = volume * envelope * MathF.Sin(2f * MathF.PI * frequency * i / sampleRate);
        }

        Sound sound = Raylib.LoadSoundFromWave(wave);
        Raylib.UnloadWave(wave);
        return sound;
    }
}
