#!/bin/bash

ATHRILL_TOP_DIR=$(cd ../../.. && pwd)
DOCKER_IMAGE=toppers/hakoniwa-core:v1.0.0

sudo docker run \
		-it --rm --net host --name hakoniwa-core ${DOCKER_IMAGE} 
