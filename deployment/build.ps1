$ErrorActionPreference = "Stop"
Clear-Host

function Remove-Item-IfExists 
{
	param([string]$path)

	if (Test-Path -Path $path) {
		Remove-Item -Path $path -Force -Recurse
	}
}

function RemoveRedundantFiles
{
	param([string]$buildPath)

	Remove-Item-IfExists -Path "$buildPath\appsettings.Development.json" 
	Remove-Item-IfExists -Path "$buildPath\config.json" 
}

function UpdateProjectVersion
{
	param([string]$filePath, [string]$version)

	if (!(Test-Path -Path $filePath)) {
		throw "$filePath does not exist - unable to update project file"
	}

	$doc = New-Object System.Xml.XmlDocument
	$doc.Load($filePath)
	UpdateXmlNodeIfExists -xmlDoc $doc -xpath "//PropertyGroup/Version" -newValue $version
	UpdateXmlNodeIfExists -xmlDoc $doc -xpath "//PropertyGroup/AssemblyVersion" -newValue $version
	UpdateXmlNodeIfExists -xmlDoc $doc -xpath "//PropertyGroup/FileVersion" -newValue $version
	UpdateXmlNodeIfExists -xmlDoc $doc -xpath "//package/metadata/version" -newValue $version
	$doc.Save($filePath)
}

function UpdateXmlNodeIfExists
{
	param($xmlDoc, $xpath, $newValue)
	$node = $xmlDoc.SelectSingleNode($xpath)
	if ($null -ne $node)
	{
		$node.InnerText = $newValue
	}
}

function ZipFile
{
	param(
		[String]$sourceFile,
		[String]$zipFile
	)

	$exeloc = ""
	if (Test-Path -Path "C:\Program Files\7-Zip\7z.exe") {
		$exeloc = "C:\Program Files\7-Zip\7z.exe"
	}
	elseif (Test-Path -Path "C:\Program Files (x86)\7-Zip\7z.exe") {
		$exeloc = "C:\Program Files (x86)\7-Zip\7z.exe"
	}
	else {
		Write-Host "Unable to find 7-zip executable" -BackgroundColor Red -ForegroundColor White
		Exit 1
	}

	set-alias sz $exeloc  
	sz a -xr!'Data\users.json' -tzip -r $zipFile $sourceFile | Out-Null
}

$rootPath = $PSScriptRoot
$sourcePath = $rootPath.Replace("deployment", "") + "source"
$buildPath = "$rootPath\build"
$version = Read-Host -Prompt "What version are we building? [e.g. 2.3.0]"

# ensure the build folder exists and is empty
Write-Host "Removing previous build files"
Remove-Item -Force -Recurse -Path $buildPath 
New-Item -ItemType Directory -Force -Path $buildPath

Write-Host "Updating project files for version $version"
UpdateProjectVersion -filePath "$sourcePath\IISLogLoader.Console\IISLogLoader.Console.csproj" -version $version

Write-Host "Building version $version"
& dotnet publish $sourcePath\IISLogLoader.Console\IISLogLoader.Console.csproj /p:EnvironmentName=Production /p:Configuration=Release --output $buildPath

Write-Host "Removing redundant files"
RemoveRedundantFiles -buildPath $buildPath

# package it up 
$zipPath = "$rootPath\IISLogLoader_v$version.zip"
Write-Host "Packaging IISLogLoader version $version into $zipPath"
Remove-Item -Path $zipPath -Force -ErrorAction Ignore
ZipFile -sourcefile "$buildPath\*.*" -zipfile $zipPath 

Write-Host "========== BUILD SUCCESS ==========" -BackgroundColor Green -ForegroundColor White
