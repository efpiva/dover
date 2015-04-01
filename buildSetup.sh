#!/bin/bash
export fulltag=$(git describe --tags)
export tag=$(echo $fulltag | sed s/-/./ | sed s/-.*//)
export dots=${tag//[^.]}

if [ ${#dots} -lt 3 ]
then
    tag=$tag".0"
fi

MSBuild.exe AddOnInstallAPI/AddOnInstallAPI.sln //p:Configuration=Release
MSBuild.exe Dover.sln //p:Configuration=Release //p:Platform=x64

cp AddOnInstallAPI/bin/Release/AddOnInstallAPI.exe Core/bin/x64/Release
cp Install/Doverx64.iss Core/bin/x64/Release
pushd .
cd Core/bin/x64/Release
iscc Doverx64.iss
popd

echo "Creating ard file for $fulltag on branch $branch - git_branch $GIT_BRANCH"

rm -rf build

export setup_dir=build/x64/setup
export exe_dir=build/x64/exe
export dll_dir=build/x64/dll

mkdir -p $exe_dir
mkdir -p $dll_dir
mkdir -p $setup_dir

cp -a Core/bin/x64/Release/*.exe $exe_dir
cp -a Core/bin/x64/Release/*.dll $dll_dir
cp -a Core/bin/x64/Release/pt-BR $dll_dir
cp -a Core/bin/x64/Release/Output/setup.exe $setup_dir

rm -rf temp
mkdir temp
cp Install/Doverx64.xml temp
cp $setup_dir/setup.exe temp
cp $exe_dir/Dover.exe temp

cd temp
AddOnRegDataGen Doverx64.xml $tag setup.exe setup.exe Dover.exe
cd ..

mv temp/Dover.ard $setup_dir

# 32 bits stuff
MSBuild.exe Dover.sln //p:Configuration=Release //p:Platform=x86

cp Install/Dover.iss Core/bin/Release
pushd .
cd Core/bin/Release
iscc Dover.iss
popd

export setup_dir=build/x86/setup
export exe_dir=build/x86/exe
export dll_dir=build/x86/dll

mkdir -p $exe_dir
mkdir -p $setup_dir
mkdir -p $dll_dir

cp -a Core/bin/Release/*.exe $exe_dir
cp -a Core/bin/Release/*.dll $dll_dir
cp -a Core/bin/Release/pt-BR $dll_dir
cp -a Core/bin/Release/Output/setup.exe $setup_dir

rm -rf temp
mkdir temp
cp Install/Dover.xml temp
cp $setup_dir/setup.exe temp
cp $exe_dir/Dover.exe temp

cd temp
AddOnRegDataGen Dover.xml $tag setup.exe setup.exe Dover.exe
cd ..

mv temp/Dover.ard $setup_dir
