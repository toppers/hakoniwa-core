#!/bin/bash

DOCKER_ID=`sudo docker ps | grep "toppers/hakoniwa-core:v1.0.0" | awk '{print $1}'`

sudo docker exec -it ${DOCKER_ID} /bin/bash
