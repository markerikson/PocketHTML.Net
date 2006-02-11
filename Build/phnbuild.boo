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
	
def ObfuscateEXE(inputExeLocation, newBuildDir):
	xcproj = "PocketHTML-xenocode.xcproj"
	xcode = XmlDocument()
	xcode.Load(xcproj)
	
	nodes = xcode.GetElementsByTagName("Assembly")
	outputNode = nodes[0]
	outputNode.Attributes["SourcePath"].Value = inputExeLocation
	
	nodes = xcode.GetElementsByTagName("OutputLocation")
	outputNode = nodes[0]
	outputNode.Attributes["Value"].Value = newBuildDir
	
	xcode.Save(xcproj)
	
	xccLocation = "c:\\Program Files\\Programming\\Utilities\\Xenocode 2005\\xcc.exe"
	xcc = Process()
	xcc.StartInfo.FileName = xccLocation
	xcc.StartInfo.Arguments = xcproj	
	xcc.StartInfo.UseShellExecute = false
	
	xcc.Start()
	xcc.WaitForExit()
	
	
def EnableHighResolution(currDir, newBuildDir):
	phnLocation = newBuildDir + "\\PocketHTML.Net.exe"
	
	ver = FileVersionInfo.GetVersionInfo(phnLocation)	
	
	versionStringDots = ver.FileVersion
	versionStringCommas = /\./.Replace(versionStringDots, ",")
	
	UpdateRC(currDir, versionStringCommas, versionStringDots)
	
	CompileResFile(currDir)
	
	RunRes2Exe(currDir, phnLocation)
	
	
def UpdateRC(currDir as string, versionStringCommas as string, versionStringDots as string):
	rcpath = currDir + "\\PocketHTML-hires.rc"
	sr = StreamReader(rcpath)
	rctext = sr.ReadToEnd()
	sr.Close()
	
	modtext = /\d+,\d+,\d+,\d+/.Replace(rctext, versionStringCommas)	
	modtext = /\d+\.\d+\.\d+\.\d+/.Replace(modtext, versionStringDots)
		
	sw = StreamWriter(rcpath)
	sw.WriteLine(modtext)
	sw.Close()
	

def CompileResFile(currDir as string):
	rcpath = currDir + "\\PocketHTML-hires.rc"
	compilerpath = "c:\\Program Files\\Microsoft Visual Studio 8\\VC\\bin\\rc.exe"
	
	rc = Process()
	rc.StartInfo.FileName = compilerpath
	rc.StartInfo.Arguments = "/r /v " + rcpath
	rc.StartInfo.UseShellExecute = false
	
	rc.Start()	
	rc.WaitForExit()
	

def RunRes2Exe(currDir as string, phnLocation as string):
	res2exeLocation = "C:\\toolkits\\Developer Resources for Windows Mobile 2003 Second Edition\\tools\\res2exe.exe"
	respath = currDir + "\\PocketHTML-hires.res"
	
	res2exe = Process()
	res2exe.StartInfo.FileName = "\"" + res2exeLocation + "\""
	res2exe.StartInfo.Arguments = "-r -c " + respath + " " + phnLocation
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
		
	buildMode = argv[0]
	versionString = argv[1]	
	
	currDir = Directory.GetCurrentDirectory()
	mainDir = currDir.Substring(0, currDir.IndexOf("\\Build"))
	buildsDir = currDir + "\\Builds"
	newBuildDir = buildsDir + "\\" + versionString
	templatesDir = mainDir + "\\Templates"
	
	if not Directory.Exists(newBuildDir):
		Directory.CreateDirectory(newBuildDir)
		
	binDir = mainDir + "\\bin\\" + buildMode
	inputExeLocation = binDir + "\\PocketHTML.Net.exe"
	
	ObfuscateEXE(inputExeLocation, newBuildDir)
	
	EnableHighResolution(currDir, newBuildDir)
		
	nircmd = Process()
	nircmd.StartInfo.FileName = "nircmdc.exe"
	nircmd.StartInfo.CreateNoWindow = true
	nircmd.StartInfo.RedirectStandardOutput = true
	nircmd.StartInfo.UseShellExecute = false
	
	infDirs = [newBuildDir, mainDir, templatesDir]
	SetupINF(nircmd, currDir, infDirs)
	
	BuildCab(nircmd, currDir, newBuildDir)
	
	CreateSetup(nircmd, currDir, newBuildDir, buildMode, versionString)
	
	print("Build complete")
