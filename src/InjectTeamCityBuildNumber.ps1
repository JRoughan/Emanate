$buildNumber = "$(BUILD_NUMBER)"
$productInfoFile = "ProductInfo.cs"
    
$assemblyPattern = "[0-9]+(\.([0-9]+|\*)){1,3}"  
$assemblyVersionPattern = 'AssemblyVersion\("([0-9]+(\.([0-9]+|\*)){1,3})"\)'  
      
$rawVersionNumberMatch = get-content $productInfoFile | select-string -pattern $assemblyVersionPattern | % { $_.Matches }              
$rawVersionNumber = $rawVersionNumberMatch.Groups[1].Value  
                    
$versionParts = $rawVersionNumber.Split('.')  
$versionParts[3] = ([int]$versionParts[3]) + 1  
$updatedAssemblyVersion = "{0}.{1}.{2}.{3}" -f $versionParts[0], $versionParts[1], $buildNumber, $versionParts[2]  
      

(Get-Content $productInfoFile) | ForEach-Object {  
            % {$_ -replace $assemblyPattern, $updatedAssemblyVersion }                
        } | Set-Content $productInfoFile -encoding UTF8