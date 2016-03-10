Import-Module psake
$nuget = (Get-Command nuget.exe).Path
$msbuild = (Get-Command msbuild.exe).Path
$hg = (Get-Command hg.exe).Path
$git = (Get-Command git.exe).Path

Task default -depends pack

Task clean {
    & $msbuild (Resolve-path $PSScriptRoot\Elementary.Hierarchy.Collections.sln) /t:Clean /p:Configuration=Release
    & $msbuild (Resolve-path $PSScriptRoot\Elementary.Hierarchy.Collections.sln) /t:Clean /p:Configuration=Debug
    
    Remove-Item $PSScriptRoot\*.nupkg -ErrorAction SilentlyContinue
}

Task build {

    & $msbuild (Resolve-path $PSScriptRoot\Elementary.Hierarchy.Collections.sln) /t:Build /p:Configuration=Release

} -precondition { Test-Path $msbuild } 

Task pack {

    & $nuget Pack (Resolve-path $PSScriptRoot\Elementary.Hierarchy.Collections\Elementary.Hierarchy.Collections.csproj) -Prop Configuration=Release -Build -Symbols -MSbuildVersion 14
    
    Copy-Item $PSScriptRoot\Elementary.Hierarchy.*.nupkg C:\src\packages

} -precondition { Test-Path $nuget } -depends clean 

Task commit {

    & $hg commit -m "Auto commit of changed files before push"
    & $git commit -m "Auto commit of changed files before push"

} -precondition { Test-Path $hg }

Task push {

    & $hg push bitbucket
    & $git push

} -precondition { Test-Path $hg } -depends commit

Task measure {
    
    Push-Location $PSScriptRoot\Elementary.Hierarchy.Collections.Benchmarks\bin\Release
    
    try {
        # clean old benchmarks
        #Get-ChildItem . -Directory | Remove-Item -Force -Recurse

        #$benchmarks = (Resolve-Path .\Elementary.Hierarchy.Collections.Benchmarks.exe)
        #& $benchmarks

    } catch {
        Pop-Location
    }

    $resultDirName = (Get-Date).ToString("yyyyMMddHHmmss")

    mkdir $PSScriptRoot\Elementary.Hierarchy.Collections.Benchmarks\Results\$resultDirName
    Copy-Item $PSScriptRoot\Elementary.Hierarchy.Collections.Benchmarks\bin\Release\* $PSScriptRoot\Elementary.Hierarchy.Collections.Benchmarks\Results\$resultDirName -Exclude @(".config",".dll",".exe",".pdb")

} -depends clean,build


    