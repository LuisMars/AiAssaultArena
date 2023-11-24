using AiAssaultArena.SampleBot;

var tank = new Tank();
await tank.StartAsync();
Console.ReadLine();
tank.Dispose();