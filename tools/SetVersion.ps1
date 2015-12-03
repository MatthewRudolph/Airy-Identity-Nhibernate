[CmdletBinding()]
Param(
	[Parameter(Mandatory=$True)]
	[string][alias("s")]
	$version,
	
	[Parameter(Mandatory=$False)]
	[string][alias("t")]
	$releaseType = "Release"
)

function CalculateVersions()
{
	#Regex to match exactly 4 part version:
	$hasVersion = "'"+$version+"'" -match '([0-9]+)\.([0-9]+)\.([0-9]+)\.([0-9]+)';

	if (!$hasVersion)
	{
		Write-Output "No version found, using 0.1.0.1000 instead.";
		$major=0;
		$minor=1;
		$patch=0;
		$build=1000;
	}
	else
	{
		$versionFormattedCorrectly = $matches[0] -match "(?<major>[0-9]+)\.(?<minor>[0-9]+)\.(?<patch>[0-9]+)\.(?<build>[0-9]+)"	
		if (!$versionFormattedCorrectly) 
		{
			Write-Output "The supplied version is not formatted correctly: " $version
			return;
		}
	
		$major=$matches['major'] -as [int]
		$minor=$matches['minor'] -as [int]
		$patch=$matches['patch'] -as [int]
		$build=$matches['build'] -as [int]
	}

	$Script:AssemblyVersion = "$major.$minor.$patch.0"
	$Script:AssemblyFileVersion = "$major.$minor.$patch.$build"
	$Script:AssemblyInformationalVersion = "$major.$minor.$patch-$releaseType$build"
	$Script:NugetVersion = "$major.$minor.$patch"
	if ($releaseType -notmatch "^Release$")
	{
		$Script:NugetVersion = "$major.$minor.$patch-$releaseType$build"
	}

	Write-Output "Using Assembly Version: $AssemblyVersion"
	Write-Output "Using File Version: $AssemblyFileVersion"
	Write-Output "Using Informational Version: $AssemblyInformationalVersion"
	Write-Output "Using Nuget Version: $nugetVersion"
}

function SetAssemblyVersion()
{
	#$AssemblyFileVersion = "$major.$minor.$env:APPVEYOR_BUILD_NUMBER"
	#$AssemblyInformationalVersion = "$AssemblyFileVersion-$env:APPVEYOR_REPO_SCM" + ($env:APPVEYOR_REPO_COMMIT).Substring(0, 8)

	$fileAssemblyVersion = 'AssemblyVersion("' + $AssemblyVersion + '")';
	$fileFileVersion = 'AssemblyFileVersion("' + $AssemblyFileVersion + '")';
	$fileInformationalVersion = 'AssemblyInformationalVersion("' + $AssemblyInformationalVersion + '")';

	$foundFiles = get-childitem .\*AssemblyInfo.cs -recurse  
	foreach( $file in $foundFiles )  
	{
		if ($file.Name -eq $globalAssemblyFile)
		{
			#Don't patch the global info.
			continue;
		}

		$content = Get-Content "$file"
	
		Write-Output "Patching $file"
	
		$av = $fileAssemblyVersion
		$afv = $fileFileVersion
		$aiv = $fileInformationalVersion	
	
		$content -replace 'AssemblyVersion\("[0-9]+(\.([0-9]+|\*)){1,3}"\)', $av `
				 -replace 'AssemblyFileVersion\("[0-9]+(\.([0-9]+|\*)){1,3}"\)', $afv `
				 -replace 'AssemblyInformationalVersion\("[0-9]+(\.([0-9]+|\*)){1,3}.*"\)', $aiv | Set-Content "$file"
	
		Write-Output "Completed patching $file"
	}
}

CalculateVersions;
SetAssemblyVersion;