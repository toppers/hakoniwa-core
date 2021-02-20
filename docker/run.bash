#!/bin/bash

HAKONIWA_TOP_DIR=$(cd .. && pwd)
DOCKER_IMAGE=toppers/hakoniwa-core:v1.0.0

sudo docker run -v ${HAKONIWA_TOP_DIR}:/root/workspace/hakoniwa \
		-it --rm --net host --name hakoniwa-core ${DOCKER_IMAGE} 
