using FSyncTest;

var dirName = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds().ToString();

var dirPath = Path.Combine(Environment.CurrentDirectory, dirName);

Directory.CreateDirectory(dirPath);

Console.WriteLine($"Created directory: {dirPath}");

using var fs = new FileStream(Path.Combine(dirPath, "foo.txt"), FileMode.Create, FileAccess.Write);

fs.Write("Hello, world!"u8);
fs.Flush(false);

FSyncWrapper.FSyncDir(dirPath);

Console.WriteLine("fsync completed");
