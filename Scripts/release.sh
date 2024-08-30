#!/bin/bash

# Instantly exit if any command fails
set -e

# Print usage text
usage()
{
   echo ""
   echo "Usage: $0 release_version"
   echo -e "\trelease_version\tThe release version in 3-digit semver format."
   exit 1 # Exit script after printing help
}

# Ensure there are the right number of arguments
if [ "$#" -ne 1 ]
then
  usage
fi

# Ensure the sem ver is correctly formatted
if [[ ! "$1" =~ ^[0-9]+\.[0-9]+\.[0-9]+$ ]]
then
  usage
fi

# Set up the variables
tag_name="v$1"
full_version="$1.0"
script_directory=$(dirname "$(realpath "$0")")
base_directory="${script_directory}/../"
app_manifest="${base_directory}app.manifest"
package_appx_manifest="${base_directory}CourseEquivalencyDesktopPackagingProject/Package.appxmanifest"

# Replace the version numbers in the manifests
sed -i "s/assemblyIdentity .* name=\"ExCourseEquivalency\"/assemblyIdentity version=\"${full_version}\" name=\"ExCourseEquivalency\"/g" "${app_manifest}"
sed -i "s/ Version=.* \/>/ Version=\"${full_version}\" \/>/g" "${package_appx_manifest}"

# Commit the changes and add the tag
git add "${app_manifest}" "${package_appx_manifest}"
git commit -m "Updating version number to $1."
git tag "${tag_name}"

# Notify of completion
echo ""
echo "Version update to $1 complete. If everything is correct, please run:"
echo -e "\tgit push"
echo -e "\tgit push origin tag ${tag_name}"
