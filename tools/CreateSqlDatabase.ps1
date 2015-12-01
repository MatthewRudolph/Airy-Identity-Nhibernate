# Creates a new database for the tests to use.
# References:
#  http://sqlblog.com/blogs/allen_white/archive/2008/04/28/create-database-from-powershell.aspx

[CmdletBinding()]
Param(
    [Parameter(Mandatory=$True)]
    [string][alias("s")]
    $serverName,
    
    [Parameter(Mandatory=$True)]
    [string][alias("d")]
    $databaseName,

    [Parameter(Mandatory=$False)]
    [double][alias("f")]
    $databaseDataFileSize = "25",

    [Parameter(Mandatory=$False)]
    [double][alias("g")]
    $databaseDataFileGrowth = "25",

    [Parameter(Mandatory=$False)]
    [double][alias("l")]
    $databaseLogFileSize = "10",

    [Parameter(Mandatory=$False)]
    [double][alias("z")]
    $databaseLogFileGrowth = "25"
)

function EntryPoint()
{
    try
    {
        [System.Reflection.Assembly]::LoadWithPartialName("Microsoft.SqlServer.SMO")  | out-null    
        # Configure database.
        Write-Host $serverName;
        Write-Host $databaseDataFileSize;
        $server = new-object -TypeName Microsoft.SqlServer.Management.Smo.Server -argumentlist $serverName;
        $database = new-object -TypeName Microsoft.SqlServer.Management.Smo.Database -argumentlist $server, $databaseName;

        # Configure primary filegroup / data file.
        $primaryFileGroup = new-object ("Microsoft.SqlServer.Management.Smo.FileGroup") ($database, "PRIMARY");
        $database.FileGroups.Add($primaryFileGroup);
        $primaryFileName = $databaseName;
        $Primaryfile = new-object ("Microsoft.SqlServer.Management.Smo.DataFile") ($primaryFileGroup, $primaryFileName);
        $primaryFileGroup.Files.Add($Primaryfile);
        $Primaryfile.FileName = $server.Information.MasterDBPath + "\" + $primaryFileName + ".mdf";
        $Primaryfile.Size = [double]($databaseDataFileSize * 1024.0);
        $Primaryfile.GrowthType = "Percent";
        $Primaryfile.Growth = $databaseDataFileGrowth;
        $Primaryfile.IsPrimaryFile = 'True';

        # Configure log file.
        $logName = $databaseName + '_log';
        $logFile = new-object ('Microsoft.SqlServer.Management.Smo.LogFile') ($database, $logName);
        $database.LogFiles.Add($logFile);
        $logFile.FileName = $server.Information.MasterDBLogPath + '\' + $logName + '.ldf';
        $logFile.Size = [double]($databaseLogFileSize * 1024.0);
        $logFile.GrowthType = 'Percent';
        $logFile.Growth = $databaseLogFileGrowth;

        # Create the database.
        $database.Create();

        exit 0;
    }
    catch
    {
        throw;
        exit 1;
    }
}

EntryPoint;