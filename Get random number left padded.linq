<Query Kind="Statements" />

var dice = new Random();
Enumerable
	.Range(0, 10)
	.Select(_ => dice.Next(0, 10))
	.Aggregate(
		new StringBuilder(),
		(state, item) => state.Append(item),
		state => state.ToString())
	.Dump();
