import System.Xml
import System.IO
import System.Diagnostics
import System.Windows.Forms
import System.Text

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
	ncsb.Append(",\"Common")
	ncsb.Append(num)
	ncsb.Append("\",,")
	ncsb.Append("\"")
	ncsb.Append(destDir)
	ncsb.Append("\"")
	
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

def Main(argv as (string)):
	
	if(argv.Length < 2):
		print("Usage: phnbuild [debug | release] [VersionString]")
		return
		
	buildMode = argv[0]
	versionString = argv[1]
	
	xcproj = "PocketHTML-xenocode.xcproj"
	
	currDir = Directory.GetCurrentDirectory()
	mainDir = currDir.Substring(0, currDir.IndexOf("\\Build"))
	buildsDir = currDir + "\\Builds"
	newBuildDir = buildsDir + "\\" + versionString
	xcodeOutputExeLocation = newBuildDir + "\\PocketHTML.Net.exe"
	setupExeLocation = newBuildDir + "\\PocketHTML" + versionString + "Setup.exe"
	templatesDir = mainDir + "\\Templates"
	
	if not Directory.Exists(newBuildDir):
		Directory.CreateDirectory(newBuildDir)
		
	binDir = mainDir + "\\bin\\" + buildMode
	inputExeLocation = binDir + "\\PocketHTML.Net.exe"
	
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
	xcc.StartInfo.CreateNoWindow = true
	xcc.StartInfo.RedirectStandardOutput = true
	xcc.StartInfo.UseShellExecute = false
	xcc.Start()
	output = xcc.StandardOutput.ReadToEnd()
	xcc.WaitForExit()
	
	print(output)
	
	infDirs = [newBuildDir, mainDir, templatesDir]
	i = 1
	nircmd = Process()
	nircmd.StartInfo.FileName = "nircmdc.exe"
	nircmd.StartInfo.CreateNoWindow = true
	nircmd.StartInfo.RedirectStandardOutput = true
	nircmd.StartInfo.UseShellExecute = false
	
	print("Setting keys in PocketHTML.inf")
	for infDir in infDirs:	
		nircmd.StartInfo.Arguments = BuildSourceDiskString(i, currDir, infDir)
		i += 1
		nircmd.Start()
		nircmd.WaitForExit()
	
	print("Done with PocketHTML.inf")
		
	cabwiz = Process()
	cabwiz.StartInfo.FileName = "cabwiz.exe"
	cabwiz.StartInfo.Arguments = "PocketHTML.inf /dest " + newBuildDir
	cabwiz.StartInfo.CreateNoWindow = true
	cabwiz.StartInfo.RedirectStandardOutput = true
	cabwiz.StartInfo.UseShellExecute = false

	cabwiz.Start()
	output = cabwiz.StandardOutput.ReadToEnd()
	cabwiz.WaitForExit()
	
	print(output)
	
	destCabName = newBuildDir + "\\PocketHTML" + versionString + ".cab"
	File.Move(newBuildDir + "\\PocketHTML.cab", destCabName)
	
	nircmdArgs = BuildSetupINIString(currDir, destCabName)
	print("nircmdargs: " + nircmdArgs)
	nircmd.StartInfo.Arguments = nircmdArgs
	nircmd.Start()
	nircmd.WaitForExit()
	
	ezsetup = Process()
	ezsetup.StartInfo.FileName = "ezsetup.exe"
	ezsetupArgs = "-l english -i PHNSetup.ini -r readme-" + buildMode + ".txt -e eula.txt -o " + newBuildDir + "\\PocketHTML" + versionString + "Setup.exe"
	ezsetup.StartInfo.Arguments = ezsetupArgs
	ezsetup.StartInfo.CreateNoWindow = true
	ezsetup.StartInfo.RedirectStandardOutput = true
	ezsetup.StartInfo.UseShellExecute = false
	
	ezsetup.Start()
	output = ezsetup.StandardOutput.ReadToEnd()
	ezsetup.WaitForExit()
	
	print(output)
	print("Build complete")
