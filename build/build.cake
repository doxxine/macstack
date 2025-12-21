///////////////////////////////////////////////////////////////////////////////
// MACSTACK BUILD (MACOS ARM64 ONLY)
///////////////////////////////////////////////////////////////////////////////

var target = Argument("target", "build");
var packageVersion = Argument("packageVersion", "0.1.0");

var rid = "darwin-arm64";
var runtimeRid = "osx-arm64";

// SCRIPT LIVES IN build/, SO PATHS ARE ../...
var srcProj  = "./../src/mac/mac.csproj";
var binDir   = "./../.bin";
var distDir  = "./../dist";
var stageDir = "./../dist/stage";

Task("clean")
  .Does(() =>
{
  EnsureDirectoryExists(binDir);
  EnsureDirectoryExists(distDir);

  CleanDirectory(binDir);
  CleanDirectory(distDir);
});

Task("build")
  .IsDependentOn("clean")
  .Does(() =>
{
  DotNetPublish(srcProj, new DotNetPublishSettings
  {
    Configuration = "Release",
    Runtime = runtimeRid,
    OutputDirectory = binDir
  });

  StartProcess("chmod", new ProcessSettings
  {
    Arguments = $"+x \"{binDir}/mac\""
  });
});

Task("release")
  .IsDependentOn("build")
  .Does(() =>
{
  EnsureDirectoryExists(distDir);
  EnsureDirectoryExists(stageDir);
  CleanDirectory(stageDir);

  // STAGE MINIMUM RUNTIME PAYLOAD
  CopyFile($"{binDir}/mac", $"{stageDir}/mac");
  CopyDirectory("./../plugins", $"{stageDir}/plugins");
  CopyDirectory("./../lib", $"{stageDir}/lib");

  // OPTIONAL TEMPLATE CONFIG
  var hasConfig = FileExists("./../macstack.conf");
  if (hasConfig)
    CopyFile("./../macstack.conf", $"{stageDir}/macstack.conf");

  // CREATE TARBALL WITHOUT WRAPPER FOLDER
  var tgz = $"{distDir}/macstack-{packageVersion}-{rid}.tar.gz";
  var files = hasConfig ? "mac plugins lib macstack.conf" : "mac plugins lib";

  var tarExit = StartProcess("tar", new ProcessSettings
  {
    Arguments = $"-czf \"{tgz}\" -C \"{stageDir}\" {files}"
  });

  if (tarExit != 0)
    throw new Exception($"tar failed with exit code {tarExit}");

  // SHA256 (CAPTURE STDOUT VIA OUT PARAM)
  IEnumerable<string> shaOut;
  var shaExit = StartProcess("shasum", new ProcessSettings
  {
    Arguments = $"-a 256 \"{tgz}\"",
    RedirectStandardOutput = true
  }, out shaOut);

  if (shaExit != 0)
    throw new Exception($"shasum failed with exit code {shaExit}");

  System.IO.File.WriteAllText($"{tgz}.sha256", string.Join("\n", shaOut).TrimEnd() + "\n");

  Information("RELEASE ASSET: {0}", tgz);
  Information("SHA256 FILE  : {0}", $"{tgz}.sha256");
});

Task("default")
  .IsDependentOn("build");

RunTarget(target);
