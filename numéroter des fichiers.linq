<Query Kind="Statements" />

const string séparateur = @"C:\Users\deschaseauxr\Documents\Kodak Info Input Solution\GECO2\GECO2-SEPPLI-V1.pdf";
const string folder = @"C:\Users\deschaseauxr\Documents\Kodak Info Input Solution\GECO2\liasse de documents séparés";
Directory
	.GetFiles(folder)
	.Select((file, index) => new { index, file })
	.ToList()
	.ForEach(item => {
		var fileName = Path.GetFileName(item.file);
		fileName = Regex.Replace(
			input: fileName,
			pattern: @"^",
			replacement: $"{(item.index * 2 + 1):00}-");
		var directory = Path.GetDirectoryName(item.file);
		var file = Path.Combine(directory, fileName);
		File.Move(sourceFileName: item.file, destFileName: file, overwrite: true);
	});
var séparateurFileName =
	Regex.Replace(
		input: Path.GetFileName(séparateur),
		pattern: @"^\d-",
		replacement: "");
var nbSéparateurs = files.Count() - 1;
Enumerable
	.Range(0, nbSéparateurs)
	.ToList()
	.ForEach(index => {
		var séparateurNum = index * 2 + 2;
		var séparateurFilePath = Path.Combine(folder, $"{séparateurNum:00}-{séparateurFileName}");
		File.Copy(sourceFileName: séparateur, destFileName: séparateurFilePath, overwrite: true);
	});