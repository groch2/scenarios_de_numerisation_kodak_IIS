<Query Kind="Statements" />

const int exponentOf10 = 6;
var powOf10 = (int)Math.Pow(10, exponentOf10);
new Random().Next(powOf10).ToString().PadLeft(exponentOf10, '0').Dump();