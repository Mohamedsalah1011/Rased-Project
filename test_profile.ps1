$ErrorActionPreference = 'Stop'
$baseUrl = "http://localhost:5010/api/Account"

$email = "profiletest9823@example.com"
$password = "P@ssw0rd123!"

try {
    Write-Host "Logging in..."
    $loginBody = @{
        Email = $email
        Password = $password
    } | ConvertTo-Json

    $loginResponse = Invoke-RestMethod -Uri "$baseUrl/Login" -Method Post -Body $loginBody -ContentType "application/json"
    $token = $loginResponse.token
    
    if (-not $token) {
        Write-Host "Failed to get token! Response was:"
        $loginResponse | Format-List
        exit 1
    }
    
    Write-Host "Token obtained: $token"

    $headers = @{
        Authorization = "Bearer $token"
    }

    Write-Host "`n--- GET Profile ---"
    $profile = Invoke-RestMethod -Uri "$baseUrl/profile" -Method Get -Headers $headers
    $profile | Format-List

    Write-Host "`n--- PUT Profile ---"
    $updateBody = @{
        FullName = "Updated Profile Name"
        PhoneNumber = "0120001234"
        PlateNumber = "ABC-123"
    } | ConvertTo-Json

    Invoke-RestMethod -Uri "$baseUrl/profile" -Method Put -Headers $headers -Body $updateBody -ContentType "application/json" | Out-Null
    Write-Host "Profile updated successfully."

    Write-Host "`n--- GET Profile (After Update) ---"
    $updatedProfile = Invoke-RestMethod -Uri "$baseUrl/profile" -Method Get -Headers $headers
    $updatedProfile | Format-List

    Write-Host "`n--- POST Change Password ---"
    $newPassword = "NewP@ssw0rd123!"
    $changePasswordBody = @{
        OldPassword = $password
        NewPassword = $newPassword
        ConfirmPassword = $newPassword
    } | ConvertTo-Json

    Invoke-RestMethod -Uri "$baseUrl/change-password" -Method Post -Headers $headers -Body $changePasswordBody -ContentType "application/json" | Out-Null
    Write-Host "Password changed successfully."
    
} catch {
    Write-Host "An error occurred: $($_.Exception.Message)"
    if ($_.Exception.Response) {
        $reader = New-Object System.IO.StreamReader($_.Exception.Response.GetResponseStream())
        $responseBody = $reader.ReadToEnd()
        Write-Host "Response body: $responseBody"
    }
    exit 1
}
