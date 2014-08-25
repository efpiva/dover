Dover
=====

A development Framework for SAP Business One

### Building it

To build Dover, you'll need Git Bash installed and also in your windows PATH, because this is used to automatically generate a AssemblyInfo with the appropriate Version, based on the latest git tag. You can check if this is working accordingly by running the updateVersion.sh bash script.

### Packing it

To pack Dover, you need to call the bash script named buildSetup.sh. 

You'll need to have on your Path Git, Inno Setup, MSBuild and AddOnRegDataGen from Business One SDK. The output is placed on output folder named build.

### General information

You can check more information on [Dover WebSite](http://efpiva.github.io/).


