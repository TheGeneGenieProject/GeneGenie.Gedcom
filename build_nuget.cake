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
var projGedCom ="GeneGenie.Gedcom";
var projParser = "GeneGenie.Gedcom.Parser";
var projReports = "GeneGenie.Gedcom.Reports";

//////////////////////////////////////////////////////////////////////
// TASKS
//////////////////////////////////////////////////////////////////////

Task("Clean")
    .Does(() =>
{
    CleanDirectory(Directory(projGedCom + "/bin/" + configuration));
	CleanDirectory(Directory(projParser + "/bin/" + configuration));
	CleanDirectory(Directory(projReports + "/bin/" + configuration));
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
	
	//GeneGenie.Gedcom
	Information("Creating nuget package " + projGedCom);

	var assemblyInfo = ParseAssemblyInfo("./" + projGedCom + "/AssemblyInfo.cs");
	var settings = new NuGetPackSettings {
                                Id = projGedCom,
                                Version = assemblyInfo.AssemblyVersion,
                                Title = assemblyInfo.Title,
								Authors = new[] { "GeneGenie" },
								Copyright = assemblyInfo.Copyright,
								Description	= assemblyInfo.Description,
								BasePath = Directory(projGedCom),
								OutputDirectory = projGedCom + "/NuGet/PackageSource/"
	};

	
	NuGetPack(projGedCom + "/NuGet/GeneGenie.Gedcom.nuspec", settings); 

	//GeneGenie.Gedcom.Parser

	Information("Creating nuget package " + projParser);
	assemblyInfo = ParseAssemblyInfo("./" + projParser + "/AssemblyInfo.cs");
	
	settings.Id = projParser;
	settings.Version = assemblyInfo.AssemblyVersion;
	settings.Title = assemblyInfo.Title;
	settings.Description = assemblyInfo.Description;	
	settings.BasePath = Directory(projParser);
    settings.OutputDirectory = projParser + "/NuGet/PackageSource/";
	NuGetPack(projParser + "/NuGet/GeneGenie.Gedcom.Parser.nuspec",  settings); 

	//GeneGenie.Gedcom.Reports

	Information("Creating nuget package " + projReports);
	assemblyInfo = ParseAssemblyInfo("./" + projReports + "/AssemblyInfo.cs");

	settings.Id = projReports; 
	settings.Version = assemblyInfo.AssemblyVersion;
	settings.Title = assemblyInfo.Title;
	settings.Description = assemblyInfo.Description;		
	settings.BasePath = Directory(projReports);
    settings.OutputDirectory = projReports + "/NuGet/PackageSource/";
	NuGetPack(projReports + "/NuGet/GeneGenie.Gedcom.Reports.nuspec",  settings); 		
});

Task("Publish")
    .IsDependentOn("Package")
    .Does(() =>
{
	Information("Publishing nuget packages");

	// Get the paths to the packages.
	var packages = new List<string>();
	
	packages.Add(projReports + "/NuGet/PackageSource/*.nupkg");
	packages.Add(projParser + "/NuGet/PackageSource/*.nupkg");
	packages.Add(projGedCom + "/NuGet/PackageSource/*.nupkg");

	Information("Publishing DISABLED (Edit build_nugget.cake to enable it)");
            
	// Push the package.
	//NuGetPush(packages, new NuGetPushSettings {
	//	Source = "http://example.com/nugetfeed",
	//	ApiKey = "4003d786-cc37-4004-bfdf-c4f3e8ef9b3a"
	//});

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
