#!/bin/bash

which dotnet-format > /dev/null 2>&1;
RESULT=$?

if [ $RESULT -ne 0 ]; then
    printf "You do not have dotnet-format installed. Want to install it now (y/n)? "
    read -r choice

    case "$choice" in
    y|Y ) dotnet tool install --global dotnet-format --version 5.1.250801;;
    n|N ) echo "Skipping installation";;
    * ) exit 0;;
    esac
fi

GIT_HOOKS_DIR=$(git rev-parse --git-path hooks)

if [ ! -d "$GIT_HOOKS_DIR" ]; then
    GIT_HOOKS_DIR=".git/hooks/"
    echo "$GIT_HOOKS_DIR: Directory not found - Creating directory"
    mkdir -p $GIT_HOOKS_DIR
fi

echo "$GIT_HOOKS_DIR: Copying the pre-commit hook into directory"
cp .git_hooks/pre-commit "$GIT_HOOKS_DIR"/pre-commit
cp .git_hooks/pre-commit "$GIT_HOOKS_DIR"/pre-push

# ensure they are executable
chmod +x "$GIT_HOOKS_DIR/pre-commit" "$GIT_HOOKS_DIR/pre-push"
