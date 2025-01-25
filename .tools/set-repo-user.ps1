param (
    [string]$username
)

if (-not $username) {
    Write-Host "Usage: set-repo-user.ps1 <username>"
    exit 1
}

git config --local credential.https://github.com.username $username