import System.Xml
import System.IO
import System.Diagnostics
import System.Windows.Forms
import System.Text
import System.Reflection

def BuildSourceDiskString(num, currDir, destDir):
	ncsb = StringBuilder()
	ncsb.Append("inisetval ")
	ncsb.Append("\"")
	ncsb.Append(currDir)
	ncsb.Append("\\PocketHTML.inf")
	ncsb.Append("\" ")
	ncsb.Append("\"SourceDisksNames\" ")
	ncsb.Append("\"")
	ncsb.Append(num)
	ncsb.Append("\" ")
	ncsb.Append(",\"~qCommon")
	ncsb.Append(num)
	ncsb.Append("~q\",,")
	ncsb.Append("\"~q")
	ncsb.Append(destDir)
	ncsb.Append("\\~q\"")
	
	return ncsb.ToString()
	
def BuildSetupINIString(currDir, cabName):
	ncsb = StringBuilder()
	ncsb.Append("inisetval ")
	ncsb.Append("\"")
	ncsb.Append(currDir)
	ncsb.Append("\\PHNSetup.ini")
	ncsb.Append("\" ")
	ncsb.Append("\"PocketHTML.Net\" ")
	ncsb.Append("\"CabFiles\" ")
	ncsb.Append("\"")
	ncsb.Append(cabName)
	ncsb.Append("\"")
	
	return ncsb.ToString()
	
#def ObfuscateEXE(inputExeLocation, newBuildDir):
#	xcproj = "PocketHTML-xenocode.xcproj"
#	xcode = XmlDocument()
#	xcode.Load(xcproj)
#	
#	nodes = xcode.GetElementsByTagName("Assembly")
#	outputNode = nodes[0]
#	outputNode.Attributes["SourcePath"].Value = inputExeLocation
#	
#	nodes = xcode.GetElementsByTagName("OutputLocation")
#	outputNode = nodes[0]
#	outputNode.Attributes["Value"].Value = newBuildDir
#	
#	xcode.Save(xcproj)
#	
#	xccLocation = "c:\\Program Files\\Programming\\Utilities\\Xenocode 2005\\xcc.exe"
#	xcc = Process()
#	xcc.StartInfo.FileName = xccLocation
#	xcc.StartInfo.Arguments = xcproj	
#	xcc.StartInfo.UseShellExecute = false
#	
#	xcc.Start()
#	xcc.WaitForExit()

def ObfuscateFiles(releaseMode as string):
	projectFile = "PocketHTML-dotfuscate-" + releaseMode + ".xml"
	
	dotfuscatorLocation = "C:\\Program Files\\Microsoft Visual Studio 8\\Application\\PreEmptive Solutions\\Dotfuscator Community Edition\\dotfuscator.exe"
	dotfuscator = Process()
	dotfuscator.StartInfo.FileName = dotfuscatorLocation
	dotfuscator.StartInfo.Arguments = projectFile
	dotfuscator.StartInfo.UseShellExecute = false
	
	dotfuscator.Start()
	dotfuscator.WaitForExit();
	
def EnableHighResolution(releaseMode as string, newBuildDir as string, currDir as string):
	#phnLocation = newBuildDir + "\\PocketHTML.Net.exe"
	phnLocation = currDir + "\\Builds\\Dotfuscated\\" + releaseMode + "\\PocketHTML.Net.exe"
	
	filenames = ["PocketHTML.Net", "TagEditor"]
	
	ver = FileVersionInfo.GetVersionInfo(phnLocation)	
	
	versionStringDots = ver.FileVersion
	versionStringCommas = /\./.Replace(versionStringDots, ",")
	
	for file in filenames:
		UpdateRC(file, currDir, versionStringCommas, versionStringDots)
		
		CompileResFile(file, currDir)
		
		RunRes2Exe(file, releaseMode, currDir, newBuildDir)
	
	
def UpdateRC(file as string, currDir as string, versionStringCommas as string, versionStringDots as string):
	#rcpath = currDir + "\\PocketHTML-hires.rc"
	rcpath = currDir + "\\" + file + "-hires.rc"
	sr = StreamReader(rcpath)
	rctext = sr.ReadToEnd()
	sr.Close()
	
	modtext = /\d+,\d+,\d+,\d+/.Replace(rctext, versionStringCommas)	
	modtext = /\d+\.\d+\.\d+\.\d+/.Replace(modtext, versionStringDots)
		
	sw = StreamWriter(rcpath)
	sw.WriteLine(modtext)
	sw.Close()
	

def CompileResFile(file as string, currDir as string):
	#rcpath = currDir + "\\PocketHTML-hires.rc"
	rcpath = currDir + "\\" + file + "-hires.rc"
	compilerpath = "c:\\Program Files\\Microsoft Visual Studio 8\\VC\\bin\\rc.exe"
	
	rc = Process()
	rc.StartInfo.FileName = compilerpath
	rc.StartInfo.Arguments = "/r /v " + rcpath
	rc.StartInfo.UseShellExecute = false
	
	rc.Start()	
	rc.WaitForExit()
	

