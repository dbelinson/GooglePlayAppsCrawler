param($installPath, $toolsPath, $package, $project)

function TryGetProjectItem($folder, $name) {
    $item = $null
    try {
        $item = $folder.ProjectItems.Item($name)
    }
    catch { }
    return $item
}

$folder = $project
if ($folder -ne $null) {
    $appData = TryGetProjectItem $folder "App_Data"
    if ($appData -ne $null) {
        $item = TryGetProjectItem $appData "lz4.x86.dll"
        if ($item -ne $null) {
            $item.Delete()
        }
    }
}

$folder = $project
if ($folder -ne $null) {
    $appData = TryGetProjectItem $folder "App_Data"
    if ($appData -ne $null) {
        $item = TryGetProjectItem $appData "lz4.x64.dll"
        if ($item -ne $null) {
            $item.Delete()
        }
    }
}
