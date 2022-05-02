using CsdlComparer.Comparer;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace CsdlComparer
{
    class Program
    {
        static void Main(string[] args)
        {
            var dir = @"C:\temp\schema_samples";

            // for each file in before, check if there is a matching file, and compare
            var beforeFiles = Directory.EnumerateFiles($@"{dir}\before").Select(x => x.Replace($@"{dir}\before\", ""));
            List<IElementComparisionResult> result = new List<IElementComparisionResult>();
            foreach (var file in beforeFiles)
            {
                if (File.Exists($@"{dir}\after\{file}"))
                {
                    Console.WriteLine($"Comparing - {file}");
                    result.AddRange(new CsdlComparer(new SchemaElementComparerFactory()).Compare($@"{dir}\before\{file}", $@"{dir}\after\{file}"));
                }
                else
                {
                    Console.WriteLine($"File - {file} not found in destination");
                }
            }
            foreach(var newFile in Directory.EnumerateFiles($@"{dir}\after").Select(x => x.Replace($@"{dir}\after\", "")).Where(afterFile => !beforeFiles.Contains(afterFile)))
            {
                Console.WriteLine($"Detected new file - {newFile}");
            }
            foreach (var r in result)
            {
                Console.WriteLine(r.ToString());
            }
        }
    }
}
