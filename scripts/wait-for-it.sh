#!/bin/sh
# Скрипт для проверки доступности порта
timeout=$1
host=$2
port=$3

for i in `seq $timeout` ; do
  nc -z "$host" "$port" > /dev/null 2>&1
  result=$?
  if [ $result -eq 0 ] ; then
    exit 0
  fi
  sleep 1
done
exit 1