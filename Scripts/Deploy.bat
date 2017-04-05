echo Deploying meta..

xcopy /S /Y ..\..\..\..\meta.xml %GTAVRP_SERVER_PATH%\resources\rp\

echo Deployed!

echo Deploying acl..

xcopy /S /Y ..\..\..\..\acl.xml %GTAVRP_SERVER_PATH%\

echo Deployed!

echo Deploying client side resources..

xcopy /S /Y /R ..\..\..\Client %GTAVRP_SERVER_PATH%\resources\rp\Client

echo Deployed!

