#!/bin/bash

set -e

solution_dir="src"
api_project_dir="StorefrontCommunity.Gallery.API"
test_project_dir="StorefrontCommunity.Gallery.Tests"

dotnet clean $solution_dir

rm -rf $solution_dir/$api_project_dir/bin
rm -rf $solution_dir/$api_project_dir/obj
rm -rf $solution_dir/$test_project_dir/bin
rm -rf $solution_dir/$test_project_dir/obj
rm -rf $solution_dir/$test_project_dir/coverage

dotnet restore $solution_dir
