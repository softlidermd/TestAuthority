sc stop "TestAuthority"
timeout /t 5 /nobreak > NUL
sc delete "TestAuthority"