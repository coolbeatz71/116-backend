#!/bin/sh

# C# Code Formatting Script
# Formats code using dotnet format with support for staged files and verification

# Colors for output
RED='\033[0;31m'
YELLOW='\033[1;33m'
GREEN='\033[0;32m'
BLUE='\033[0;34m'
NC='\033[0m' # No Color

staged_only=false

if ! TEMP=$(getopt -o s --long staged-only -n 'formatting' -- "$@"); then exit 1 ; fi
eval set -- "$TEMP"
while true; do
    case "$1" in
    -s | --staged-only ) staged_only=true; shift 2 ;;
    -- ) shift; break ;;
    * ) break ;;
    esac
done

if $staged_only; then
    echo "${BLUE}🔍 Checking for dotnet-format availability...${NC}"
    
    which dotnet-format > /dev/null 2>&1;
    format_res=$?
    if [ $format_res -ne 0 ]; then
        echo "${RED}❌ ERROR: dotnet-format is not installed!${NC}"
        echo "${YELLOW}📋 Code formatting is required for maintaining code quality${NC}"
        echo "${YELLOW}💡 To install dotnet-format, run the setup.sh script${NC}"
        exit 1;
    fi

    echo "${BLUE}🔎 Scanning for staged C# files...${NC}"
    FILES=$(git diff --staged --name-only --diff-filter=ACM "*.cs")
    
    if [ -z "$FILES" ]; then
        echo "${GREEN}✅ No staged C# files to format${NC}"
        exit 0
    fi

    echo "${GREEN}🎨 Formatting staged C# files...${NC}"
    dotnet format --include "$FILES"

    echo "${BLUE}📝 Re-staging formatted files...${NC}"
    echo "$FILES" | xargs git add

    echo "${GREEN}✅ Code formatting complete! All staged files are now properly formatted${NC}"
else
    echo "${BLUE}🔍 Validating code formatting and style rules...${NC}"
    
    echo "${BLUE}📏 Checking style rules...${NC}"
    dotnet format --verify-no-changes --no-restore --severity error style
    check_res=$?
    if [ $check_res -ne 0 ]; then
        echo "${RED}❌ Code style rules violations detected!${NC}"
        echo "${YELLOW}💡 Run 'dotnet format' to fix style issues${NC}"
        exit 1;
    fi
    
    echo "${BLUE}📐 Checking whitespace formatting...${NC}"
    dotnet format --verify-no-changes --no-restore --severity error whitespace
    check_res=$?
    if [ $check_res -ne 0 ]; then
        echo "${RED}❌ Whitespace formatting violations detected!${NC}"
        echo "${YELLOW}💡 Run 'dotnet format' to fix whitespace issues${NC}"
        exit 1;
    fi

    echo "${GREEN}✅ All formatting and style rules are properly followed!${NC}"
fi
