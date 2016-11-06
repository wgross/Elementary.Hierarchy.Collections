Import-Module psake

Properties {

    $nuget = (Get-Command nuget.exe).Path
    $msbuild = (Get-Command msbuild.exe).Path
    $git = (Get-Command git.exe).Path

    $solutionFile = (Resolve-path $PSScriptRoot\Elementary.Hierarchy.Collections.sln)
    $packedProject = (Resolve-path $PSScriptRoot\Elementary.Hierarchy.Collections\Elementary.Hierarchy.Collections.csproj)
    $localPackageSource = (Resolve-Path "$PSScriptRoot\..\packages")
    
    $benchmarkResultExtensions = @(
        "*.csv"
        "*.html"
        "*.log" 
        "*.md" 
        "*.R"
        "*.txt"
    )
}
    
Task default -depends clean,build,test

#region Build
 
Task build_nuget {

    & $nuget restore

}

Task build_csharp {

    & $msbuild $solutionFile /t:Build /p:Configuration=Debug
}

Task build_package {
    
    & $nuget Pack $packedProject -Prop Configuration=Release -Build -Symbols -MSbuildVersion 14
}

Task build -depends build_nuget,buid_csharp 

#endregion 

Task clean_nuget {
    Remove-Item $PSScriptRoot/packages
}

Task clean {
    & $msbuild $solutionFile /t:Clean /p:Configuration=Release
    
    Remove-Item $PSScriptRoot\*.nupkg -ErrorAction SilentlyContinue
}

#region publish

Task pack {

    & $nuget Pack $packedProject -Prop Configuration=Release -Build -Symbols -MSbuildVersion 14
    
    Copy-Item $PSScriptRoot\Elementary.Hierarchy.*.nupkg $localPackageSource
    Get-Item $PSScriptRoot\Elementary.Hierarchy.*.nupkg

} -precondition { Test-Path $nuget } -depends clean 

#endregion 

Task test {

    $nunit = (Get-Command $PSScriptRoot\packages\NUnit.ConsoleRunner.3.2.1\tools\nunit3-console.exe).Path

    & $nunit (Resolve-Path $PSScriptRoot/Elementary.Hierarchy.Collections.Test/Elementary.Hierarchy.Collections.Test.csproj)

} -depends build,build_nuget

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


    