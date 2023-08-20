#!/bin/bash

docker pull 0646894062/atomiccore.iostorage.storageport

docker stop iostorage

docker rm iostorage

docker run -d -p 8019:80 \
--name=iostorage \
-e Logging__Console__FormatterName="" \
-e IOSTORAGE_APPTOKEN=a6e2f27ee1f544cc889898e4397f7b07 \
-e IOSTORAGE_SAVEROOTDIR=uploads \
-e IOSTORAGE_ALLOWFILEEXTS=".jpg,.jpeg,.gif,.xls,.xlsx,.doc,.docx,.ppt,.pptx,.wgt,.apk,.bmp,.png,.psd,.txt,.pdf" \
-e IOSTORAGE_ALLOWFILEMBSIZELIMIT=50 \
-v /data/wwwroot/static/dataprotection:/app/DataProtection:rw \
-v /data/wwwroot/static/uploads:/app/wwwroot/uploads:rw \
--privileged=true \
--restart=always \
-it 0646894062/atomiccore.iostorage.storageport

sleep 3
