//////////////////////////////////////////////////////////////////////
// ARGUMENTS
//////////////////////////////////////////////////////////////////////

var target = Argument("target", "Default");
var configuration = Argument("configuration", "Release");
var configurations = new List<string>() { "Release Nuget 4.5" , "Release Nuget 4.6" };

//////////////////////////////////////////////////////////////////////
// PREPARATION
//////////////////////////////////////////////////////////////////////

// Define directories
var buildDirGeneGenieGedCom = Directory("GeneGenie.Gedcom/bin") + Directory(configuration);
var buildDirGeneGenieGedComParser = Directory("GeneGenie.Gedcom/bin") + Directory(configuration);
var buildDirGeneGenieGedComReports = Directory("GeneGenie.Gedcom/bin") + Directory(configuration);

//////////////////////////////////////////////////////////////////////
// TASKS
//////////////////////////////////////////////////////////////////////

Task("Clean")
    .Does(() =>
{
    CleanDirectory(buildDirGeneGenieGedCom);
	CleanDirectory(buildDirGeneGenieGedComParser);
	CleanDirectory(buildDirGeneGenieGedComReports);
});

Task("Restore-NuGet-Packages")
    .IsDependentOn("Clean")
    .Does(() =>
{
    NuGetRestore("GeneGenie.Gedcom.sln");
});

Task("Build")
    .IsDependentOn("Restore-NuGet-Packages")
    .Does(() =>
{
	foreach (var buildConfig in configurations) 
	{ 
		if(IsRunningOnWindows())
		{
		  // Use MSBuild
		  MSBuild("GeneGenie.Gedcom.sln", settings => settings.SetConfiguration(buildConfig));
		}
		else
		{
		  // Use XBuild
		  XBuild("GeneGenie.Gedcom.sln", settings => settings.SetConfiguration(buildConfig));
		}
	}
    
});

Task("Package")
    .IsDependentOn("Build")
    .Does(() =>
{
	Information("Creating nuget package...");
});

Task("Publish")
    .IsDependentOn("Package")
    .Does(() =>
{
	Information("Publishing nuget package...");
});


//////////////////////////////////////////////////////////////////////
// TASK TARGETS
//////////////////////////////////////////////////////////////////////

Task("Default")
    .IsDependentOn("Publish");

//////////////////////////////////////////////////////////////////////
// EXECUTION
//////////////////////////////////////////////////////////////////////

RunTarget(target);
