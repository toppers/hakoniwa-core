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
else
	DOCKER_IMAGE=toppers/hakoniwa-client-builder
fi

HAKONIWA_TOP_DIR=$(cd .. && pwd)
DOCKER_IMAGE=${DOCKER_IMAGE}:v1.0.0

sudo docker ps > /dev/null
if [ $? -ne 0 ]
then
        sudo service docker start
        echo "waiting for docker service activation.. "
        sleep 3
fi

sudo docker run -v ${HAKONIWA_TOP_DIR}:/root/hakoniwa-core \
		-it --rm --net host --name hakoniwa-core ${DOCKER_IMAGE} 
