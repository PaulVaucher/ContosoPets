# Script de nettoyage post-build s�curis�
param(
    [string]$OutputPath = "",
    [switch]$DeepClean = $false
)

Write-Host "Cleaning post-build memory artifacts..." -ForegroundColor Green

if ($DeepClean) {
    Write-Host "Deep clean mode active" -ForegroundColor Yellow
    # Suppression compl�te (� utiliser avec prudence)
    Remove-Item -Recurse -Force -ErrorAction Ignore "bin/Debug/temp" 
    Remove-Item -Recurse -Force -ErrorAction Ignore "obj/temp"
    
    # Forcer le garbage collection (utile apr�s de gros builds)
    [GC]::Collect()
    [GC]::WaitForPendingFinalizers()
    [GC]::Collect()
} else {
    # Nettoyage standard (s�curis�)
    Write-Host "Temporary files cleanup..."
    
    if ($OutputPath -and (Test-Path $OutputPath)) {
        # Nettoyer uniquement les fichiers temporaires
        Get-ChildItem -Path $OutputPath -Recurse -Include "*.tmp", "*.log", "*.cache" -ErrorAction SilentlyContinue | 
            Remove-Item -Force -ErrorAction SilentlyContinue
            
        # Nettoyer les dossiers temporaires sp�cifiques
        $tempDirs = @("temp", "logs", "cache", "*.pdb.tmp")
        foreach ($dir in $tempDirs) {
            $fullPath = Join-Path $OutputPath $dir
            if (Test-Path $fullPath) {
                Remove-Item $fullPath -Recurse -Force -ErrorAction SilentlyContinue
            }
        }
    }
}

Write-Host "Cleanup done." -ForegroundColor Green