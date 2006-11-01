 'File name: CreateIISApp.vbs
' Set some variables and constants we will use...
Dim blnInProcessApplication 'IIS In Process Application Flag
Dim objIIS 'ADSI IIS Object
Dim strVirtualDirectoryPath 'IIS Virtual Directory Path
Dim objFileSystem 'VBScript FileSystemObject
Dim strOwner 'NT Folder Owner
Dim objVirtualDirectory 'ADSI IIS Virtual Directory Object

Dim blnScriptPermissions 'IIS script permissions flag
Dim blnExecutePermissions ' IIS Execute permissions flag
Dim blnWritePermissions ' 
Dim blnReadPermissions '
Dim blnIntegratedWindowsAuthentication

Dim strHTTPReferer 'IIS Referrer Page
Dim objWSH 'Windows Scripting Host Object
Dim objRTC 'Return
Dim strACLCommand 'Command Line string to set ACLs
Dim MachineName ' computer name

strVirtualDirectoryPath = currentDir + "/" + branchDir 'IIS Virtual Directory Path
strVirtualDirectoryPath = Replace(strVirtualDirectoryPath, "/", "\")

' Get the Computer name using Wscript.Network and assign to IUSR to create IIS IUSR account name
Set WshNetwork = WScript.CreateObject("WScript.Network")
MachineName=WshNetwork.ComputerName
strOwner = "IUSR_" & MachineName
Set WshNetwork = Nothing
set wsc = Wscript.CreateObject("WScript.Shell")
'wsc.Popup "Setting Permissions for Computer Name = " & strOwner , 1
Wscript.echo ("Setting Permissions for Computer Name = " & strOwner)
blnScriptPermissions = "True" 
blnExecutePermissions = "True"
blnWritePermissions = "True"
blnReadPermissions = "True"
blnIntegratedWindowsAuthentication = "True"

Dim strIisSiteRoot: strIisSiteRoot = "IIS://localhost/W3SVC/" & webSiteName & "/Root"
Wscript.echo ("strIisSiteRoot = " & strIisSiteRoot)
Dim strObjectType: strObjectType = "IIsWebVirtualDir"

' Does this IIS application already exist in the metabase?
On Error Resume Next
Dim strIisVirtualDirFullName: strIisVirtualDirFullName = strIisSiteRoot & "/" & strVirtualDirectoryName
Set objIIS = GetObject(strIisVirtualDirFullName)
If Err.Number = 0 Then
	Wscript.echo ("An application '" & strIisVirtualDirFullName & "' already exists. ")
	Wscript.echo ("Deleting application '" & strIisVirtualDirFullName &  "'")
	Dim objIisRoot
	Set objIisRoot = GetObject(strIisSiteRoot)
	objIisRoot.Delete strObjectType, strVirtualDirectoryName
	If Err.Number = 0 Then
		Wscript.echo ("Application '" & strIisVirtualDirFullName & "' seems to be deleted")
	Else
		Wscript.echo ("Application '" & strIisVirtualDirFullName & "' was not deleted")
		Wscript.quit
	End If
End If
Set objIIS = Nothing

'Now use IIS administration objects to create the IIS application in the metabase. 
'Create the IIS application
Set objIIS = GetObject(strIisSiteRoot) '"IIS://localhost/W3SVC/1/Root"
'                 strVirtualDirectoryPath = objIIS.Path & "\" & strVirtualDirectoryName

' First check for and optionally create the physical folder under wwwroot
Set objFileSystem = Wscript.CreateObject("Scripting.FileSystemObject")
On Error Resume Next
Set Folder = objFileSystem.GetFolder(strVirtualDirectoryPath)
If Hex(Err.number) = "4C" Then
	'wsc.Popup "Creating folder " & strVirtualDirectoryPath , 1
	Wscript.echo ("Creating folder " & strVirtualDirectoryPath )
	set f = objFileSystem.CreateFolder(strVirtualDirectoryPath)
End If
Set objFileSystem = Nothing

'Using IIS Administration object , turn on script/execute permissions and define the virtual directory as an 'in-process application. 
Set objVirtualDirectory = objIIS.Create(strObjectType, strVirtualDirectoryName)
objVirtualDirectory.AccessScript = blnScriptPermissions
objVirtualDirectory.Path = strVirtualDirectoryPath
objVirtualDirectory.AppCreate blnInProcessApplication
objVirtualDirectory.AccessWrite = blnWritePermissions 
objVirtualDirectory.AccessRead = blnReadPermissions
objVirtualDirectory.AccessExecute = blnExecutePermissions
objVirtualDirectory.AuthNTLM = blnIntegratedWindowsAuthentication
objVirtualDirectory.AuthAnonymous =True
objVirtualDirectory.AnonymousUserName=strOwner
objVirtualDirectory.AnonymousPasswordSync=True
objVirtualDirectory.AppCreate (True)
objVirtualDirectory.SetInfo 

'Set Change Permissions for the owner using CACLS.exe
' need to "|" pipe the "Y" yes answer to the command "Are you sure?" prompt for this to work (see KB: Q135268 )
strACLCommand = "cmd /c echo y| CACLS "
strACLCommand = strACLCommand & strVirtualDirectoryPath
strACLCommand = strACLCommand & " /g " & strOwner & ":C"
Set objWSH = Server.CreateObject("WScript.Shell")
objRTC = objWSH.Run (strACLCommand , 0, True)
Set objWSH = Nothing

'Display results
strRes = "Web Application Created Sucessfully" & vbCRlf
strRes = strRes & "Name : " & strVirtualDirectoryName & vbCRlf
strRes = strRes & "Path : " & strVirtualDirectoryPath & vbCRlf
strRes = strRes & "Script Permissions : " & blnScriptPermissions & vbCRlf
strRes = strRes & "Read Permissions : " & blnReadPermissions & vbCRlf
strRes = strRes & "Write Permissions: " & blnWritePermissions & vbCrLf
strRes = strRes & "Execute Permission: " & blnExecutePermissions & vbCrLf
strRes = strRes & "Integrated Windows Authentication: " & blnIntegratedWindowsAuthentication & vbCrLf
strRes = strREs & strOwner & " granted change permissions" & vbCrlF
wscript.echo strRes

