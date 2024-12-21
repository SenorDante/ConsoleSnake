using System.IO;
using System.Threading;
internal class Program
{
    static void Main(string[] args)
    {
        bool isGameOver = false;
        bool collectedFood = false;
        char[,] map = ReadMap("map.txt");
        Console.CursorVisible = false;
        Random random = new Random();
        int snakeX = 25;
        int snakeY = 11;
        int foodX = random.Next(1, 49);
        int foodY = random.Next(1, 22);
        List<int[]> snakeBodyDirection = new List<int[]>();
        int score = 0;
        int[] direction = new int[2];

        Console.ForegroundColor = ConsoleColor.DarkGreen;
        DrawMap(map);

        Console.SetCursorPosition(8, 11);
        Console.ForegroundColor = ConsoleColor.DarkBlue;
        Console.Write("Press any key to start the game");

        Console.SetCursorPosition(0, 24);
        Console.Write("Controls: <, ^, >, v");

        ConsoleKeyInfo pressedKey = Console.ReadKey();
        ConsoleKeyInfo tempPressedKey;

        Task.Run(() =>
        {
            while (true)
            {
                tempPressedKey = Console.ReadKey();
                if (tempPressedKey.Key == ConsoleKey.UpArrow ||
                    tempPressedKey.Key == ConsoleKey.DownArrow ||
                    tempPressedKey.Key == ConsoleKey.LeftArrow ||
                    tempPressedKey.Key == ConsoleKey.RightArrow)
                {
                    pressedKey = tempPressedKey;
                }
            }
        });

        while (!isGameOver)
        {
            Console.Clear();

            HandleInput(pressedKey, ref snakeX, ref snakeY, map, ref isGameOver, ref direction);
            GenerateFood(ref foodX, ref foodY, snakeX, snakeY, ref score, collectedFood, random, map, ref snakeBodyDirection, ref direction);

            Console.ForegroundColor = ConsoleColor.DarkGreen;

            Console.SetCursorPosition(0, 0);
            DrawMap(map);

            Console.ForegroundColor = ConsoleColor.DarkYellow;
            Console.SetCursorPosition(snakeX, snakeY);
            Console.Write("*");

            MovingBody(ref snakeBodyDirection, ref direction, ref snakeX, ref snakeY, ref isGameOver);

            Console.ForegroundColor = ConsoleColor.DarkMagenta;
            Console.SetCursorPosition(foodX, foodY);
            Console.Write("O");

            Console.SetCursorPosition(51, 0);
            Console.Write($"Score: {score}");

            if (direction[0] == 0)
            {
                Thread.Sleep(200);
            }
            else if (direction[1] == 0) { Thread.Sleep(100); }

            if (isGameOver)
            {
                Console.ForegroundColor = ConsoleColor.DarkRed;
                Console.SetCursorPosition(7, 11);
                Console.Write("Game Over. Press Enter to start again");
                ConsoleKeyInfo exit = Console.ReadKey();
                if (exit.Key == ConsoleKey.Enter)
                {
                    isGameOver = false;
                    snakeBodyDirection.Clear();
                    snakeX = 25;
                    snakeY = 11;
                    score = 0;
                    foodX = random.Next(1, 49);
                    foodY = random.Next(1, 22);
                }
                else
                {
                    Environment.Exit(0);
                }
            }
        }
    }

    private static char[,] ReadMap(string path)
    {
        string[] file = File.ReadAllLines(path);
        char[,] map = new char[50, 23];
        for (int x = 0; x < 50; x++)
            for (int y = 0; y < 23; y++)
                map[x, y] = file[y][x];
        return map;
    }

    private static void DrawMap(char[,] map)
    {
        for (int y = 0; y < 23; y++)
        {
            for (int x = 0; x < 50; x++)
            {
                Console.Write(map[x, y]);
            }
            Console.WriteLine();
        }
    }

    private static void HandleInput(ConsoleKeyInfo pressedKey, ref int snakeX, ref int snakeY, char[,] map, ref bool gameOver, ref int[] direction)
    {
        if (pressedKey.Key == ConsoleKey.RightArrow && direction[0] == 0
            || pressedKey.Key == ConsoleKey.LeftArrow && direction[0] == 0
            || pressedKey.Key == ConsoleKey.DownArrow && direction[1] == 0
            || pressedKey.Key == ConsoleKey.UpArrow && direction[1] == 0)
        { direction = GetDirection(pressedKey); }
        int nextSnakePositionX = snakeX + direction[0];
        int nextSnakePositionY = snakeY + direction[1];
        if (map[nextSnakePositionX, nextSnakePositionY] == ' ')
        {
            snakeX = nextSnakePositionX;
            snakeY = nextSnakePositionY;
        }
        else { gameOver = true; }
    }

    private static int[] GetDirection(ConsoleKeyInfo pressedKey)
    {
        int[] direction = { 0, 0 };
        if (pressedKey.Key == ConsoleKey.UpArrow)
        {
            direction[1] = -1;
        }
        else if (pressedKey.Key == ConsoleKey.DownArrow)
        {
            direction[1] = 1;
        }
        else if (pressedKey.Key == ConsoleKey.LeftArrow)
        {
            direction[0] = -1;
        }
        else if (pressedKey.Key == ConsoleKey.RightArrow)
        {
            direction[0] = 1;
        }
        return direction;
    }

    private static void GenerateFood(ref int foodX, ref int foodY, int snakeX, int snakeY, ref int score, bool collectedFood, Random random, char[,] map, ref List<int[]> snakeBodyDirection, ref int[] direction)
    {
        if (foodX == snakeX && foodY == snakeY)
        {
            collectedFood = true;
            score++;

            snakeBodyDirection.Add(direction);
        }
        if (collectedFood)
        {
            foodX = random.Next(1, 49);
            foodY = random.Next(1, 22);
            if (map[foodX, foodY] == ' ')
            {
                collectedFood = false;
            }
        }
    }

    private static void MovingBody(ref List<int[]> snakeBodyDirection, ref int[] direction, ref int snakeX, ref int snakeY, ref bool isGameOver)
    {
        int positionX = snakeX, positionY = snakeY;
        for (int i = snakeBodyDirection.Count - 1; i >= 0; i--)
        {
            if (i == 0)
            {
                snakeBodyDirection[i] = direction;
            }
            else
            {
                snakeBodyDirection[i] = snakeBodyDirection[i - 1];
            }
        }
        for (int i = 0; i < snakeBodyDirection.Count; i++)
        {
            if (i == 0)
            {
                positionX -= snakeBodyDirection[i][0];
                positionY -= snakeBodyDirection[i][1];
                Console.SetCursorPosition(positionX, positionY);
                Console.Write("·");
            }
            else
            {
                positionX -= snakeBodyDirection[i][0];
                positionY -= snakeBodyDirection[i][1];
                Console.SetCursorPosition(positionX, positionY);
                Console.Write("·");
            }
            if (snakeX == positionX && snakeY == positionY)
            {
                isGameOver = true;
                break;
            }
        }
    }
}