def RunRes2Exe(file as string, releaseMode as string, currDir as string, outputDir as string):
	res2exeLocation = "C:\\toolkits\\Developer Resources for Windows Mobile 2003 Second Edition\\tools\\res2exe.exe"
	#respath = currDir + "\\PocketHTML-hires.res"
	inputRes = currDir + "\\" + file + "-hires.res"
	inputExe = currDir + "\\Builds\\Dotfuscated\\" + releaseMode + "\\" + file + ".exe"
	outputExe = outputDir + "\\" + file + ".exe"
	
	
	res2exe = Process()
	res2exe.StartInfo.FileName = "\"" + res2exeLocation + "\""
	res2exe.StartInfo.Arguments = "-r -c " + inputRes + " " + inputExe + " -fo " + outputExe
	res2exe.StartInfo.UseShellExecute = false
	
	res2exe.Start()
	res2exe.WaitForExit()
	
	
def SetupINF(nircmd as Process, currDir, infDirs):	
	i = 1
	
	print("Setting keys in PocketHTML.inf")
	for infDir in infDirs:	
		nircmd.StartInfo.Arguments = BuildSourceDiskString(i, currDir, infDir)
		i += 1
		nircmd.Start()
		nircmd.WaitForExit()
	
	print("Done with PocketHTML.inf")
	
def BuildCab(nircmd as Process, currDir, newBuildDir):
	cabwiz = Process()
	cabwiz.StartInfo.FileName = "cabwiz.exe"
	cabwiz.StartInfo.Arguments = "PocketHTML.inf /dest " + newBuildDir
	cabwiz.StartInfo.UseShellExecute = false

	cabwiz.Start()
	cabwiz.WaitForExit()
	
	
def CreateSetup(nircmd as Process, currDir, newBuildDir, buildMode, versionString):
	originalCabName = "PocketHTML.cab"
	originalCabLocation = newBuildDir + "\\" + originalCabName
	
	nircmdArgs = BuildSetupINIString(currDir, originalCabName)
	print("nircmdargs: " + nircmdArgs)
	nircmd.StartInfo.Arguments = nircmdArgs
	nircmd.Start()
	nircmd.WaitForExit()
	
	tempCabLocation = currDir + "\\" + originalCabName
	File.Copy(originalCabLocation, tempCabLocation)
	
	ezsetup = Process()
	ezsetup.StartInfo.FileName = "ezsetup.exe"
	ezsetupArgs = "-l english -i PHNSetup.ini -r readme-" + buildMode + ".txt -e eula.txt -o " + newBuildDir + "\\PocketHTML" + versionString + "Setup.exe"
	ezsetup.StartInfo.Arguments = ezsetupArgs
	ezsetup.StartInfo.UseShellExecute = false
	
	ezsetup.Start()
	ezsetup.WaitForExit()
	
	destCabName = "PocketHTML" + versionString + ".cab"
	destCabLocation = newBuildDir + "\\" + destCabName
	File.Copy(newBuildDir + "\\PocketHTML.cab", destCabLocation)
	File.Delete(tempCabLocation)
	
def Main(argv as (string)):
				
	if(argv.Length < 2):
		print("Usage: phnbuild [debug | release] [VersionString]")
		return
		
	# save command-line arguments
	buildMode = argv[0]
	versionString = argv[1]	
	
	# create directory strings based on location
	currDir = Directory.GetCurrentDirectory()
	mainDir = currDir.Substring(0, currDir.IndexOf("\\Build"))
	buildsDir = currDir + "\\Builds"
	newBuildDir = buildsDir + "\\" + versionString
	templatesDir = mainDir + "\\Templates"
	
	if not Directory.Exists(newBuildDir):
		Directory.CreateDirectory(newBuildDir)
	
	# TODO 
	binDir = mainDir + "\\bin\\" + buildMode
	inputExeLocation = binDir + "\\PocketHTML.Net.exe"
	
	print "Obfuscating files..."
	ObfuscateFiles(buildMode)
	
	# res2exe will output the .exe files in the output directory...
	print "Enabling high resolution..."
	EnableHighResolution(buildMode, newBuildDir, currDir) 
	
	# but not PocketHTML.Utility.dll
	dllLocation = mainDir + "\\PocketHTML.Utility\\bin\\" + buildMode + "\\PocketHTML.Utility.dll"
	dllDestination = newBuildDir + "\\PocketHTML.Utility.dll"
	File.Copy(dllLocation, dllDestination, true)
	
	nircmd = Process()
	nircmd.StartInfo.FileName = "nircmdc.exe"
	nircmd.StartInfo.CreateNoWindow = true
	nircmd.StartInfo.RedirectStandardOutput = true
	nircmd.StartInfo.UseShellExecute = false
	
	infDirs = [newBuildDir, mainDir, templatesDir]
	print "Setting up INF file..."
	SetupINF(nircmd, currDir, infDirs)
	
	print "Building CAB file..."
	BuildCab(nircmd, currDir, newBuildDir)
	
	print "Creating setup file..."
	CreateSetup(nircmd, currDir, newBuildDir, buildMode, versionString)
	
	print("Build complete")
