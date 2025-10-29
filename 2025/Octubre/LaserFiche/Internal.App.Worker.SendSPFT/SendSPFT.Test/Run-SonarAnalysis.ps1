coverlet "bin\Debug\net8.0\Internal.App.Worker.SendSPFT.Test.dll" `
    --target "dotnet" `
    --targetargs "test --no-build" `
    --format opencover `
    --output "..\TestResults\coverage.opencover.xml"

$env:JAVA_HOME="D:\Users\60500258\Downloads\jdk-17.0.16+8"
$env:PATH="$env:JAVA_HOME\bin;$env:PATH"
java -version

$sonarScanner="D:\Users\60500258\Downloads\sonar-scanner-7.1.1.96069-net\SonarScanner.MSBuild.dll"
$coverageFile="D:\TV\LF\git\Internal.App.Worker.SendSPFT\TestResults\coverage.opencover.xml"

dotnet $sonarScanner begin /k:"Internal.App.Worker.SendSPFT" `
    /d:sonar.host.url="http://crsjce010275vm:9000" `
    /d:sonar.login="squ_655efb16926f5180dbfec52b467475a345896b5a" `
    /d:sonar.cs.opencover.reportsPaths="$coverageFile" `
    /d:sonar.exclusions="**/temp/**,**/bin/**,**/obj/**" `
    /d:sonar.cpd.exclusions="**/temp/**,**/bin/**,**/obj/**"

dotnet build /nr:false
dotnet test --no-build
dotnet $sonarScanner end /d:sonar.login="squ_655efb16926f5180dbfec52b467475a345896b5a"

Write-Host "Análisis Sonar con cobertura finalizado."
