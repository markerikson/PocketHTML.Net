del "C:\Projects\PocketHTML.Net\Release\P*.*"
"C:\Program Files\Microsoft Visual Studio .NET 2003\PreEmptive Solutions\Dotfuscator Community Edition\dotfuscator.exe" release.xml
cd Release
c:\projects\cabwiz.exe ..\PocketHTML.Net_PPC.inf /dest c:\projects\PocketHTML.Net\Release 
c:\projects\ezsetup.exe -l english -i Setup.ini -r readme-release.txt -e ..\eula.txt -o c:\Projects\PocketHTML.Net\Release\PocketHTML11Setup.exe
ren PocketHTML.CAB PocketHTML11.cab
cd ..