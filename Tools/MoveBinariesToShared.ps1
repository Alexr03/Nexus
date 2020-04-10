Remove-Item –path "..\Nexus.Program\bin\Release\NexusBot" –recurse -force
Remove-Item –path "..\Nexus.Program\bin\Release\Nexus.zip" -force

New-Item -Path "..\Nexus.Program\bin\Release" -Name "Config" -ItemType "directory"
New-Item -Path "..\Nexus.Program\bin\Release" -Name "Modules" -ItemType "directory"
New-Item -Path "..\Nexus.Program\bin\Release" -Name "ModuleDependencies" -ItemType "directory"
New-Item -Path "..\Nexus.Program\bin\Release" -Name "Shared" -ItemType "directory"
New-Item -Path "..\Nexus.Program\bin\Release" -Name "NexusBot" -ItemType "directory"

Move-Item -Path "..\Nexus.Program\bin\Release\*.dll" -Destination "..\Nexus.Program\bin\Release\Shared"
Move-Item -Path "..\Nexus.Program\bin\Release\*" -Destination "..\Nexus.Program\bin\Release\NexusBot"

Compress-Archive -DestinationPath "..\Nexus.Program\bin\Release\Nexus.zip" -LiteralPath "..\Nexus.Program\bin\Release\NexusBot" -CompressionLevel Optimal