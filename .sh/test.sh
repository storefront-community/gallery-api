#!/bin/bash

set -e

solution_dir="src"
api_project_dir="Storefront.Gallery.API"
test_project_dir="Storefront.Gallery.Tests"

cd $solution_dir/$test_project_dir

dotnet test  \
  /p:AltCover="true" \
  /p:AltCoverForce="true" \
  /p:AltCoverThreshold="80" \
  /p:AltCoverOpenCover="true" \
  /p:AltCoverXmlReport="coverage/opencover.xml" \
  /p:AltCoverInputDirectory="$api_project_dir" \
  /p:AltCoverAttributeFilter="ExcludeFromCodeCoverage" \
  /p:AltCoverAssemblyExcludeFilter="System(.*)|xunit|$test_project_dir|$api_project_dir.Views"

dotnet reportgenerator \
  "-reports:coverage/opencover.xml" \
  "-reporttypes:Html;HtmlSummary" \
  "-targetdir:coverage/report"
