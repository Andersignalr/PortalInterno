@echo off
setlocal EnableDelayedExpansion

REM Pasta de origem (onde o .bat está)
set "ORIGEM=%~dp0"

REM Pasta de destino
set "DESTINO=%~dp0result"

REM Limite de tamanho (97152 bytes)
set MAXSIZE=97152

REM Cria a pasta result se não existir
if not exist "%DESTINO%" (
    mkdir "%DESTINO%"
)

REM Varre todos os arquivos (inclui subpastas)
for /r "%ORIGEM%" %%F in (*) do (

    REM Ignora a pasta result
    if /i not "%%~dpF"=="%DESTINO%\" (

        REM Verifica tamanho do arquivo
        if %%~zF LSS %MAXSIZE% (

            REM Nome da pasta onde o arquivo está
            set "PASTA=%%~nxdpF"
            for %%P in ("!PASTA:~0,-1!") do set "PASTA_NOME=%%~nxP"

            REM Nome do arquivo
            set "ARQUIVO=%%~nxF"

            REM Copia adicionando o nome da pasta
            copy "%%F" "%DESTINO%\!PASTA_NOME!_!ARQUIVO!" >nul
        )
    )
)

echo.
echo Copia finalizada! (somente arquivos menores que 2MB)
pause
