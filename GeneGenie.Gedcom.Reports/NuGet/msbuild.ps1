Write-Host "Building Nuget Package with all platforms using msbuild"

$msbuild = "C:\Windows\Microsoft.NET\Framework\v4.0.30319\msbuild.exe"

Invoke-Expression $msbuild ../GeneGenie.Gedcom.Reports.csproj /t:Build /p:Configuration="Release 4.5"
Invoke-Expression $msbuild ../GeneGenie.Gedcom.Reports.csproj /t:Build;Package;Publish /p:Configuration="Release 4.0"
