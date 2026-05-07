using Raylib_cs;

Raylib.InitWindow(800, 600, "Pong");
Raylib.SetTargetFPS(60);

while (!Raylib.WindowShouldClose())
{
    Raylib.BeginDrawing();
    Raylib.ClearBackground(Color.Black);
    Raylib.EndDrawing();
}

Raylib.CloseWindow();
