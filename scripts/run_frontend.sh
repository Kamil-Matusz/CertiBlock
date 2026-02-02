#!/bin/bash

FRONTEND_DIR="../src_frontend"
PORT=5173
PACKAGE_MANAGER="npm"

echo "Killing process on port $PORT..."
node ./kill_port.js $PORT

echo "Installing dependencies in $FRONTEND_DIR..."
cd "$FRONTEND_DIR" || exit 1
$PACKAGE_MANAGER install

echo "Starting Vite dev server..."
$PACKAGE_MANAGER run dev &

cd ../scripts || exit 1
echo "Waiting for port $PORT..."
node ./check_port.js $PORT

echo "Warming up app..."
curl -sSL "http://localhost:$PORT" > /dev/null 2>&1 || \
wget -q -O /dev/null "http://localhost:$PORT"

echo "Frontend is running at http://localhost:$PORT"