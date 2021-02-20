#!/bin/bash

DOCKER_IMAGE=toppers/hakoniwa-core
DOCKER_TAG=v1.0.0
sudo docker build -t ${DOCKER_IMAGE}:${DOCKER_TAG} -f Dockerfile .

