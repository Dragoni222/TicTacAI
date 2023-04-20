﻿
using System.Diagnostics;
using System.Drawing;
using System.Numerics;
using RatMaze;
using RatMaze.Games;

int[,] map =
{
    {1,1,1,1,1}, //y = 0
    {1,2,0,0,1},
    {1,0,3,0,1},
    {1,0,0,0,1},
    {1,1,1,1,1}  //y = 4
};




bool end = false;

while (end == false)
{
    LoadFrame();
    string answer = Console.ReadLine();
    int answerInt = 1;
    if (answer == null)
    {

    }
    else
    {
        try
        {
            answerInt = int.Parse(answer);
        }
        catch(FormatException e)
        {
            answerInt = 1;
        }

        if (answer == "matrixtest")
        {
            LoadMatrixTest();
        }
        else if (answer == "aitest")
        {
            LoadAiTest();
        }
        else if (answer == "tic")
        {
            LoadAiTicTacToe();
        }
    }
        


    for (int i = 0; i < answerInt; i++)
    {
        
    }
    
}


Coordinant playerPos = GetPlayerPos();

void Move(int direction) //n=1 e=2, s=3, w=4
{
    if (CanMoveInDirection(direction))
    {
        map[playerPos.Y, playerPos.X] = 0;
        Coordinant targetPos = DirectionToCoordinate(direction, playerPos);
        map[targetPos.Y, targetPos.X] = 3;

    }
}

void LoadMap()
{
    for (int y = 0; y < map.GetLength(0); y++)
    {
        for (int x = 0; x < map.GetLength(1); x++)
        {
            Console.Write(map[x,y] + " ");
        }
        Console.WriteLine();
    }
}

void LoadFrame()
{
    Console.Clear();
    LoadMap();
    LoadQuestion();
}

void LoadMatrixTest()
{
    AIDimension testMatrix = new AIDimension(1);

    Console.Write("Cube side length:");
    ulong sideLength = ulong.Parse(Console.ReadLine());
    Console.Write("\n dimensions:");
    ulong dimensions = ulong.Parse(Console.ReadLine());
    for (ulong i = 0; i < dimensions ; i++)
    {
        Stopwatch timer = new Stopwatch();
        timer.Start();
        if (i == 0)
        {
            Weight[] sideValues = new Weight[sideLength];
            for (ulong j = 0; j < sideLength; j++)
            {
                sideValues[j].GiveDopamine(j, sideValues);
            }
            testMatrix = new AIDimension(sideValues);
        }
        else
        {
        
            testMatrix = new AIDimension(sideLength, testMatrix, true);
        }
        timer.Stop();
        Console.WriteLine("Dimension: " + i + " completed. Time:" + timer.Elapsed);

    }

    Console.WriteLine($"Completed. Get a value: {dimensions} parameters needed, max value of {sideLength - 1}");
    int[] address = new int[dimensions];
    for (ulong i = 0; i < dimensions; i++)
    {
        for (ulong j = 0; j < i; j++)
        {
            Console.Write(address[j] + ", ");
        }
        address[i] = int.Parse(Console.ReadLine());
    }

    Stopwatch stopwatch = new Stopwatch();
    stopwatch.Start();
    Console.WriteLine("Value: " + AIDimension.GetSpecificWeight(testMatrix, address));
    stopwatch.Stop();
    Console.WriteLine("Time elapsed getting value: " + stopwatch.Elapsed);
    Console.ReadLine();
}

void LoadAiTest()//loads a test for learning addition.
{
    Console.Clear();
    AI testAI = new AI(200, new List<Input>() {new Input(100, 1), new Input(100,1)});
    Console.Write("Seed: ");
    Random random = new Random(int.Parse(Console.ReadLine()));
    Console.Write("\n Cycles: ");
    int cycles = int.Parse(Console.ReadLine());
    Console.Write("\n Train Speed: ");
    double trainSpeed = double.Parse(Console.ReadLine());
    Stopwatch stopwatch = new Stopwatch();
    stopwatch.Start();
    for (int i = 0; i < cycles; i++)
    {
        if (i % 1000000 == 0)
        {
            Console.WriteLine("Cycle: " + i);
        }

        testAI.Inputs[0].SetValue((ulong)random.Next(0,100));
        testAI.Inputs[1].SetValue((ulong)random.Next(0,100));
        int expectedValue = (int)testAI.Inputs[0].GetValue() + (int)testAI.Inputs[1].GetValue();
        AI.TrainByDifferenceKnown(testAI, expectedValue, random, trainSpeed);
    }

    stopwatch.Stop();
    Console.WriteLine($"Completed. Time: {stopwatch.Elapsed}.     Test? (Y/N)");
    string input = Console.ReadLine();
    if (input == "y" || input == "Y")
    {
        bool end = false;
        while (!end)
        {
            Console.Write("\nTest input 1: (0-100): ");
            testAI.Inputs[0].SetValue(ulong.Parse(Console.ReadLine()));
            Console.Write("\nTest input 2: (0-100): ");
            testAI.Inputs[1].SetValue(ulong.Parse(Console.ReadLine()));
            Console.WriteLine(AI.GetBestActionKnown(testAI.AiMatrix, testAI.Inputs ).Output + "  End? (Y/N)");
            input = Console.ReadLine();
            if (input == "Y" || input == "y")
            {
                end = true;
            }
        }
       
        
    }

}

