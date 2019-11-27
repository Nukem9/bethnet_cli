```
Usage:

list_buildinfo_entitlements <user> <passwd>                                     Lists all public branches and projects from BuildInfo API
list_user_entitlements      <user> <passwd>                                     Lists all branches and projects available for <user> from CDP API
list_project_builds         <user> <passwd> <project_id> <branch_id>            Lists all <product> builds available for <user> from CDP API
download_project            <user> <passwd> <project_id> <branch_id>            Download a product and all associated depots
download_project_build      <user> <passwd> <project_id> <branch_id> <build_id> Download a specific product build and all associated depots
download_mod                <user> <passwd> <mod_id>                            Subscribe to and download a bethesda.net mod

Examples:

bethnet_cli.exe list_buildinfo_entitlements myusername mypassword
bethnet_cli.exe download_project myusername mypassword 10 15856
bethnet_cli.exe download_project_build myusername mypassword 20 100148 292198
bethnet_cli.exe download_mod myusername mypassword 911793


Certain products require ownership in order to decrypt files. Files are stored in the local downloads directory.
```