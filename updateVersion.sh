#!/bin/sh
export fulltag=$(git describe --tags)
export tag=$(echo $fulltag | sed s/-/./ | sed s/-.*//)
export dots=${tag//[^.]}
export branch=$(git rev-parse --abbrev-ref HEAD)

if [ ${#dots} -lt 3 ]
then
    tag=$tag".0"
fi

# Assembly version
cat Properties/AssemblyInfoTemplate.cs | sed "s/FULLTAG/$fulltag/" | sed "s/TAG/$tag/" |  sed "s/BRANCHNAME/$branch/" > Properties/AssemblyInfo.cs
# ISS setup
cat Install/DoverTemplate.iss | sed "s/FULLTAG/$fulltag/" | sed "s/TAG/$tag/" |  sed "s/BRANCH/$branch/" > Install/Dover.iss
cat Install/Doverx64Template.iss | sed "s/FULLTAG/$fulltag/" | sed "s/TAG/$tag/" |  sed "s/BRANCH/$branch/" > Install/Doverx64.iss