void LoadAiTicTacToe()
{
    Console.Clear();

    Console.Write("Seed: ");
    Random random = new Random(int.Parse(Console.ReadLine()));
    Console.Write("\n Cycles: ");
    int cycles = int.Parse(Console.ReadLine());
    Console.Write("\n Train Speed: ");
    double trainSpeed = double.Parse(Console.ReadLine());
    Stopwatch stopwatch = new Stopwatch();
    TicTacToe game = new TicTacToe(trainSpeed, 0);
    stopwatch.Start();
    for (int i = 0; i < cycles; i++)
    {
        if (i % 1000000 == 0)
        {
            Console.WriteLine("Cycle: " + i);
        }

        game.TrainAi(random);
        game.Reset();
    }

    stopwatch.Stop();
    Console.WriteLine($"Completed. Time: {stopwatch.Elapsed}.     Test? (Y/N)");
    string input = Console.ReadLine();
    if (input == "y" || input == "Y")
    {
        bool endGame = false;
        while (!endGame)
        {
            Console.WriteLine("Would you like to play first or second? (1/2)");
            TicTacToe gameVsPlayer = new TicTacToe(game.Ai, 0, int.Parse(Console.ReadLine()));
            gameVsPlayer.PlayerVsAi();
            Console.WriteLine("Play again? (y/n)");
            input = Console.ReadLine();
            if (input != "Y" && input != "y")
            {
                endGame = true;
            }
        }

    }
}

void LoadLogicAiTest()
{
    Console.Clear();
    Console.WriteLine("Seed: ");
    string input = Console.ReadLine();
    Random random = new Random(int.Parse(input));
    Console.WriteLine("Training BaseTransforms. Num of training cycles per transform:");
    input = Console.ReadLine();
    int cycles = int.Parse(input);
    List<Transform> baseTransforms = new List<Transform>();
    Console.WriteLine("Maximum number in inputs?");
    input = Console.ReadLine();
    int maximum = int.Parse(input);
    baseTransforms.Add(new BaseTransform(new Addition(maximum)));
    baseTransforms.Add(new BaseTransform(new Subtraction(maximum)));

    Stopwatch timer = new Stopwatch();
    timer.Start();
    for (int i = 0; i < cycles; i++)
    {
        if (i % 1000000 == 0)
        {
            Console.WriteLine("Cycle: " + i + "    Time: "+ timer.Elapsed);
        }
        baseTransforms[0].TrainTransform(random);
        baseTransforms[1].TrainTransform(random);
    }
    timer.Stop();
    Console.WriteLine("Completed basic transform training.");
    Console.WriteLine("Cycles for Logic Training? ");
    input = Console.ReadLine();
    cycles = int.Parse(input);
    List<Input> aiInputs = new List<Input>();
    aiInputs.Add(new Input((ulong)maximum, 0)); // first number
    aiInputs.Add(new Input(2, 0)); //operator
    aiInputs.Add(new Input((ulong)maximum, 0));//second number
    aiInputs.Add(new Input((ulong)maximum * 2, 0));//output
    LogicAI testAi = new LogicAI(aiInputs);
    testAi.AddTransform(baseTransforms[0]);
    testAi.AddTransform(baseTransforms[1]);
    testAi.AddTransform(baseTransforms[2]);
    List<Input> desiredInputs = new List<Input>();
    foreach (var aiInput in aiInputs)
    {
        desiredInputs.Add(new Input(aiInput.MaxValue, 0));
    }


    for (int i = 0; i < cycles; i++)
    {
        if (i % 1000000 == 0)
        {
            Console.WriteLine("Cycle: " + i + "    Time: "+ timer.Elapsed);
        }
        aiInputs[0].SetValue((ulong)random.Next(0,maximum));
        aiInputs[1].SetValue((ulong)random.Next(0,1));
        if (aiInputs[1].GetValue() == 0)
            aiInputs[2].SetValue((ulong)random.Next(0, maximum));
        else
        {
            aiInputs[2].SetValue((ulong)random.Next(0, (int)aiInputs[0].GetValue()));
        }
        aiInputs[3].SetValue(0);
        for (int j = 0; j < aiInputs.Count - 1; j++)
        {
            desiredInputs[j].SetValue((ulong)aiInputs[j].GetValue());
        }

        if (aiInputs[1].GetValue() == 0)
        {
            desiredInputs[4].SetValue((ulong)(aiInputs[0].GetValue() + aiInputs[2].GetValue()) );
        }
        else
        {
            desiredInputs[4].SetValue((ulong)(aiInputs[0].GetValue() - aiInputs[2].GetValue()) );
        }
        
        testAi.Train(desiredInputs, random, 1);
    }

}

void LoadQuestion()
{
    Console.WriteLine("/n Frames to skip? (enter if none)");
}

bool CanMoveInDirection(int direction)
{
    Coordinant targetPos = DirectionToCoordinate(direction, playerPos);
    
    return CanMoveIntoTile(map[targetPos.Y, targetPos.X ]);

}

bool CanMoveIntoTile(int tile)
{
    if (tile == 0 || tile == 2)
        return true;
    else
        return false;
}

Coordinant GetPlayerPos()
{
    for (int y = 0; y < map.GetLength(0); y++)
    {
        for (int x = 0; x < map.GetLength(1); x++)
        {
            if (map[x,y] == 3)
            {
                return new Coordinant(x, y);
            }
        }
        
    }

    return new Coordinant(-1, -1);
}

Coordinant DirectionToCoordinate(int direction, Coordinant position)
{
    if (direction == 1)
    {
        return new Coordinant(position.X, position.Y - 1);

    }
    else if (direction == 2)
    {
        return new Coordinant(position.X + 1, position.Y);


    }
    else if (direction == 3)
    {
        return new Coordinant(position.X, position.Y + 1);


    }
    else if (direction == 4)
    {
        return new Coordinant(position.X - 1, position.Y);


    }

    return new Coordinant(0, 0);
}

struct Coordinant 
{
    public Coordinant(int x, int y)
    {
        X = x;
        Y = y;
    }

    public int X { get; }
    public int Y { get; }
}



































