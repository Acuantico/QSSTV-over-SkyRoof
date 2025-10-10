# QSSTV-over-SkyRoof

Add-on para integrar QSSTV dentro de SkyRoof y así recibir y decodificar imágenes SSTV directamente en la plataforma.

## Créditos
- Desarrollo del add-on: [Acuantico Power](https://acuanticopower.com)
- Proyecto QSSTV original: [ON4QZ/QSSTV](https://github.com/ON4QZ/QSSTV)
- Plataforma base SkyRoof: [VE3NEA/SkyRoof](https://github.com/VE3NEA/SkyRoof)

## Guía de compilación
1. **Prerrequisitos**
   - Windows 10/11 x64
   - [.NET SDK 9.0](https://dotnet.microsoft.com/es-es/download)
   - [Inno Setup 6](https://jrsoftware.org/isdl.php) (para generar el instalador)
   - Herramientas de compilación de Visual Studio 2022 con soporte para C++ (requeridas por las dependencias nativas)
2. **Compilar SkyRoof con el plugin**
   ```powershell
   dotnet restore .\SkyRoof\SkyRoof.csproj
   dotnet build .\SkyRoof\SkyRoof.csproj -c Release
   ```
3. **Generar el instalador del add-on**
   ```powershell
   & "C:\Program Files (x86)\Inno Setup 6\ISCC.exe" .\install\QSSTVPlugin.iss
   ```
   El ejecutable resultante quedará en `install\SkyRoof_QSSTV_Plugin_Setup.exe`.

Si prefieres evitar la compilación local, puedes descargar el instalador ya generado en [acuanticopower.com/qsstv-over-skiroof](https://acuanticopower.com/qsstv-over-skiroof).
