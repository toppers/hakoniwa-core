#!/bin/bash

if [ $# -ne 1 ]
then
	echo "Usage: $0 <ubuntu version>"
	exit 1
fi

VERSION=$1
if [ $VERSION = "18" ]
then
	DOCKER_IMAGE=toppers/hakoniwa-client-builder-18
	DOCKER_FILE=Dockerfile.ubuntu18
else
	DOCKER_IMAGE=toppers/hakoniwa-client-builder
	DOCKER_FILE=Dockerfile
fi
DOCKER_TAG=v1.0.0
sudo docker build -t ${DOCKER_IMAGE}:${DOCKER_TAG} -f ${DOCKER_FILE} .

