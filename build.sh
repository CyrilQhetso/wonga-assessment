#!/bin/bash
set -e

echo "Running backend tests..."
cd backend
dotnet test --verbosity minimal
cd ..

echo "Building and starting containers..."
docker compose down --remove-orphans
docker compose build --no-cache
docker compose up -d

echo "Application is running!"
echo "  Frontend: http://localhost:3000"
echo "  Backend API: http://localhost:5000"
echo "  Swagger: http://localhost:5000/swagger"