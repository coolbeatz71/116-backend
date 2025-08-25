#!/bin/sh

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
    which dotnet-format > /dev/null 2>&1;
    format_res=$?
    if [ $format_res -ne 0 ]; then
        echo "We are settled on benefiting from obeying formatting/linting rules using dotnet-format"
        echo "To proceed you need to have dotnet-format installed. Run the setup.sh script which can install it for you"
        exit 1;
    else
        FILES=$(git diff --staged --name-only --diff-filter=ACM "*.cs")
        [ -z "$FILES" ] && exit 0

        echo "Running dotnet-format for staged files"
        dotnet format --include "$FILES"

        echo "$FILES" | xargs git add

        echo "Formatting/linting complete"
    fi
else
    echo "Running dotnet-format --verify-no-changes"
    dotnet format --verify-no-changes --no-restore --severity error style
    check_res=$?
    if [ $check_res -ne 0 ]; then
        echo "👮 Formatting/linting rules are not obeyed"
        exit 1;
    fi
    dotnet format --verify-no-changes --no-restore --severity error whitespace
    check_res=$?
    if [ $check_res -ne 0 ]; then
        echo "👮 Formatting/linting rules are not obeyed"
        exit 1;
    fi
fi
