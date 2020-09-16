using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Serilog;

namespace NexusDeployment.Actions
{
    public class TransferModules : Alexr03.Common.Deployment.Actions.DeploymentAction
    {
        public override void Execute()
        {
            var copyFromDirectory = new DirectoryInfo(Program.ArgumentOptions.FromLocation);
            var binaries = copyFromDirectory.GetDirectories("*bin*", SearchOption.AllDirectories);
            var tasks = new List<Task>();

            foreach (var directoryInfo in binaries)
                tasks.Add(Task.Run(() => CopyFilesRecursively(directoryInfo,
                    new DirectoryInfo(Path.Combine(Program.ArgumentOptions.BaseLocation, "Nexus.Program", "bin",
                        "Debug", "Shared")))));

            Task.WaitAll(tasks.ToArray());
        }

        public static void CopyFilesRecursively(DirectoryInfo source, DirectoryInfo target)
        {
            var moduleName = source.Parent?.Parent?.Name + ".dll";
            var possibleModule = new FileInfo(Path.Combine(source.FullName, moduleName));
            if (possibleModule.Exists)
            {
                var destFileName = Path.Combine(target.Parent?.Parent?.FullName ?? "", "Modules", possibleModule.Name);
                Log.Error("Copying module to " + destFileName);
                possibleModule.CopyTo(destFileName, true);
            }

            var tasks = source.GetDirectories()
                .Select(dir => Task.Run(() => CopyFilesRecursively(dir, target.CreateSubdirectory(dir.Name)))).ToList();

            foreach (var file in source.GetFiles())
            {
                if (!file.Extension.EndsWith("dll")) continue;

                try
                {
                    
                    var fileInfo = new FileInfo(Path.GetFullPath(Path.Combine(target.FullName, "..\\", file.Name)));
                    if (!fileInfo.Exists)
                    {
                        tasks.Add(Task.Run(() =>
                        {
                            file.CopyTo(fileInfo.FullName, true);
                            Log.Information(
                                $"[{fileInfo.Directory?.Parent?.Name}/{fileInfo.Directory?.Name}] Deployed {file.Name} for the first time.");
                        }));
                        continue;
                    }

                    var compare = DateTime.Compare(fileInfo.LastWriteTimeUtc, file.LastWriteTimeUtc);
                    if (compare == -1)
                        tasks.Add(Task.Run(() =>
                        {
                            file.CopyTo(fileInfo.FullName, true);
                            Log.Information(
                                $"[{fileInfo.Directory?.Parent?.Name}/{fileInfo.Directory?.Name}] {file.Name} is newer than deployed. Redeployed...");
                        }));
                }
                catch (Exception e)
                {
                    Log.Error($"Failed to copy file {file.FullName} | {e.Message}");
                }
            }

            Task.WaitAll(tasks.ToArray());
        }
    }
}