<Query Kind="Statements">
  <Namespace>System.Windows.Forms</Namespace>
</Query>

var newGuid = Guid.NewGuid().ToString("N").ToUpperInvariant();
Clipboard.SetText(newGuid);
newGuid.Dump();