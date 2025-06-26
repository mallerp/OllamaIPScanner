@echo off
setlocal

REM 1. 框架依赖版发布
set OUTDIR=publish\framework_dependent
rd /s /q %OUTDIR% 2>nul
mkdir %OUTDIR%
dotnet publish OllamaIPScanner/OllamaIPScanner.csproj -c Release -o %OUTDIR% --self-contained false
REM 删除不必要的文件
cd %OUTDIR%
del /q *.pdb *.xml *.runtimeconfig.dev.json 2>nul
cd /d %~dp0

REM 2. 自包含版发布（x64）
set OUTDIR2=publish\self_contained
rd /s /q %OUTDIR2% 2>nul
mkdir %OUTDIR2%
dotnet publish OllamaIPScanner/OllamaIPScanner.csproj -c Release -o %OUTDIR2% --self-contained true -r win-x64 /p:PublishSingleFile=true /p:IncludeAllContentForSelfExtract=true
REM 只保留exe
cd %OUTDIR2%
for %%f in (*) do (
  if /I not "%%~xf"==".exe" del /q "%%f"
)
cd /d %~dp0

echo 发布完成！
pause 