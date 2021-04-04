#!/bin/bash

HAKONIWA_TOP_DIR=$(cd .. && pwd)
DOCKER_IMAGE=toppers/hakoniwa-core:v1.0.0

sudo docker ps > /dev/null
if [ $? -ne 0 ]
then
        sudo service docker start
        echo "waiting for docker service activation.. "
        sleep 3
fi

sudo docker run -v ${HAKONIWA_TOP_DIR}:/root/hakoniwa-core \
		-it --rm --net host --name hakoniwa-core ${DOCKER_IMAGE} 
