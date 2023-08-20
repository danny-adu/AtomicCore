#!/bin/bash -e

docker build -f "D:\GitHub_Pros\AtomicCore\AtomicCore.IOStorage.StoragePort\Dockerfile" --force-rm -t 0646894062/atomiccore.iostorage.storageport:ws  --label "com.microsoft.created-by=visual-studio" --label "com.microsoft.visual-studio.project-name=AtomicCore.IOStorage.StoragePort" "D:\GitHub_Pros\AtomicCore"

docker push 0646894062/atomiccore.iostorage.storageport:ws

sleep 10