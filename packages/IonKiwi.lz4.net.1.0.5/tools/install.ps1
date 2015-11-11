param($installPath, $toolsPath, $package, $project)

function TryGetProjectItem($folder, $name) {
    $item = $null
    try {
        $item = $folder.ProjectItems.Item($name)
    }
    catch { }
    return $item
}

## Set property: CopyToOutputDirectory
$folder = $project
if ($folder -ne $null) {
    $appData = TryGetProjectItem $folder "App_Data"
    $item = TryGetProjectItem $folder "lz4.x86.dll"
    if ($item -ne $null) {
        if ($appData -eq $null) {
            $property = $item.Properties.Item("CopyToOutputDirectory")
            if ($property -ne $null) {
                $property.Value = 2
            }
        }
        else {
            Write-Host 'Detected a web project, copying assemblies to App_Data folder'
            $filePath = $item.Properties.Item("FullPath").Value
            $targetPath = $appData.Properties.Item("FullPath").Value
            if (Test-Path "$($targetPath)\lz4.x86.dll") {
                Remove-Item "$($targetPath)\lz4.x86.dll" -force
            }
            $appData.ProjectItems.AddFromFileCopy($filePath)
            $item.Delete()
        }
    }   
}

$folder = $project
if ($folder -ne $null) {
    $appData = TryGetProjectItem $folder "App_Data"
    $item = TryGetProjectItem $folder "lz4.x64.dll"
    if ($item -ne $null) {
        if ($appData -eq $null) {
            $property = $item.Properties.Item("CopyToOutputDirectory")
            if ($property -ne $null) {
                $property.Value = 2
            }
        }
        else {
            Write-Host 'Detected a web project, copying assemblies to App_Data folder'
            $filePath = $item.Properties.Item("FullPath").Value
            $targetPath = $appData.Properties.Item("FullPath").Value
            if (Test-Path "$($targetPath)\lz4.x64.dll") {
                Remove-Item "$($targetPath)\lz4.x64.dll" -force
            }
            $appData.ProjectItems.AddFromFileCopy($filePath)
            $item.Delete()
        }
    }   
}

$folder = $project
if ($folder -ne $null) {
    $appData = TryGetProjectItem $folder "App_Data"
    if ($appData -ne $null) {
        $xml = New-Object xml
        $config = $project.ProjectItems | where {$_.Name -eq "Web.config"}
        $localPath = $config.Properties | where {$_.Name -eq "LocalPath"}
        $xml.Load($localPath.Value)
        $node = $xml.SelectSingleNode("configuration/appSettings")
        $node2 = $node.SelectSingleNode("add[@key='lz4path']")
        if ($node2 -eq $null) {
            $e = $xml.CreateElement('add');
            $e.SetAttribute('key', 'lz4path');
            $e.SetAttribute('value', '..\App_Data');
            $node.AppendChild($e);
            $xml.Save($localPath.Value)
        }
    }
}
