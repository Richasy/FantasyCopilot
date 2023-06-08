# Get the root directory path.
$rootPath = Split-Path -Parent $MyInvocation.MyCommand.Path

# Find subdirectories and execute the "dotnet publish" command.
Get-ChildItem -Path $rootPath -Directory | ForEach-Object {
    Set-Location $_.FullName
    dotnet publish -c Release -r win-x64 --self-contained false
}

# Move the file to the package directory.
$packagePath = Join-Path $rootPath "package"
if (-not (Test-Path $packagePath)) {
    New-Item -ItemType Directory -Path $packagePath | Out-Null
}

Get-ChildItem -Path $rootPath -Directory | ForEach-Object {
    $sourcePath = Join-Path $_.FullName "bin\Release\net7.0\win-x64"
    if (Test-Path $sourcePath) {
        Get-ChildItem -Path $sourcePath -Recurse | ForEach-Object {
            $destinationPath = Join-Path $packagePath $_.FullName.Substring($sourcePath.Length + 1)
            if (-not (Test-Path $destinationPath)) {
                Copy-Item $_.FullName -Destination $destinationPath
            }
        }
    }
}

# Copy the config.json file to the package directory.
$configPath = Join-Path $rootPath "config.json"
if (Test-Path $configPath) {
    Copy-Item $configPath -Destination $packagePath
}

# Delete the "native" directory.
$nativePath = Join-Path $packagePath "native"
if (Test-Path $nativePath) {
    Remove-Item $nativePath -Recurse -Force
}

# Delete the "publish" directory.
$publishPath = Join-Path $packagePath "publish"
if (Test-Path $publishPath) {
    Remove-Item $publishPath -Recurse -Force
}

# Packaging as a .fcpkg file.
$zipPath = Join-Path $rootPath "package.zip"
Compress-Archive -Path $packagePath\* -DestinationPath $zipPath -Force
Rename-Item $zipPath -NewName "com.richasy.fantasycopilot.core.fcpkg"

# Delete the package directory.
Remove-Item $packagePath -Recurse -Force

# Output completion information.
Write-Host "Package completed"
Set-Location $rootPath