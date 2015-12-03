$globalAssemblyVersion = '17.23.1.1001'
$releaseType = "Release"

#Regex to match exactly 4 part version:
$hasAssemblyVersion = "'"+$globalAssemblyVersion+"'" -match '([0-9]+)\.([0-9]+)\.([0-9]+)\.([0-9]+)'

#Regex to match 2, 3 or 4 part version:
#$hasAssemblyVersion = "'"+$globalAssemblyVersion+"'" -match '[0-9]+(\.([0-9]+|\*)){1,3}'

if (!$hasAssemblyVersion)
{
	Write-Host "No version found, using 0.1.0.1000 instead."	
	$major=0
	$minor=1
	$patch=0 ## build
	$build=1000 ## revision
}
else
{
	$assemblyVersionFormattedCorrectly = $matches[0] -match "(?<major>[0-9]+)\.(?<minor>[0-9]+)\.(?<patch>[0-9]+)\.(?<build>[0-9]+)"
	##$assemblyVersionFormattedCorrectly = $matches[0] -match "(?<major>[0-9]+)\.(?<minor>[0-9])+(\.(?<patch>([0-9])))?(\.(?<build>([0-9])))?"
	
	if (!$assemblyVersionFormattedCorrectly) 
	{
		Write-Host "The supplied version is not formatted correctly: " $globalAssemblyVersion
		return;
	}
	
	$major=$matches['major'] -as [int]
	$minor=$matches['minor'] -as [int]
	$patch=$matches['patch'] -as [int]
	$build=$matches['build'] -as [int]
}

$AssemblyVersion = "$major.$minor.$patch.0"
$AssemblyFileVersion = "$major.$minor.$patch.$build"
$AssemblyInformationalVersion = "$major.$minor.$patch-$releaseType$build"
$NugetVersion = "$major.$minor.$patch"
if ($releaseType -notmatch "^Release$")
{
	$NugetVersion = "$major.$minor.$patch-$releaseType$build"
}

Write-Host "Using Assembly Version: $AssemblyVersion"
Write-Host "Using File Version: $AssemblyFileVersion"
Write-Host "Using Informational Version: $AssemblyInformationalVersion"
Write-Host "Using Nuget Version: $nugetVersion"

#$AssemblyFileVersion = "$major.$minor.$env:APPVEYOR_BUILD_NUMBER"
#$AssemblyInformationalVersion = "$AssemblyFileVersion-$env:APPVEYOR_REPO_SCM" + ($env:APPVEYOR_REPO_COMMIT).Substring(0, 8)


$fileVersion = 'AssemblyFileVersion("' + $AssemblyFileVersion + '")';
$informationalVersion = 'AssemblyInformationalVersion("' + $AssemblyInformationalVersion + '")';

$foundFiles = get-childitem .\*AssemblyInfo.cs -recurse  
foreach( $file in $foundFiles )  
{
	if ($file.Name -eq $globalAssemblyFile)
	{
		#Don't patch the global info.
		continue;
	}

	$content = Get-Content "$file"
	
	Write-Host "Patching $file"
	
	$afv = $fileVersion
	$aiv = $informationalVersion
	
	$hasFileAssemblyVersion = "'"+$content+"'" -match 'AssemblyVersion\("[0-9]+(\.([0-9]+|\*)){1,3}"\)'

	if ($hasFileAssemblyVersion)
	{
		$assemblyVersionFormattedCorrectly = $matches[0] -match "(?<major>[0-9]+)\.(?<minor>[0-9])+(\.(?<patch>([0-9])))?(\.(?<build>([0-9])))?"

		if ($assemblyVersionFormattedCorrectly) 
		{
			$fileMajor=$matches['major'] -as [int]
			$fileMinor=$matches['minor'] -as [int]	
			$filePatch=$matches['patch'] -as [int]
			$fileBuild=$matches['build'] -as [int]	
			
			
			$afv = "$fileMajor.$fileMinor.$filePatch.$fileBuild"
			$aiv = "$afv-master" + ("AREALLYLONGHASH").Substring(0, 8)
			
			Write-Host "Specific AssemblyVersion found, using that instead: $fileMajor.$fileMinor.$filePatch.$fileBuild"
			Write-Host "Patched File Version: $afv"
			Write-Host "Patched Informational Version: $aiv"
			
			$afv = 'AssemblyFileVersion("' + $afv + '")';
			$aiv = 'AssemblyInformationalVersion("' + $aiv + '")';
		}
		else
		{
			Write-Host "Specific AssemblyVersion found, but it's not formatted correctly, skipping."
		}
	}
	
	$content -replace 'AssemblyFileVersion\("[0-9]+(\.([0-9]+|\*)){1,3}"\)', $afv `
			 -replace 'AssemblyInformationalVersion\("[0-9]+(\.([0-9]+|\*)){1,3}"\)', $aiv | Set-Content "$file"
}
