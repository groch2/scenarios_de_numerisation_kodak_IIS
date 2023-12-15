<Query Kind="Statements" />

const string séparateur = @"C:\Users\deschaseauxr\Documents\Kodak Info Input Solution\GECO1\exemple de lot de contrats numérisés avec séparateurs\GECO1_SEPPLI.pdf";
const string sourceDirectory = @"C:\Users\deschaseauxr\Documents\Kodak Info Input Solution\GECO1\exemple de lot de contrats numérisés avec séparateurs\extracted-pages";
var outputDirectory = Path.Combine(Directory.GetParent(sourceDirectory).ToString(), "output");
if (Directory.Exists(outputDirectory)) {
	Directory.Delete(path: outputDirectory, recursive: true);
}
Directory.CreateDirectory(path: outputDirectory);
var files =
	Directory
		.GetFiles(sourceDirectory)
		.Select((file, index) => new { index, file })
		.ToList();
files
	.ForEach(item => {
		var fileName = Path.GetFileName(item.file);
		fileName = Regex.Replace(
			input: fileName,
			pattern: @"^",
			replacement: $"{(item.index * 2 + 2):00}-");
		var file = Path.Combine(outputDirectory, fileName);
		File.Copy(sourceFileName: item.file, destFileName: file, overwrite: true);
	});
var séparateurFileName =
	Regex.Replace(
		input: Path.GetFileName(séparateur),
		pattern: @"^\d-",
		replacement: "");
var nbSéparateurs = files.Count();
Enumerable
	.Range(0, nbSéparateurs)
	.ToList()
	.ForEach(index => {
		var séparateurNum = index * 2 + 1;
		var séparateurFilePath = Path.Combine(outputDirectory, $"{séparateurNum:00}-{séparateurFileName}");
		File.Copy(sourceFileName: séparateur, destFileName: séparateurFilePath, overwrite: true);
	});