param ([bool] $IncreaseMajor, [bool] $IncreaseMinor, [bool] $IncreasePatch, [string] $SelectedVersion, [string] $PackageName)

if ($SelectedVersion -ne "not-set") {
    Write-Host "Version set manually";
    Write-Output "##vso[task.setvariable variable=UPDATED_PACKAGE_VERSION;isOutput=true]$SelectedVersion"
}
else {
    if (!(Get-Module -ListAvailable -Name SemVerPS)) {
        Write-Host "Installing SemVerPS";
        Install-Module -Name SemVerPS -Force
    } 

    Import-Module -Name SemVerPS;
    
    $endpointUri = "https://azuresearch-usnc.nuget.org/query?q=$PackageName&prerelease=true";
    $response = Invoke-WebRequest -Uri $endpointUri -UseBasicParsing

    if ($response.StatusCode -ne 200) {
        Write-Error "Request failed $response";
        return;
    }

    $body = $response.Content;

    if ($null -eq $body) {
        Write-Error "Body is null";
        return;
    }

    $data = ConvertFrom-Json -InputObject $body;
    $version = $data[0].data.versions[-1].version;
    $semVer = ConvertTo-SemVer -Version $version;


    Write-Host "Found existing package version $semVer";
    
    $major = $semVer.Major;
    if ($IncreaseMajor) {
        Write-Host "Updating major version"
        $major = $major + 1;
    }
    
    $minor = $semVer.Minor;
    if ($IncreaseMinor) {
        Write-Host "Updating minor version"
        $minor = $minor + 1;
    }
    
    $patch = $semVer.Patch;
    if ($IncreasePatch) {
        Write-Host "Updating patch version"
        $patch = $patch + 1;
    }
    
    $newVersion = [Semver.SemVersion]::New($major, $minor, $patch, $semVer.Prerelease)
    
    Write-Host "Setting new version $newVersion"
    Write-Output "##vso[task.setvariable variable=UPDATED_PACKAGE_VERSION;isOutput=true]$newVersion"
}

