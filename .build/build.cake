///////////////////////////////////////////////////////////////////////////////
// MACSTACK BUILD (MACOS ARM64 ONLY)
///////////////////////////////////////////////////////////////////////////////

var target = Argument("target", "build");

var srcProj = File("../src/mac/mac.csproj");
var outDir  = Directory("../.bin");

Task("clean")
  .Does(() =>
{
  CleanDirectory(outDir);
});

Task("build")
  .IsDependentOn("clean")
  .Does(() =>
{
  DotNetPublish(srcProj.FullPath, new DotNetPublishSettings
  {
    Configuration = "Release",
    Runtime = "osx-arm64",
    OutputDirectory = outDir,
    NoRestore = false
  });

  StartProcess("chmod", new ProcessSettings {
    Arguments = $"+x \"{outDir}/mac\""
  });
});

RunTarget(target);