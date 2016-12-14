# only run if on appveyor, don't want to disable strong name verification elsewhere
if ($env:APPVEYOR) {
    # mount file share with test easy files
    $drive = $env:TestEasySupportPath.replace("/", "");
    echo "net use z: \\${env:FilesAccount}.file.core.windows.net\testeasy /u:${env:FilesAccount} <key>"
    net use $drive \\${env:FilesAccount}.file.core.windows.net\testeasy /u:${env:FilesAccount} ${env:FilesKey}

    # disable strong name verification
    echo 'new-item -path "HKLM:\SOFTWARE\Wow6432Node\Microsoft\StrongName\Verification" -name "*,*" -force'
    new-item -path "HKLM:\SOFTWARE\Wow6432Node\Microsoft\StrongName\Verification" -name "*,*" -force

    # create dummy web site config for test easy'
    echo 'new-item -path "${env:SystemDrive}${env:HomePath}\Documents\My Web Sites\WebSite1\Web.config" -type file -force'
    new-item -path "${env:SystemDrive}${env:HomePath}\Documents\My Web Sites\WebSite1\Web.config" -type file -force -value '<?xml version="1.0" encoding="utf-8" ?><configuration><system.web></system.web></configuration>'
}
