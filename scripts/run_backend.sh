#!/bin/bash

# Run all services in parallel
echo "Starting all API services..."

# Search for .Api folders in src and src_frontend directories
for dir in ../src/*/*.Api/ ../src_frontend/*/*.Api/; do
    if [ -d "$dir" ]; then
        echo "Starting service in $dir"
        (cd "$dir" && dotnet run) &
    fi
done

# Wait for all background processes
wait