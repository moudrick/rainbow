rem find "rainbow.googlecode.com" "%AppData%\Subversion\auth\svn.ssl.server\*" 
del %AppData%\Subversion\auth\svn.ssl.server\5cad9de1fb22b2b8494542f6b9c1603a
echo p | svn -u -N status  --username %1 --password %2 
