Import-Module psake

$nuget = (Get-Command nuget.exe).Path
$msbuild = (Get-Command msbuild.exe).Path
$hg = (Get-Command hg.exe).Path
$git = (Get-Command git.exe).Path
$nunit = (Get-Command $PSScriptRoot\packages\NUnit.ConsoleRunner.3.2.0\tools\nunit3-console.exe).Path
$solutionFile = (Resolve-path $PSScriptRoot\Elementary.Hierarchy.Collections.sln)
$packedProject = (Resolve-path $PSScriptRoot\Elementary.Hierarchy.Collections\Elementary.Hierarchy.Collections.csproj)
$localPackageSource = (Resolve-Path "C:\src\packages")
$benchmarkResultExtensions = @(
    "*.csv"
    "*.html"
    "*.log" 
    "*.md" 
    "*.R"
    "*.txt"
)
    
Task default -depends build

Task package_restore {

    & $nuget restore

} -precondition { Test-Path $nuget }

Task clean {
    & $msbuild $solutionFile /t:Clean /p:Configuration=Release
    
    Remove-Item $PSScriptRoot\*.nupkg -ErrorAction SilentlyContinue
}

Task pack {

    & $nuget Pack $packedProject -Prop Configuration=Release -Build -Symbols -MSbuildVersion 14
    
    Copy-Item $PSScriptRoot\Elementary.Hierarchy.*.nupkg $localPackageSource
    Get-Item $PSScriptRoot\Elementary.Hierarchy.*.nupkg

} -precondition { Test-Path $nuget } -depends clean 

Task build {

    & $msbuild $solutionFile /t:Build /p:Configuration=Release

} -precondition { Test-Path $msbuild } -depends package_restore

Task test {

    & $nunit (Resolve-Path $PSScriptRoot/Elementary.Hierarchy.Collections.Test/Elementary.Hierarchy.Collections.Test.csproj)

} -precondition { Test-Path $nunit } -depends build,package_restore

Task measure {
    
    Push-Location $PSScriptRoot\Elementary.Hierarchy.Collections.Benchmarks\bin\Release
    
    try {
        # clean old benchmarks
        Get-ChildItem . -Directory | Remove-Item -Force -Recurse

        $benchmarks = (Resolve-Path .\Elementary.Hierarchy.Collections.Benchmarks.exe)
        & $benchmarks

    } catch {
        Pop-Location
    }

    $resultDirName = (Get-Date).ToString("yyyyMMddHHmmss")

    mkdir $PSScriptRoot\Elementary.Hierarchy.Collections.Benchmarks\Results\$resultDirName
    
    $benchmarkResultExtensions | ForEach-Object {
        Copy-Item `
            $PSScriptRoot\Elementary.Hierarchy.Collections.Benchmarks\bin\Release\$_ `
            $PSScriptRoot\Elementary.Hierarchy.Collections.Benchmarks\Results\$resultDirName
    }
} -depends clean,build


    