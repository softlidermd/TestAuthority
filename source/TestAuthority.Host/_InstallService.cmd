sc stop "TestAuthority"
timeout /t 5 /nobreak > NUL
sc delete "TestAuthority"
sc create "TestAuthority" binPath= "%~dp0TestAuthority.Host.exe"
sc failure "TestAuthority" actions= restart/60000/restart/60000/restart/60000 reset= 86400
sc start "TestAuthority"
sc config "TestAuthority" start=auto