using Framework;

namespace Challenges2022;

public class Solution7 : SolutionFramework
{
    public Solution7() : base(7) { }

    struct File
    {
        public string Name;
        public long Size;
    }

    struct Dir
    {
        public Guid guid;
        public List<Dir> Dirs;
        public string Name;
        public int Level;
        public List<File> Files;
        public long Size()
        {
            if (!Dirs.Any())
            {
                return Files.Sum(f => f.Size);
            }

            long size = 0;
            foreach (var child in Dirs)
            {
                size += child.Size();
            }

            return size + Files.Sum(f => f.Size);
        }
    }

    public override string[] Solve()
    {
        Dir rootDirectory = new() { Level = 0, Files = new List<File>(), Name = "/", Dirs = new List<Dir>(), guid = Guid.NewGuid() };
        var fileSystem = new List<Dir>
        {
            rootDirectory
        };

        Dir currentDir = fileSystem[0];

        foreach (var line in RawInput.SplitByNewline())
        {
            var name = line.Split(' ').Last();

            switch (line)
            {
                case {} when IsLs(line):
                    continue;
                case {} when IsCd(line):
                    if (line.EndsWith(".."))
                        currentDir = fileSystem.Single(x => x.Dirs.Any(y => y.guid == currentDir.guid));
                    else
                        currentDir = name == rootDirectory.Name ? fileSystem[0] : currentDir.Dirs.Single(d => d.Name == name);
                    break;
                case {} when IsDirectory(line):
                    var dir = new Dir { Name = name, Level = currentDir.Level+1, Files = new List<File>(), Dirs = new List<Dir>(), guid = Guid.NewGuid()};
                    currentDir.Dirs.Add(dir);
                    fileSystem.Add(dir);
                    break;
                case {} when IsFile(line):
                    var size = long.Parse(line.Split(' ').First());
                    var f = new File() { Name = name, Size = size };
                    currentDir.Files.Add(f);
                    break;
            }
        }

        AssignAnswer1(fileSystem.Where(x => x.Size() <= 100000).Select(x => x.Size()).Sum());

        var totalDiskSize = 70000000;
        var unusedSpaceWish = 30000000;
        var fs = fileSystem.OrderBy(x => x.Size()).ToList();
        var csize = fileSystem[0].Size();
        var unused = totalDiskSize - csize;

        foreach (var d in fs)
        {
            if (unused + d.Size() >= unusedSpaceWish)
            {
                AssignAnswer2(d.Size());
                break;
            }
        }

        return Answers;
    }

    private static bool IsCd(string line)
    {
        return line.StartsWith("$ cd");
    }

    private static bool IsLs(string line)
    {
        return line.StartsWith("$ ls");
    }

    private static bool IsDirectory(string line)
    {
        return line.StartsWith("dir");
    }

    private static bool IsFile(string line)
    {
        return Char.IsDigit(line.First());
    }
}
